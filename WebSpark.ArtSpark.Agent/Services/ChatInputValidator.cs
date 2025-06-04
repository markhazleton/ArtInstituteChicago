using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using WebSpark.ArtSpark.Agent.Interfaces;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;

namespace WebSpark.ArtSpark.Agent.Services
{
    /// <summary>
    /// Service for validating chat input and implementing content guard rails
    /// to ensure appropriate and on-topic conversations about artwork
    /// </summary>
    public class ChatInputValidator : IChatInputValidator
    {
        private readonly ILogger<ChatInputValidator> _logger;

        // Content validation patterns and keywords
        private static readonly HashSet<string> InappropriateKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            // Add inappropriate content keywords here
            "spam", "advertisement", "buy now", "click here", "free money",
            // Offensive language (add as needed while being mindful of context)
            // Note: These should be carefully curated to avoid false positives
        };

        private static readonly HashSet<string> ArtRelatedKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "art", "artwork", "artist", "painting", "sculpture", "museum", "gallery", "exhibition",
            "color", "technique", "style", "medium", "canvas", "bronze", "stone", "clay",
            "composition", "perspective", "light", "shadow", "texture", "pattern", "design",
            "culture", "history", "period", "movement", "renaissance", "modern", "contemporary",
            "abstract", "realistic", "impressionist", "expressionist", "cubist", "surreal",
            "brush", "paint", "pigment", "oil", "watercolor", "acrylic", "charcoal", "pencil",
            "sketch", "drawing", "etching", "print", "photograph", "digital", "installation",
            "curator", "collection", "acquisition", "conservation", "restoration", "frame",
            "display", "lighting", "interpretation", "analysis", "criticism", "aesthetics",
            "beauty", "emotion", "meaning", "symbolism", "metaphor", "representation",
            "influence", "inspiration", "technique", "craftsmanship", "skill", "talent"
        };

        private static readonly Dictionary<ChatPersona, HashSet<string>> PersonaKeywords = new()
        {
            [ChatPersona.Artwork] = new(StringComparer.OrdinalIgnoreCase)
            {
                "feel", "experience", "emotion", "story", "journey", "creation", "life",
                "purpose", "meaning", "existence", "display", "viewed", "admired", "touched",
                "moved", "changed", "aged", "preserved", "traveled", "exhibited"
            },
            [ChatPersona.Artist] = new(StringComparer.OrdinalIgnoreCase)
            {
                "create", "inspiration", "technique", "process", "vision", "intention",
                "express", "communicate", "method", "tools", "materials", "practice",
                "develop", "experiment", "innovation", "tradition", "influence", "study"
            },
            [ChatPersona.Curator] = new(StringComparer.OrdinalIgnoreCase)
            {
                "collection", "acquisition", "research", "context", "interpretation",
                "exhibition", "display", "conservation", "preservation", "documentation",
                "provenance", "significance", "analysis", "scholarship", "expertise"
            },
            [ChatPersona.Historian] = new(StringComparer.OrdinalIgnoreCase)
            {
                "period", "era", "historical", "context", "influence", "movement", "development",
                "timeline", "chronology", "evolution", "comparison", "precedent", "legacy",
                "documentation", "records", "sources", "research", "analysis", "significance"
            }
        };

        // Message validation constants
        private const int MaxMessageLength = 500;
        private const int MinMessageLength = 2;

        // Regex patterns for validation
        private static readonly Regex ExcessiveRepeatingChars = new(@"(.)\1{4,}", RegexOptions.Compiled);
        private static readonly Regex ExcessivePunctuation = new(@"[!.?]{4,}", RegexOptions.Compiled);
        private static readonly Regex SpamPatterns = new(@"(https?://|www\.|\.com|\.org|\.net|buy\s+now|click\s+here|free\s+money)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ChatInputValidator(ILogger<ChatInputValidator> logger)
        {
            _logger = logger;
        }        /// <summary>
                 /// Validates a complete chat request against all guard rails and content policies
                 /// </summary>
                 /// <param name="request">The chat request to validate</param>
                 /// <returns>Validation result with success status and error details</returns>
        public ChatValidationResult ValidateRequest(ChatRequest request)
        {
            try
            {
                _logger.LogDebug("Starting validation for chat request - ArtworkId: {ArtworkId}, Persona: {Persona}",
                    request.ArtworkId, request.Persona);

                // 1. Basic format validation
                if (!IsValidFormat(request.Message))
                {
                    return CreateValidationError(request.Message);
                }

                // 2. Check for inappropriate content
                if (ContainsInappropriateContent(request.Message))
                {
                    _logger.LogWarning("Inappropriate content detected in message: {MessagePreview}",
                        request.Message.Substring(0, Math.Min(50, request.Message.Length)));

                    return ChatValidationResult.Failure(
                        ChatValidationError.InappropriateContent,
                        "Your message contains content that may not be appropriate for this educational platform. Please keep discussions respectful and family-friendly.",
                        "Try asking about the artwork's visual elements, historical context, or cultural significance."
                    );
                }

                // 3. Check if content is art-related
                if (!IsArtRelated(request.Message))
                {
                    _logger.LogInformation("Off-topic message detected: {MessagePreview}",
                        request.Message.Substring(0, Math.Min(50, request.Message.Length)));

                    return ChatValidationResult.Failure(
                        ChatValidationError.OffTopic,
                        "I'm here to discuss art and this artwork specifically. Your question doesn't seem to be related to art or this piece.",
                        GetArtTopicSuggestion(request.Persona)
                    );
                }

                // 4. Check persona appropriateness
                if (!IsPersonaAppropriate(request.Message, request.Persona))
                {
                    return ChatValidationResult.Failure(
                        ChatValidationError.PersonaMismatch,
                        $"Your question might be better suited for a different perspective. The {request.Persona} persona focuses on specific aspects of the artwork.",
                        GetPersonaSuggestion(request.Persona)
                    );
                }

                // 5. Additional validation could be added here (rate limiting, spam detection, etc.)

                _logger.LogDebug("Chat request validation passed successfully");
                return ChatValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat request validation");
                return ChatValidationResult.Failure(
                    ChatValidationError.ValidationError,
                    "An error occurred while validating your message. Please try again.",
                    "If the problem persists, try rephrasing your question."
                );
            }
        }

        /// <summary>
        /// Validates message format including length, structure, and basic content rules
        /// </summary>
        /// <param name="message">The user message to validate</param>
        /// <returns>True if the message format is valid</returns>
        public bool IsValidFormat(string message)
        {
            // Check for null or empty
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            // Check length constraints
            if (message.Length < MinMessageLength || message.Length > MaxMessageLength)
            {
                return false;
            }

            // Check for excessive repeating characters
            if (ExcessiveRepeatingChars.IsMatch(message))
            {
                return false;
            }

            // Check for excessive punctuation
            if (ExcessivePunctuation.IsMatch(message))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Detects potentially inappropriate, offensive, or harmful content
        /// </summary>
        /// <param name="message">The user message to check</param>
        /// <returns>True if inappropriate content is detected</returns>
        public bool ContainsInappropriateContent(string message)
        {
            // Check against inappropriate keywords
            foreach (var keyword in InappropriateKeywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Check for potential spam patterns
            if (SpamPatterns.IsMatch(message))
            {
                return true;
            }

            // Check for excessive capitalization (potential shouting)
            var upperCaseChars = message.Count(char.IsUpper);
            var totalLetters = message.Count(char.IsLetter);
            if (totalLetters > 10 && upperCaseChars > totalLetters * 0.7)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the message content is related to art, museums, or cultural topics
        /// </summary>
        /// <param name="message">The user message to analyze</param>
        /// <returns>True if the message is art-related</returns>
        public bool IsArtRelated(string message)
        {
            // Always allow greetings and basic conversation starters
            var greetings = new[] { "hello", "hi", "hey", "good morning", "good afternoon", "good evening", "thanks", "thank you" };
            if (greetings.Any(greeting => message.Trim().ToLowerInvariant().StartsWith(greeting)))
            {
                return true;
            }

            // Check for art-related keywords
            foreach (var keyword in ArtRelatedKeywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Check for art-related question words and phrases
            var artQuestionPhrases = new[]
            {
                "what is", "what does", "how was", "how did", "why did", "when was", "where is",
                "tell me about", "describe", "explain", "show me", "can you", "what inspired",
                "what technique", "what style", "what period", "what culture", "what material"
            };

            foreach (var phrase in artQuestionPhrases)
            {
                if (message.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // If message is short and seems like a continuation of conversation, allow it
            if (message.Length < 50 && (
                message.Contains("?") ||
                message.Contains("more") ||
                message.Contains("else") ||
                message.Contains("also") ||
                message.Contains("and")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates if the message content is appropriate for the selected persona
        /// </summary>
        /// <param name="message">The user message</param>
        /// <param name="persona">The selected chat persona</param>
        /// <returns>True if the message is appropriate for the persona</returns>
        public bool IsPersonaAppropriate(string message, ChatPersona persona)
        {
            // For now, we'll use a lenient approach - most art-related questions are appropriate for any persona
            // This could be made more strict in the future based on specific persona capabilities

            // Check if the message contains persona-specific keywords that suggest good alignment
            if (PersonaKeywords.TryGetValue(persona, out var keywords))
            {
                foreach (var keyword in keywords)
                {
                    if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            // Allow general art questions for any persona - they can redirect if needed
            return IsArtRelated(message);
        }

        /// <summary>
        /// Creates a validation error result based on format issues
        /// </summary>
        /// <param name="message">The message that failed validation</param>
        /// <returns>Appropriate validation error result</returns>
        private ChatValidationResult CreateValidationError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return ChatValidationResult.Failure(
                    ChatValidationError.EmptyMessage,
                    "Please enter a message to start our conversation about this artwork.",
                    "Try asking about what you see, the artwork's history, or its cultural significance."
                );
            }

            if (message.Length > MaxMessageLength)
            {
                return ChatValidationResult.Failure(
                    ChatValidationError.MessageTooLong,
                    $"Your message is too long. Please keep messages under {MaxMessageLength} characters.",
                    "Try breaking your question into smaller, more focused parts."
                );
            }

            if (message.Length < MinMessageLength)
            {
                return ChatValidationResult.Failure(
                    ChatValidationError.EmptyMessage,
                    "Your message is too short. Please provide more detail in your question.",
                    "Try asking a complete question about the artwork."
                );
            }

            return ChatValidationResult.Failure(
                ChatValidationError.ValidationError,
                "Your message contains formatting that might cause issues. Please try rephrasing.",
                "Avoid excessive punctuation or repeated characters."
            );
        }

        /// <summary>
        /// Gets a suggestion for art-related topics based on the persona
        /// </summary>
        /// <param name="persona">The current persona</param>
        /// <returns>A helpful suggestion for staying on topic</returns>
        private string GetArtTopicSuggestion(ChatPersona persona)
        {
            return persona switch
            {
                ChatPersona.Artwork => "Try asking about my colors, textures, or how I make you feel.",
                ChatPersona.Artist => "Ask about my creative process, inspiration, or the techniques I used.",
                ChatPersona.Curator => "I can discuss the artwork's significance, research, or place in our collection.",
                ChatPersona.Historian => "Ask about the historical context, period, or cultural movement this artwork represents.",
                _ => "Try asking about the artwork's visual elements, history, or cultural significance."
            };
        }

        /// <summary>
        /// Gets a suggestion for better persona alignment
        /// </summary>
        /// <param name="persona">The current persona</param>
        /// <returns>A suggestion for better persona-appropriate questions</returns>
        private string GetPersonaSuggestion(ChatPersona persona)
        {
            return persona switch
            {
                ChatPersona.Artwork => "I can share my personal experience and how I feel about being viewed and interpreted.",
                ChatPersona.Artist => "I can discuss my creative intentions, techniques, and the inspiration behind this work.",
                ChatPersona.Curator => "I can provide scholarly context, research insights, and museum perspectives.",
                ChatPersona.Historian => "I can explain the historical context, period influences, and cultural significance.",
                _ => "Each persona offers a unique perspective on the artwork."
            };
        }
    }
}
