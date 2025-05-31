using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSpark.ArtSpark.Client.Clients;
using WebSpark.ArtSpark.Client.Models.Common;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;

// Setup DI
var services = new ServiceCollection();

// Add HttpContextAccessor for Tag Helper support
services.AddHttpContextAccessor();

// Add HttpClient factory (required for WebSpark.HttpClientUtility)
services.AddHttpClient();

// Register WebSpark.HttpClientUtility services
services.AddSingleton<IStringConverter, SystemJsonStringConverter>();
services.AddScoped<IHttpClientService, HttpClientService>();
services.AddScoped<HttpRequestResultService>();

// Register IHttpRequestResultService with telemetry decorator
services.AddScoped<IHttpRequestResultService>(provider =>
{
    IHttpRequestResultService service = provider.GetRequiredService<HttpRequestResultService>();

    // Add Telemetry (basic decorator for logging and monitoring)
    service = new HttpRequestResultServiceTelemetry(
        provider.GetRequiredService<ILogger<HttpRequestResultServiceTelemetry>>(),
        service
    );

    return service;
});

// Add logging (required for Telemetry)
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

services.AddSingleton<IConfiguration>(configuration);

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Resolve dependencies
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient();
var httpRequestResultService = serviceProvider.GetRequiredService<IHttpRequestResultService>();

// Pass IHttpRequestResultService to ArtInstituteClient
var client = new ArtInstituteClient(httpRequestResultService);

try
{
    System.Console.WriteLine("Making API request to Art Institute of Chicago...");

    // Get first 10 artworks
    var query = new ApiQuery { Limit = 10 };
    var response = await client.GetArtworksAsync(query);

    System.Console.WriteLine($"API URL: {response.Config?.IiifUrl ?? "N/A"}");
    System.Console.WriteLine($"Total artworks retrieved: {response.Data?.Count ?? 0}");
    System.Console.WriteLine($"Response Config: {response.Config != null}");
    System.Console.WriteLine($"Response Data: {response.Data != null}");
    System.Console.WriteLine($"Response Info: {response.Info != null}");

    if (response.Data != null && response.Data.Count > 0)
    {
        foreach (var artwork in response.Data)
        {
            System.Console.WriteLine($"- {artwork.Title} (ID: {artwork.Id})");
        }
    }
    else
    {
        System.Console.WriteLine("No artwork data returned from API");

        // Try a simple test to see if the API is responding at all
        System.Console.WriteLine("\nTesting basic API connectivity...");
        var testQuery = new ApiQuery { Limit = 1 };
        var testResponse = await client.GetArtworksAsync(testQuery);
        System.Console.WriteLine($"Test response data count: {testResponse.Data?.Count ?? 0}");
        System.Console.WriteLine($"Test response info: {testResponse.Info?.ToString() ?? "null"}");
    }
}
catch (Exception ex)
{
    System.Console.WriteLine($"Error occurred: {ex.Message}");
    System.Console.WriteLine($"Exception type: {ex.GetType().Name}");
    if (ex.InnerException != null)
    {
        System.Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

System.Console.WriteLine("Press any key to exit...");
System.Console.ReadLine();



