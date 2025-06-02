using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;
namespace WebSpark.ArtSpark.Agent.Interfaces
{
    public interface IPersonaHandler
    {
        string GenerateSystemPrompt(ArtworkData artwork);
        Task<string> ProcessMessageAsync(string message, ArtworkData artwork, List<ChatMessage> history);
        Task<List<string>> GenerateConversationStartersAsync(ArtworkData artwork);
        ChatPersona PersonaType { get; }
    }
}
