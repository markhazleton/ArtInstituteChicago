using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;
using WebSpark.ArtSpark.Agent.Configuration;
using WebSpark.ArtSpark.Agent.Interfaces;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;
namespace WebSpark.ArtSpark.Agent.Services;

public class ArtworkChatAgent : IArtworkChatAgent
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly IPersonaFactory _personaFactory;
    private readonly IArtworkDataProvider _artworkProvider;
    private readonly IChatMemory _chatMemory;
    private readonly ILogger<ArtworkChatAgent> _logger;
    private readonly AgentConfiguration _config;

    public ArtworkChatAgent(
        Kernel kernel,
        IPersonaFactory personaFactory,
        IArtworkDataProvider artworkProvider,
        IChatMemory chatMemory,
        ILogger<ArtworkChatAgent> logger,
        IOptions<AgentConfiguration> config)
    {
        _kernel = kernel;
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
        _personaFactory = personaFactory;
        _artworkProvider = artworkProvider;
        _chatMemory = chatMemory;
        _logger = logger;
        _config = config.Value;
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting chat for artwork {ArtworkId} with persona {Persona}",
                request.ArtworkId, request.Persona);

            // Get artwork data
            var artwork = await _artworkProvider.GetArtworkAsync(request.ArtworkId, cancellationToken);
            if (artwork == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    Error = "Artwork not found"
                };
            }

            // Enrich artwork data if needed
            artwork = await _artworkProvider.EnrichArtworkDataAsync(artwork, cancellationToken);

            // Create persona handler
            var persona = _personaFactory.CreatePersona(request.Persona, artwork);

            // Build chat history
            var chatHistory = await BuildChatHistoryAsync(request, artwork, persona, cancellationToken);

            // Get response from AI
            var settings = request.Settings ?? _config.DefaultChatSettings;
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = settings.MaxTokens,
                Temperature = settings.Temperature,
                TopP = settings.TopP
            };

            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                cancellationToken: cancellationToken);

            // Generate suggested questions
            var suggestedQuestions = await GenerateSuggestedQuestionsAsync(
                artwork, request.Message, request.Persona, cancellationToken);

            // Save conversation if memory is enabled
            if (_config.Cache.Enabled)
            {
                var updatedHistory = request.ConversationHistory.ToList();
                updatedHistory.Add(new ChatMessage { Role = MessageRole.User, Content = request.Message });
                updatedHistory.Add(new ChatMessage { Role = MessageRole.Assistant, Content = response.Content });

                await _chatMemory.SaveConversationAsync(
                    request.ArtworkId, request.Persona, updatedHistory, cancellationToken);
            }

            stopwatch.Stop();

            var chatResponse = new ChatResponse
            {
                Response = response.Content,
                Success = true,
                SuggestedQuestions = suggestedQuestions,
                Analytics = new ChatAnalytics
                {
                    TokensUsed = response.Metadata?.ContainsKey("Usage") == true
                        ? ExtractTokenUsage(response.Metadata["Usage"])
                        : 0,
                    ResponseTime = stopwatch.Elapsed,
                    ModelUsed = settings.ModelId,
                    VisionUsed = request.IncludeVisualAnalysis && !string.IsNullOrEmpty(artwork.ImageUrl)
                }
            };

            _logger.LogInformation("Chat completed for artwork {ArtworkId} in {ElapsedMs}ms",
                request.ArtworkId, stopwatch.ElapsedMilliseconds);

            return chatResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChatAsync for artwork {ArtworkId}", request.ArtworkId);
            return new ChatResponse
            {
                Success = false,
                Error = "An error occurred while processing your request",
                Analytics = new ChatAnalytics { ResponseTime = stopwatch.Elapsed }
            };
        }
    }

    private async Task<ChatHistory> BuildChatHistoryAsync(
        ChatRequest request,
        ArtworkData artwork,
        IPersonaHandler persona,
        CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory();

        // Add system prompt
        var systemPrompt = persona.GenerateSystemPrompt(artwork);
        chatHistory.AddSystemMessage(systemPrompt);

        // Add conversation history
        foreach (var msg in request.ConversationHistory)
        {
            switch (msg.Role)
            {
                case MessageRole.User:
                    chatHistory.AddUserMessage(msg.Content);
                    break;
                case MessageRole.Assistant:
                    chatHistory.AddAssistantMessage(msg.Content);
                    break;
            }
        }

        // Add current message with vision if enabled
        await AddUserMessageWithVisionAsync(chatHistory, request, artwork, cancellationToken);

        return chatHistory;
    }

    private async Task AddUserMessageWithVisionAsync(
        ChatHistory chatHistory,
        ChatRequest request,
        ArtworkData artwork,
        CancellationToken cancellationToken)
    {
        var needsVision = request.IncludeVisualAnalysis &&
                         !string.IsNullOrEmpty(artwork.ImageUrl) &&
                         (IsVisualAnalysisRequest(request.Message) || request.ConversationHistory.Count == 0);

        if (needsVision && _config.OpenAI.VisionModelId.Contains("vision") || _config.OpenAI.VisionModelId.Contains("4o"))
        {
            try
            {
                var messageContent = new ChatMessageContentItemCollection
                {
                    new TextContent(request.Message),
                    new ImageContent(new Uri(artwork.ImageUrl))
                };

                chatHistory.Add(new ChatMessageContent(AuthorRole.User, messageContent));
                _logger.LogDebug("Added vision-enabled message for artwork {ArtworkId}", artwork.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add vision content, falling back to text-only");
                chatHistory.AddUserMessage(request.Message);
            }
        }
        else
        {
            chatHistory.AddUserMessage(request.Message);
        }
    }

    private bool IsVisualAnalysisRequest(string message)
    {
        var visualKeywords = new[]
        {
            "see", "look", "color", "shape", "pattern", "design", "visual", "appearance",
            "texture", "carving", "details", "surface", "decoration", "style", "form",
            "composition", "describe", "show", "appears", "seems", "looks like"
        };

        return visualKeywords.Any(keyword =>
            message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private int ExtractTokenUsage(object usage)
    {
        // Implementation depends on the specific format of usage metadata
        // This is a placeholder - adjust based on actual metadata structure
        return 0;
    }

    public async Task<List<string>> GenerateConversationStartersAsync(
        ArtworkData artwork,
        ChatPersona persona,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var personaHandler = _personaFactory.CreatePersona(persona, artwork);
            return await personaHandler.GenerateConversationStartersAsync(artwork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating conversation starters for artwork {ArtworkId}", artwork.Id);
            return new List<string>
            {
                "Tell me about yourself",
                "What's your story?",
                "What makes you special?"
            };
        }
    }

    public async Task<List<string>> GenerateSuggestedQuestionsAsync(
        ArtworkData artwork,
        string lastMessage,
        ChatPersona persona,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildSuggestedQuestionsPrompt(artwork, lastMessage, persona);
            var response = await _chatService.GetChatMessageContentAsync(prompt, cancellationToken: cancellationToken);

            return ParseSuggestedQuestions(response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating suggested questions for artwork {ArtworkId}", artwork.Id);
            return GetDefaultSuggestedQuestions(persona);
        }
    }

    private string BuildSuggestedQuestionsPrompt(ArtworkData artwork, string lastMessage, ChatPersona persona)
    {
        var personaContext = persona switch
        {
            ChatPersona.Artwork => "as the artwork itself",
            ChatPersona.Artist => "as the artist who created this work",
            ChatPersona.Curator => "from a curatorial perspective",
            ChatPersona.Historian => "from a historical context",
            _ => ""
        };

        return $@"
Based on this conversation about '{artwork.Title}' {personaContext}, suggest 3 engaging follow-up questions.

Artwork: {artwork.Title} by {artwork.ArtistDisplay}
Origin: {artwork.PlaceOfOrigin}, {artwork.DateDisplay}
Cultural Context: {artwork.CulturalContext}
User's last message: {lastMessage}

Generate 3 natural, conversational questions that would help deepen the user's understanding.
Consider the {persona.ToString().ToLower()} perspective and include questions about:
- Visual elements and artistic techniques
- Cultural significance and historical context  
- Personal stories and emotional connections
- Contemporary relevance and interpretation

Format as a simple numbered list.
";
    }

    private List<string> ParseSuggestedQuestions(string response)
    {
        return response.Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Trim().TrimStart('-', '*', '1', '2', '3', '.', ' '))
            .Where(line => line.Length > 10) // Filter out too-short responses
            .Take(3)
            .ToList();
    }

    private List<string> GetDefaultSuggestedQuestions(ChatPersona persona)
    {
        return persona switch
        {
            ChatPersona.Artwork => new List<string>
            {
                "What do your colors and patterns mean?",
                "Tell me about your cultural significance",
                "How do you feel about being in a museum?"
            },
            ChatPersona.Artist => new List<string>
            {
                "What inspired you to create this piece?",
                "What techniques did you use?",
                "What does this work mean to your community?"
            },
            ChatPersona.Curator => new List<string>
            {
                "How did this artwork come to the museum?",
                "What research has been done on this piece?",
                "How does this compare to similar works?"
            },
            ChatPersona.Historian => new List<string>
            {
                "What was happening historically when this was created?",
                "How did cultural exchanges influence this work?",
                "What can this teach us about its time period?"
            },
            _ => new List<string>
            {
                "Tell me more about this artwork",
                "What makes this piece special?",
                "What should I know about its history?"
            }
        };
    }

    public async Task<string> AnalyzeArtworkVisuallyAsync(
        ArtworkData artwork,
        string? specificQuestion = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(artwork.ImageUrl))
            {
                return "I don't have access to a visual representation of this artwork.";
            }

            var prompt = specificQuestion ?? "Provide a detailed visual analysis of this artwork, describing its colors, patterns, textures, composition, and notable artistic elements.";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are an expert art analyst. Provide detailed, accurate visual descriptions based on what you can see in the image.");

            var messageContent = new ChatMessageContentItemCollection
            {
                new TextContent(prompt),
                new ImageContent(new Uri(artwork.ImageUrl))
            };

            chatHistory.Add(new ChatMessageContent(AuthorRole.User, messageContent));

            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.3 },
                cancellationToken: cancellationToken);

            return response.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in visual analysis for artwork {ArtworkId}", artwork.Id);
            return "I'm unable to perform visual analysis at this time.";
        }
    }
}
