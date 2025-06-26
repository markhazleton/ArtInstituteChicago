namespace WebSpark.ArtSpark.Demo.Constants;

/// <summary>
/// Static readonly arrays for health check tags to address CA1861 warnings
/// These arrays are reused across health check configurations
/// </summary>
public static class HealthCheckTags
{
    public static readonly string[] Database = ["database", "entity-framework"];
    public static readonly string[] External = ["external", "api"];
    public static readonly string[] Network = ["network", "connectivity"];
    public static readonly string[] Storage = ["storage", "file-system"];
    public static readonly string[] Cache = ["cache", "memory"];
    public static readonly string[] Security = ["security", "authentication"];
}
