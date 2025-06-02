using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;
namespace WebSpark.ArtSpark.Agent.Interfaces
{
    public interface IChatMemory
    {
        Task SaveConversationAsync(int artworkId, ChatPersona persona, List<ChatMessage> messages, CancellationToken cancellationToken = default);
        Task<List<ChatMessage>> GetConversationHistoryAsync(int artworkId, ChatPersona persona, int maxMessages = 10, CancellationToken cancellationToken = default);
        Task ClearConversationAsync(int artworkId, ChatPersona persona, CancellationToken cancellationToken = default);
    }
}
