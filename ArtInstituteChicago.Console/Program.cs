using ArtInstituteChicago.Client.Clients;
using ArtInstituteChicago.Client.Models.Common;
using System.Net.Http;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json; // Add this namespace

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
services.AddLogging();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

services.AddSingleton<IConfiguration>(configuration);

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Resolve dependencies
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient();
var httpRequestResultService = serviceProvider.GetRequiredService<IHttpRequestResultService>();

// Pass IHttpRequestResultService to ArtInstituteClient
var client = new ArtInstituteClient(httpClient, httpRequestResultService);

// Get first 10 artworks
var query = new ApiQuery { Limit = 10 };
var response = await client.GetArtworksAsync(query);

System.Console.WriteLine($"API URL: {response.Config?.IiifUrl ?? "N/A"}");
System.Console.WriteLine($"Total artworks retrieved: {response.Data?.Count ?? 0}");

if (response.Data != null)
{
    foreach (var artwork in response.Data)
    {
        System.Console.WriteLine($"- {artwork.Title} (ID: {artwork.Id})");
    }
}

System.Console.WriteLine("Press any key to exit...");
System.Console.ReadLine();



