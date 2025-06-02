using WebSpark.ArtSpark.Agent.Models;
namespace WebSpark.ArtSpark.Agent.Configuration;

public class AgentConfiguration
{
    public OpenAISettings OpenAI { get; set; } = new();
    public ArtInstituteSettings ArtInstitute { get; set; } = new();
    public ChatSettings DefaultChatSettings { get; set; } = new();
    public CacheSettings Cache { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
}
