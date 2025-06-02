namespace WebSpark.ArtSpark.Agent.Models
{
    public class ChatAnalytics
    {
        public int TokensUsed { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string ModelUsed { get; set; } = string.Empty;
        public bool VisionUsed { get; set; }
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
    }
}
