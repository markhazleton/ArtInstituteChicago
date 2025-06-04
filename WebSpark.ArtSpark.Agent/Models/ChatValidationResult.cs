namespace WebSpark.ArtSpark.Agent.Models
{
    /// <summary>
    /// Result of chat input validation containing success status and error details
    /// </summary>
    public class ChatValidationResult
    {
        /// <summary>
        /// Gets or sets whether the validation passed
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the validation error message if validation failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the specific validation error code for categorization
        /// </summary>
        public ChatValidationError ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets additional context or suggestions for the user
        /// </summary>
        public string? Suggestion { get; set; }

        /// <summary>
        /// Gets or sets metadata about the validation process
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Creates a successful validation result
        /// </summary>
        /// <returns>A validation result indicating success</returns>
        public static ChatValidationResult Success() => new()
        {
            IsValid = true,
            ErrorCode = ChatValidationError.None
        };

        /// <summary>
        /// Creates a failed validation result with error details
        /// </summary>
        /// <param name="errorCode">The specific error code</param>
        /// <param name="errorMessage">The error message to display</param>
        /// <param name="suggestion">Optional suggestion for the user</param>
        /// <returns>A validation result indicating failure</returns>
        public static ChatValidationResult Failure(ChatValidationError errorCode, string errorMessage, string? suggestion = null) => new()
        {
            IsValid = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            Suggestion = suggestion
        };
    }

    /// <summary>
    /// Enumeration of possible chat validation error types
    /// </summary>
    public enum ChatValidationError
    {
        /// <summary>
        /// No error - validation passed
        /// </summary>
        None = 0,

        /// <summary>
        /// Message is empty or contains only whitespace
        /// </summary>
        EmptyMessage = 1,

        /// <summary>
        /// Message exceeds maximum allowed length
        /// </summary>
        MessageTooLong = 2,

        /// <summary>
        /// Message contains inappropriate or offensive content
        /// </summary>
        InappropriateContent = 3,

        /// <summary>
        /// Message is not related to art or the museum context
        /// </summary>
        OffTopic = 4,

        /// <summary>
        /// Message is not appropriate for the selected persona
        /// </summary>
        PersonaMismatch = 5,

        /// <summary>
        /// Message contains potential spam or repeated content
        /// </summary>
        SpamDetected = 6,

        /// <summary>
        /// Message contains potentially harmful or dangerous content
        /// </summary>
        HarmfulContent = 7,

        /// <summary>
        /// Rate limiting exceeded
        /// </summary>
        RateLimitExceeded = 8,

        /// <summary>
        /// General validation error
        /// </summary>
        ValidationError = 9
    }
}
