namespace WebSpark.ArtSpark.Demo.Configuration;

/// <summary>
/// Performance configuration settings
/// Centralizes performance-related configuration for easy tuning
/// </summary>
public class PerformanceSettings
{
    public const string SectionName = "Performance";

    /// <summary>
    /// HTTP client timeout in seconds (default: 30)
    /// </summary>
    public int HttpClientTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum concurrent HTTP requests (default: 10)
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 10;

    /// <summary>
    /// Response caching duration in minutes (default: 5)
    /// </summary>
    public int ResponseCachingMinutes { get; set; } = 5;

    /// <summary>
    /// Memory cache size limit in MB (default: 100)
    /// </summary>
    public int MemoryCacheSizeLimitMB { get; set; } = 100;

    /// <summary>
    /// Database command timeout in seconds (default: 30)
    /// </summary>
    public int DatabaseCommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable compression for responses (default: true)
    /// </summary>
    public bool EnableResponseCompression { get; set; } = true;

    /// <summary>
    /// Rate limiting: requests per minute per IP (default: 100)
    /// </summary>
    public int RateLimitRequestsPerMinute { get; set; } = 100;
}
