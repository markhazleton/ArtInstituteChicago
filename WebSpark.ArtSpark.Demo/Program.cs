using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.RateLimiting;
using WebSpark.ArtSpark.Agent.Extensions;
using WebSpark.ArtSpark.Client.Clients;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Demo.Data;
using WebSpark.ArtSpark.Demo.HttpClientUtility.MemoryCache;
using WebSpark.ArtSpark.Demo.Models;
using WebSpark.ArtSpark.Demo.Services;
using WebSpark.ArtSpark.Demo.Utilities;
using WebSpark.Bootswatch;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
builder.ConfigureLogging();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add HttpContextAccessor for Tag Helper support
builder.Services.AddHttpContextAccessor();

// Performance: Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Performance: Response Caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1MB
    options.UseCaseSensitivePaths = false;
});

// Performance: Memory Cache with limits
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000; // Limit number of entries
    options.CompactionPercentage = 0.25; // Remove 25% when limit reached
});

// Security: Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // API Rate Limiting
    options.AddFixedWindowLimiter("api", configure =>
    {
        configure.PermitLimit = 100;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configure.QueueLimit = 10;
    });

    // General Rate Limiting for web requests
    options.AddSlidingWindowLimiter("general", configure =>
    {
        configure.PermitLimit = 200;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.SegmentsPerWindow = 4;
        configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configure.QueueLimit = 20;
    });
});

// Health Checks with comprehensive monitoring
builder.Services.AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db",
               name: "database",
               tags: new[] { "db", "ready" })
    .AddCheck("art_institute_api", () =>
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = client.GetAsync("https://api.artic.edu/api/v1/artworks").GetAwaiter().GetResult();
            return response.IsSuccessStatusCode ?
                HealthCheckResult.Healthy("Art Institute API is responding") :
                HealthCheckResult.Degraded($"Art Institute API returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Art Institute API is unreachable", ex);
        }
    }, tags: new[] { "external", "ready" })
    .AddCheck("memory", () =>
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var description = $"Memory: {allocated / (1024 * 1024)}MB, Gen0: {GC.CollectionCount(0)}, Gen1: {GC.CollectionCount(1)}, Gen2: {GC.CollectionCount(2)}";

        return allocated < 512L * 1024L * 1024L ? // 512MB limit
            HealthCheckResult.Healthy(description) :
            HealthCheckResult.Degraded(description);
    }, tags: new[] { "memory", "ready" })
    .AddCheck("disk_space", () =>
    {
        var drive = new DriveInfo(Environment.CurrentDirectory);
        var freeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0);
        var description = $"Free space: {freeSpaceGB:F2}GB of {drive.TotalSize / (1024.0 * 1024.0 * 1024.0):F2}GB";

        return freeSpaceGB > 1.0 ? // 1GB minimum
            HealthCheckResult.Healthy(description) :
            HealthCheckResult.Unhealthy(description);
    }, tags: new[] { "storage", "ready" });

// Add Database Context
builder.Services.AddDbContext<ArtSparkDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use SQLite for development
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        // Use SQLite for production as well
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Add Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ArtSparkDbContext>()
.AddDefaultTokenProviders();

// Add Review Services
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IPublicCollectionService, PublicCollectionService>();
builder.Services.AddSingleton<IBuildInfoService, BuildInfoService>();

// Add SEO Optimization Service
builder.Services.AddScoped<ISeoOptimizationService, SeoOptimizationService>();

// Add HttpClient factory (required for WebSpark.HttpClientUtility)
builder.Services.AddHttpClient();

// Add ArtSpark Agent services
builder.Services.AddArtSparkAgent(builder.Configuration, config =>
{
    // Override default settings if needed
    config.OpenAI.ModelId = "gpt-4o";
    config.OpenAI.Temperature = 0.7;
    config.Cache.Enabled = true;
});

RegisterHttpClientUtilities(builder);

builder.Services.AddSingleton<IStringConverter, SystemJsonStringConverter>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<HttpRequestResultService>();
builder.Services.AddScoped<IArtInstituteClient, ArtInstituteClient>();

