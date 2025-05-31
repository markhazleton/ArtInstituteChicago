using Serilog;
using Serilog.Events;

namespace WebSpark.ArtSpark.Demo.Utilities;

/// <summary>
/// Utility class for configuring Serilog logging with file system output
/// </summary>
public static class LoggingUtility
{
    /// <summary>
    /// Configures Serilog for file system logging with rolling intervals and EF Core filtering
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance</param>
    /// <returns>The configured WebApplicationBuilder</returns>
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        // Clear existing logging providers
        builder.Logging.ClearProviders();

        // Get log file path from configuration
        var logFilePath = builder.Configuration["WebSpark:LogFilePath"] ?? "logs/artspark-.txt";

        // Configure Serilog
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                shared: true,
                buffered: false)
            .CreateLogger();

        // Use Serilog as the logging provider
        builder.Host.UseSerilog(logger);

        return builder;
    }
}
