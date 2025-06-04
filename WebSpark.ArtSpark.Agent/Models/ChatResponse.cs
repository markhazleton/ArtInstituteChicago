namespace WebSpark.ArtSpark.Agent.Models
{
    public class ChatResponse
    {
        public string Response { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
        public List<string> SuggestedQuestions { get; set; } = new();
        public ChatAnalytics Analytics { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Creates a ChatResponse for validation errors with appropriate suggestions
        /// </summary>
        /// <param name="validationResult">The validation result containing error details</param>
        /// <returns>A ChatResponse indicating validation failure</returns>
        public static ChatResponse FromValidationError(ChatValidationResult validationResult)
        {
            var suggestions = new List<string>();

            // Add the suggestion from validation result if available
            if (!string.IsNullOrWhiteSpace(validationResult.Suggestion))
            {
                suggestions.Add(validationResult.Suggestion);
            }

            // Add error-specific suggestions
            suggestions.AddRange(validationResult.ErrorCode switch
            {
                ChatValidationError.EmptyMessage => new[]
                {
                    "What do you see in this artwork?",
                    "Tell me about the colors and shapes.",
                    "What does this artwork make you feel?"
                },
                ChatValidationError.MessageTooLong => new[]
                {
                    "What's the main thing you'd like to know?",
                    "Can you focus on one specific aspect?",
                    "What interests you most about this piece?"
                },
                ChatValidationError.OffTopic => new[]
                {
                    "What draws your eye in this artwork?",
                    "How does this piece make you feel?",
                    "What questions do you have about the artist's technique?"
                },
                ChatValidationError.PersonaMismatch => new[]
                {
                    "What would you like to explore about this artwork?",
                    "Is there a particular aspect that interests you?",
                    "What perspective would be most helpful?"
                },
                _ => new[]
                {
                    "What would you like to know about this artwork?",
                    "How can I help you explore this piece?",
                    "What aspect interests you most?"
                }
            });

            return new ChatResponse
            {
                Success = false,
                Error = validationResult.ErrorMessage ?? "Your message didn't meet our content guidelines.",
                SuggestedQuestions = suggestions.Take(3).ToList(),
                Metadata = new Dictionary<string, object>
                {
                    ["ValidationError"] = validationResult.ErrorCode.ToString(),
                    ["OriginalSuggestion"] = validationResult.Suggestion ?? string.Empty
                }
            };
        }
    }
}
