namespace WebSpark.ArtSpark.Agent.Configuration;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = "gpt-4o";
    public string VisionModelId { get; set; } = "gpt-4o";
    public int MaxTokens { get; set; } = 1000;
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 0.9;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(2);
    public int MaxRetries { get; set; } = 3;
}
