namespace WebSpark.ArtSpark.Agent.Configuration;

public class LoggingSettings
{
    public bool EnableTelemetry { get; set; } = true;
    public bool LogConversations { get; set; } = false;
    public bool LogApiCalls { get; set; } = true;
    public string LogLevel { get; set; } = "Information";
}
