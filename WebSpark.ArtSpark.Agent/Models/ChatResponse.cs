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
    }
}
