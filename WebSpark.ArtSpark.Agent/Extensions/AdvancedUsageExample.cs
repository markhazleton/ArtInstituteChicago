using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebSpark.ArtSpark.Agent.Extensions;
using WebSpark.ArtSpark.Agent.Interfaces;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;

namespace WebSpark.ArtSpark.Agent.Examples;

public class AdvancedUsageExample
{
    public static async Task RunAdvancedExample()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddArtSparkAgent(context.Configuration, config =>
                {
                    // Advanced configuration
                    config.OpenAI.ModelId = "gpt-4o";
                    config.OpenAI.VisionModelId = "gpt-4o";
                    config.OpenAI.MaxTokens = 1500;
                    config.OpenAI.Temperature = 0.8;
                    config.Cache.Enabled = true;
                    config.Cache.ConversationExpiry = TimeSpan.FromHours(2);
                    config.Logging.EnableTelemetry = true;
                });
            })
            .Build();

        var chatAgent = host.Services.GetRequiredService<IArtworkChatAgent>();
        var dataProvider = host.Services.GetRequiredService<IArtworkDataProvider>();
        var logger = host.Services.GetRequiredService<ILogger<AdvancedUsageExample>>();

        try
        {
            // Example 1: Multi-turn conversation with memory
            logger.LogInformation("Starting multi-turn conversation example...");

            var artworkId = 111628;
            var conversationHistory = new List<ChatMessage>();

            // First interaction
            var request1 = new ChatRequest
            {
                ArtworkId = artworkId,
                Message = "Hello! I'd like to learn about you. What should I know?",
                Persona = ChatPersona.Artwork,
                ConversationHistory = conversationHistory,
                Settings = new ChatSettings
                {
                    MaxTokens = 800,
                    Temperature = 0.7,
                    EnableVision = true
                }
            };

            var response1 = await chatAgent.ChatAsync(request1);
            if (response1.Success)
            {
                conversationHistory.Add(new ChatMessage { Role = MessageRole.User, Content = request1.Message });
                conversationHistory.Add(new ChatMessage { Role = MessageRole.Assistant, Content = response1.Response });
                logger.LogInformation("First Response: {Response}", response1.Response);
            }

            // Follow-up interaction
            var request2 = new ChatRequest
            {
                ArtworkId = artworkId,
                Message = "That's fascinating! Can you tell me more about the specific patterns I can see on your surface?",
                Persona = ChatPersona.Artwork,
                ConversationHistory = conversationHistory,
                IncludeVisualAnalysis = true
            };

            var response2 = await chatAgent.ChatAsync(request2);
            if (response2.Success)
            {
                logger.LogInformation("Follow-up Response: {Response}", response2.Response);
                logger.LogInformation("Vision Used: {VisionUsed}", response2.Analytics.VisionUsed);
            }

            // Example 2: Persona comparison
            logger.LogInformation("Comparing different personas for the same artwork...");

            var question = "What is the significance of the geometric patterns?";
            var personas = new[] { ChatPersona.Artwork, ChatPersona.Artist, ChatPersona.Curator, ChatPersona.Historian };

            foreach (var persona in personas)
            {
                var request = new ChatRequest
                {
                    ArtworkId = artworkId,
                    Message = question,
                    Persona = persona,
                    IncludeVisualAnalysis = persona == ChatPersona.Artwork
                };

                var response = await chatAgent.ChatAsync(request);
                if (response.Success)
                {
                    logger.LogInformation("{Persona} perspective: {Response}",
                        persona, response.Response.Substring(0, Math.Min(200, response.Response.Length)) + "...");
                }
            }

            // Example 3: Batch artwork analysis
            logger.LogInformation("Running batch analysis on multiple artworks...");

            var featuredArtworks = await dataProvider.GetFeaturedArtworksAsync(5);
            var batchTasks = featuredArtworks.Select(async artwork =>
            {
                var visualAnalysis = await chatAgent.AnalyzeArtworkVisuallyAsync(artwork);
                var starters = await chatAgent.GenerateConversationStartersAsync(artwork, ChatPersona.Artwork);

                return new
                {
                    Artwork = artwork,
                    VisualAnalysis = visualAnalysis,
                    ConversationStarters = starters
                };
            });

            var batchResults = await Task.WhenAll(batchTasks);

            foreach (var result in batchResults)
            {
                logger.LogInformation("Artwork: {Title} - Starters: {Count}",
                    result.Artwork.Title, result.ConversationStarters.Count);
            }

            // Example 4: Search and chat workflow
            logger.LogInformation("Search and chat workflow example...");

            var searchResults = await dataProvider.SearchArtworksAsync("mask", 3);
            logger.LogInformation("Found {Count} artworks matching 'mask'", searchResults.Artworks.Count);

            foreach (var artwork in searchResults.Artworks.Take(2))
            {
                var chatRequest = new ChatRequest
                {
                    ArtworkId = artwork.Id,
                    Message = "In one sentence, what makes you unique compared to other masks?",
                    Persona = ChatPersona.Artwork
                };

                var response = await chatAgent.ChatAsync(chatRequest);
                if (response.Success)
                {
                    logger.LogInformation("{Title}: {Response}", artwork.Title, response.Response);
                }
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in advanced example execution");
        }
    }
}

