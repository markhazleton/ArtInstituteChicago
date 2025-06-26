using System.Diagnostics;

namespace WebSpark.ArtSpark.Demo.Telemetry;

/// <summary>
/// Custom telemetry service for application monitoring without external dependencies
/// Provides basic telemetry functionality using built-in .NET capabilities
/// </summary>
public class CustomTelemetryService
{
    private readonly ILogger<CustomTelemetryService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomTelemetryService(ILogger<CustomTelemetryService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Track a custom event with properties
    /// </summary>
    public void TrackEvent(string eventName, Dictionary<string, string>? properties = null)
    {
        var context = GetTelemetryContext();
        var logMessage = $"Event: {eventName}";

        if (properties?.Any() == true)
        {
            var propertiesStr = string.Join(", ", properties.Select(p => $"{p.Key}={p.Value}"));
            logMessage += $" | Properties: {propertiesStr}";
        }

        logMessage += $" | Context: {context}";

        _logger.LogInformation("{EventName} tracked with context {Context}", eventName, context);
    }

    /// <summary>
    /// Track a page view
    /// </summary>
    public void TrackPageView(string pageName, string? url = null, TimeSpan? duration = null)
    {
        var context = GetTelemetryContext();
        var properties = new Dictionary<string, string>
        {
            ["PageName"] = pageName,
            ["Url"] = url ?? _httpContextAccessor.HttpContext?.Request.Path.ToString() ?? "Unknown",
            ["Duration"] = duration?.TotalMilliseconds.ToString() ?? "0",
            ["UserAgent"] = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown"
        };

        TrackEvent("PageView", properties);
    }

    /// <summary>
    /// Track an exception
    /// </summary>
    public void TrackException(Exception exception, Dictionary<string, string>? properties = null)
    {
        var context = GetTelemetryContext();
        var allProperties = new Dictionary<string, string>
        {
            ["ExceptionType"] = exception.GetType().Name,
            ["Message"] = exception.Message,
            ["StackTrace"] = exception.StackTrace ?? "No stack trace available"
        };

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                allProperties[prop.Key] = prop.Value;
            }
        }

        _logger.LogError(exception, "Exception tracked: {ExceptionType} with context {Context}",
            exception.GetType().Name, context);

        TrackEvent("Exception", allProperties);
    }

    /// <summary>
    /// Track a dependency call (e.g., external API, database)
    /// </summary>
    public void TrackDependency(string dependencyType, string target, string commandName,
        TimeSpan duration, bool success, string? resultCode = null)
    {
        var properties = new Dictionary<string, string>
        {
            ["DependencyType"] = dependencyType,
            ["Target"] = target,
            ["CommandName"] = commandName,
            ["Duration"] = duration.TotalMilliseconds.ToString(),
            ["Success"] = success.ToString(),
            ["ResultCode"] = resultCode ?? (success ? "200" : "500")
        };

        TrackEvent("Dependency", properties);
    }

    /// <summary>
    /// Track a custom metric
    /// </summary>
    public void TrackMetric(string metricName, double value, Dictionary<string, string>? properties = null)
    {
        var allProperties = new Dictionary<string, string>
        {
            ["MetricName"] = metricName,
            ["Value"] = value.ToString("F2")
        };

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                allProperties[prop.Key] = prop.Value;
            }
        }

        _logger.LogInformation("Metric {MetricName} = {Value}", metricName, value);
        TrackEvent("Metric", allProperties);
    }

    /// <summary>
    /// Get current telemetry context information
    /// </summary>
    private string GetTelemetryContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var activity = Activity.Current;

        var context = new
        {
            TraceId = activity?.TraceId.ToString() ?? "No-Trace",
            SpanId = activity?.SpanId.ToString() ?? "No-Span",
            UserId = httpContext?.User?.Identity?.Name ?? "Anonymous",
            SessionId = httpContext?.Session?.Id ?? "No-Session",
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown",
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            RequestPath = httpContext?.Request.Path.ToString() ?? "Unknown",
            Timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };

        return System.Text.Json.JsonSerializer.Serialize(context);
    }
}

/// <summary>
/// Extension methods for registering custom telemetry service
/// </summary>
public static class TelemetryServiceExtensions
{
    /// <summary>
    /// Add custom telemetry service to DI container
    /// </summary>
    public static IServiceCollection AddCustomTelemetry(this IServiceCollection services)
    {
        services.AddSingleton<CustomTelemetryService>();
        return services;
    }

    /// <summary>
    /// Configure telemetry middleware in the request pipeline
    /// </summary>
    public static IApplicationBuilder UseCustomTelemetry(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TelemetryMiddleware>();
    }
}

/// <summary>
/// Middleware to automatically track request telemetry
/// </summary>
public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CustomTelemetryService _telemetry;
    private readonly ILogger<TelemetryMiddleware> _logger;

    public TelemetryMiddleware(RequestDelegate next, CustomTelemetryService telemetry, ILogger<TelemetryMiddleware> logger)
    {
        _next = next;
        _telemetry = telemetry;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.ToString();

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Track successful request
            _telemetry.TrackEvent("RequestCompleted", new Dictionary<string, string>
            {
                ["Path"] = requestPath,
                ["Method"] = context.Request.Method,
                ["StatusCode"] = context.Response.StatusCode.ToString(),
                ["Duration"] = stopwatch.ElapsedMilliseconds.ToString(),
                ["Success"] = (context.Response.StatusCode < 400).ToString()
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Track failed request
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                ["Path"] = requestPath,
                ["Method"] = context.Request.Method,
                ["Duration"] = stopwatch.ElapsedMilliseconds.ToString()
            });

            throw;
        }
    }
}
