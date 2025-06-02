namespace WebSpark.ArtSpark.Agent.Configuration;

public class CacheSettings
{
    public bool Enabled { get; set; } = true;
    public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan ArtworkDataExpiry { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan ConversationExpiry { get; set; } = TimeSpan.FromMinutes(30);
    public int MaxCacheSize { get; set; } = 1000;
}
