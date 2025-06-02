using WebSpark.ArtSpark.Agent.Personas;

namespace WebSpark.ArtSpark.Agent.Models
{
    public class ChatRequest
    {
        public int ArtworkId { get; set; }
        public string Message { get; set; } = string.Empty;
        public ChatPersona Persona { get; set; } = ChatPersona.Artwork;
        public List<ChatMessage> ConversationHistory { get; set; } = new();
        public bool IncludeVisualAnalysis { get; set; } = true;
        public ChatSettings? Settings { get; set; }
    }
}
