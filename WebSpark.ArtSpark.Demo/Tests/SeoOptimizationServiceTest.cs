using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebSpark.ArtSpark.Agent.Extensions;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Demo.Services;

namespace WebSpark.ArtSpark.Demo.Tests;

/// <summary>
/// Simple test to verify SEO optimization service functionality
/// </summary>
public class SeoOptimizationServiceTest
{
    /// <summary>
    /// Tests collection optimization functionality
    /// </summary>
    public static async Task TestCollectionOptimization()
    {
        // Setup dependency injection
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Add logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Add ArtSpark Agent services (needed for Semantic Kernel)
                services.AddArtSparkAgent(context.Configuration, config =>
                {
                    // Use default configuration or override as needed
                    config.OpenAI.ModelId = "gpt-4o";
                    config.OpenAI.Temperature = 0.7;
                });

                // Add SEO optimization service
                services.AddScoped<ISeoOptimizationService, SeoOptimizationService>();
            })
            .Build();

        var seoService = host.Services.GetRequiredService<ISeoOptimizationService>();
        var logger = host.Services.GetRequiredService<ILogger<SeoOptimizationServiceTest>>();

        try
        {
            logger.LogInformation("Testing collection optimization...");

            var collectionDescription = "A curated collection of Renaissance paintings featuring works by Leonardo da Vinci, Michelangelo, and Raphael. This collection showcases the evolution of artistic techniques during the Italian Renaissance period, focusing on religious and mythological themes.";
            var userId = "test-user-123";

            var optimizedCollection = await seoService.OptimizeCollectionAsync(collectionDescription, userId);

            logger.LogInformation("Collection optimization completed successfully!");
            logger.LogInformation("Name: {Name}", optimizedCollection.Name);
            logger.LogInformation("Slug: {Slug}", optimizedCollection.Slug);
            logger.LogInformation("Meta Title: {MetaTitle}", optimizedCollection.MetaTitle);
            logger.LogInformation("Meta Description: {MetaDescription}", optimizedCollection.MetaDescription);
            logger.LogInformation("Tags: {Tags}", optimizedCollection.Tags);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during collection optimization test");
            throw;
        }
    }

    /// <summary>
    /// Tests artwork optimization functionality
    /// </summary>
    public static async Task TestArtworkOptimization()
    {
        // Setup dependency injection (same as above)
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                services.AddArtSparkAgent(context.Configuration, config =>
                {
                    config.OpenAI.ModelId = "gpt-4o";
                    config.OpenAI.Temperature = 0.7;
                });

                services.AddScoped<ISeoOptimizationService, SeoOptimizationService>();
            })
            .Build();

        var seoService = host.Services.GetRequiredService<ISeoOptimizationService>();
        var logger = host.Services.GetRequiredService<ILogger<SeoOptimizationServiceTest>>();

        try
        {
            logger.LogInformation("Testing artwork optimization...");

            var artworkData = new ArtworkData
            {
                Id = 123,
                Title = "The Starry Night",
                ArtistDisplay = "Vincent van Gogh",
                DateDisplay = "1889",
                Medium = "Oil on canvas",
                Dimensions = "73.7 cm × 92.1 cm (29 in × 36¼ in)",
                PlaceOfOrigin = "France",
                Description = "A masterpiece depicting swirling clouds and a bright crescent moon illuminating a sleeping village below.",
                CulturalContext = "Post-Impressionist painting",
                StyleTitle = "Post-Impressionism",
                Classification = "Painting"
            };

            var collectionId = 456;
            var optimizedArtwork = await seoService.OptimizeCollectionArtworkAsync(artworkData, collectionId);

            logger.LogInformation("Artwork optimization completed successfully!");
            logger.LogInformation("Custom Title: {CustomTitle}", optimizedArtwork.CustomTitle);
            logger.LogInformation("Slug: {Slug}", optimizedArtwork.Slug);
            logger.LogInformation("Meta Title: {MetaTitle}", optimizedArtwork.MetaTitle);
            logger.LogInformation("Meta Description: {MetaDescription}", optimizedArtwork.MetaDescription);
            logger.LogInformation("Curator Notes: {CuratorNotes}", optimizedArtwork.CuratorNotes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during artwork optimization test");
            throw;
        }
    }
}
