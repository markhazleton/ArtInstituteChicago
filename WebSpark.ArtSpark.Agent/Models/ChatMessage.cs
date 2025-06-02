namespace WebSpark.ArtSpark.Agent.Models
{
    public class ChatMessage
    {
        public MessageRole Role { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
