using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;

namespace WebSpark.ArtSpark.Agent.Interfaces
{
    /// <summary>
    /// Interface for validating chat input and implementing guard rails to ensure appropriate content
    /// </summary>
    public interface IChatInputValidator
    {        /// <summary>
             /// Validates a chat request against content policies and persona appropriateness
             /// </summary>
             /// <param name="request">The chat request to validate</param>
             /// <returns>Validation result with success status and error details if applicable</returns>
        ChatValidationResult ValidateRequest(ChatRequest request);

        /// <summary>
        /// Validates if the message content is appropriate for the selected persona
        /// </summary>
        /// <param name="message">The user message</param>
        /// <param name="persona">The selected persona</param>
        /// <returns>True if the message is appropriate for the persona</returns>
        bool IsPersonaAppropriate(string message, ChatPersona persona);

        /// <summary>
        /// Checks if the message content is art-related and on-topic
        /// </summary>
        /// <param name="message">The user message</param>
        /// <returns>True if the message is art-related</returns>
        bool IsArtRelated(string message);

        /// <summary>
        /// Detects potentially inappropriate or offensive content
        /// </summary>
        /// <param name="message">The user message</param>
        /// <returns>True if content is inappropriate</returns>
        bool ContainsInappropriateContent(string message);

        /// <summary>
        /// Validates message length and basic formatting
        /// </summary>
        /// <param name="message">The user message</param>
        /// <returns>True if message format is valid</returns>
        bool IsValidFormat(string message);
    }
}