// Register IHttpRequestResultService with telemetry decorator
builder.Services.AddScoped<IHttpRequestResultService>(provider =>
{
    IHttpRequestResultService service = provider.GetRequiredService<HttpRequestResultService>();

    // Add Telemetry (basic decorator for logging and monitoring)
    service = new HttpRequestResultServiceTelemetry(
        provider.GetRequiredService<ILogger<HttpRequestResultServiceTelemetry>>(),
        service
    );

    return service;
});

// Add WebSpark.Bootswatch theme switcher
builder.Services.AddBootswatchThemeSwitcher();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Performance middleware
app.UseResponseCompression();
app.UseResponseCaching();

// Security middleware
app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseRouting();

// Configure WebSpark.Bootswatch (includes static files and style cache)
app.UseBootswatchAll();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Always healthy for liveness
});

// SEO-friendly collection routes
app.MapControllerRoute(
    name: "collectionBySlug",
    pattern: "collection/{slug}",
    defaults: new { controller = "PublicCollections", action = "Details" });

app.MapControllerRoute(
    name: "collectionItemBySlug",
    pattern: "collection/{collectionSlug}/item/{itemSlug}",
    defaults: new { controller = "Account", action = "CollectionItemDetails" });

app.MapControllerRoute(
    name: "collections",
    pattern: "collections",
    defaults: new { controller = "Account", action = "Collections" });

app.MapControllerRoute(
    name: "publicCollections",
    pattern: "explore/collections",
    defaults: new { controller = "PublicCollections", action = "Index" });

app.MapControllerRoute(
    name: "featuredCollections",
    pattern: "explore/featured",
    defaults: new { controller = "PublicCollections", action = "Featured" });

app.MapControllerRoute(
    name: "recentCollections",
    pattern: "explore/recent",
    defaults: new { controller = "PublicCollections", action = "Recent" });

app.MapControllerRoute(
    name: "popularCollections",
    pattern: "explore/popular",
    defaults: new { controller = "PublicCollections", action = "Popular" });

app.MapControllerRoute(
    name: "collectionsByTag",
    pattern: "explore/collections/tag/{tag}",
    defaults: new { controller = "PublicCollections", action = "ByTag" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();


static void RegisterHttpClientUtilities(WebApplicationBuilder builder)
{
    builder.Services.AddHttpClient("HttpClientService", client =>
    {
        client.Timeout = TimeSpan.FromMilliseconds(90000);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "HttpClientService");
        client.DefaultRequestHeaders.Add("X-Request-ID", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Add("X-Request-Source", "HttpClientService");
    });

    // Register IHttpClientMemoryCache implementation
    builder.Services.AddSingleton<IHttpClientMemoryCache, HttpClientMemoryCache>();

    // HTTP Send Service with Decorator Pattern
    builder.Services.AddSingleton(serviceProvider =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var retryOptions = configuration.GetSection("HttpRequestResultPollyOptions").Get<HttpRequestResultPollyOptions>();

        IHttpRequestResultService baseService = new HttpRequestResultService(
            serviceProvider.GetRequiredService<ILogger<HttpRequestResultService>>(),
            configuration,
            serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("HttpClientDecorator"));

        IHttpRequestResultService pollyService = new HttpRequestResultServicePolly(
            serviceProvider.GetRequiredService<ILogger<HttpRequestResultServicePolly>>(),
            baseService,
            retryOptions);

        IHttpRequestResultService telemetryService = new HttpRequestResultServiceTelemetry(
            serviceProvider.GetRequiredService<ILogger<HttpRequestResultServiceTelemetry>>(),
            pollyService);

        IHttpRequestResultService cacheService = new HttpRequestResultServiceCache(
            telemetryService,
            serviceProvider.GetRequiredService<ILogger<HttpRequestResultServiceCache>>(),
            serviceProvider.GetRequiredService<IMemoryCache>()); return cacheService;
    });
}

// Make the Program class accessible for testing
public partial class Program { }
