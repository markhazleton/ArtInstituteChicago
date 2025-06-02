namespace WebSpark.ArtSpark.Agent.Models
{
    public class ChatSettings
    {
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public double TopP { get; set; } = 0.9;
        public string ModelId { get; set; } = "gpt-4o";
        public bool EnableVision { get; set; } = true;
        public string Language { get; set; } = "en";
    }
}
