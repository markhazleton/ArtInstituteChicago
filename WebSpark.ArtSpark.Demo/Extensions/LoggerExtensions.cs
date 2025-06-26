namespace WebSpark.ArtSpark.Demo.Extensions;

/// <summary>
/// High-performance logging extensions using LoggerMessage delegates
/// Addresses CA1848 warnings by providing compiled logging methods
/// </summary>
public static partial class LoggerExtensions
{
    // Error logging delegates
    [LoggerMessage(1001, LogLevel.Error, "An error occurred in {Controller}.{Action}: {Message}")]
    public static partial void LogControllerError(this ILogger logger, string controller, string action, string message, Exception exception);

    [LoggerMessage(1002, LogLevel.Error, "Database operation failed: {Operation}")]
    public static partial void LogDatabaseError(this ILogger logger, string operation, Exception exception);

    [LoggerMessage(1003, LogLevel.Error, "HTTP client error for {Url}: {StatusCode}")]
    public static partial void LogHttpClientError(this ILogger logger, string url, int statusCode, Exception exception);

    [LoggerMessage(1004, LogLevel.Error, "SEO optimization failed for {Type} {Id}")]
    public static partial void LogSeoOptimizationError(this ILogger logger, string type, int id, Exception exception);

    // Warning logging delegates
    [LoggerMessage(2001, LogLevel.Warning, "{Controller}.{Action}: {Message}")]
    public static partial void LogControllerWarning(this ILogger logger, string controller, string action, string message);

    [LoggerMessage(2002, LogLevel.Warning, "Resource not found: {ResourceType} with ID {Id}")]
    public static partial void LogResourceNotFound(this ILogger logger, string resourceType, int id);

    [LoggerMessage(2003, LogLevel.Warning, "Invalid request parameter: {Parameter} = {Value}")]
    public static partial void LogInvalidParameter(this ILogger logger, string parameter, string value);

    // Information logging delegates
    [LoggerMessage(3001, LogLevel.Information, "{Controller}.{Action} - Processing request for {Resource}")]
    public static partial void LogControllerAction(this ILogger logger, string controller, string action, string resource);

    [LoggerMessage(3002, LogLevel.Information, "User {UserId} performed {Action} on {Resource} {ResourceId}")]
    public static partial void LogUserAction(this ILogger logger, string userId, string action, string resource, int resourceId);

    [LoggerMessage(3003, LogLevel.Information, "SEO optimization completed for {Type} {Id}: {Description}")]
    public static partial void LogSeoOptimizationSuccess(this ILogger logger, string type, int id, string description);

    [LoggerMessage(3004, LogLevel.Information, "Collection {CollectionId} enriched with {Count} items")]
    public static partial void LogCollectionEnriched(this ILogger logger, int collectionId, int count);
}
