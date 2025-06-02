using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebSpark.ArtSpark.Agent.Extensions;
using WebSpark.ArtSpark.Agent.Interfaces;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;

namespace WebSpark.ArtSpark.Agent.Examples;

public class BasicUsageExample
{
    public static async Task RunExample()
    {
        // Create host builder
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Add ArtSpark Agent services
                services.AddArtSparkAgent(context.Configuration, config =>
                {
                    // Override default settings if needed
                    config.OpenAI.ModelId = "gpt-4o";
                    config.OpenAI.Temperature = 0.7;
                    config.Cache.Enabled = true;
                });
            })
            .Build();

        // Get the chat agent service
        var chatAgent = host.Services.GetRequiredService<IArtworkChatAgent>();
        var logger = host.Services.GetRequiredService<ILogger<BasicUsageExample>>();

        try
        {
            // Example 1: Chat with an artwork
            logger.LogInformation("Starting artwork chat example...");

            var chatRequest = new ChatRequest
            {
                ArtworkId = 111628, // Helmet Mask from your site
                Message = "Tell me about your cultural significance and what you represent",
                Persona = ChatPersona.Artwork,
                IncludeVisualAnalysis = true
            };

            var response = await chatAgent.ChatAsync(chatRequest);

            if (response.Success)
            {
                logger.LogInformation("Artwork Response: {Response}", response.Response);
                logger.LogInformation("Suggested Questions: {Questions}", string.Join(", ", response.SuggestedQuestions));
                logger.LogInformation("Tokens Used: {Tokens}, Response Time: {Time}ms",
                    response.Analytics.TokensUsed, response.Analytics.ResponseTime.TotalMilliseconds);
            }
            else
            {
                logger.LogError("Chat failed: {Error}", response.Error);
            }

            // Example 2: Chat with the artist
            var artistRequest = new ChatRequest
            {
                ArtworkId = 111628,
                Message = "What inspired you to create this mask? What techniques did you use?",
                Persona = ChatPersona.Artist,
                ConversationHistory = new List<ChatMessage>() // Start fresh conversation
            };

            var artistResponse = await chatAgent.ChatAsync(artistRequest);

            if (artistResponse.Success)
            {
                logger.LogInformation("Artist Response: {Response}", artistResponse.Response);
            }

            // Example 3: Generate conversation starters
            var dataProvider = host.Services.GetRequiredService<IArtworkDataProvider>();
            var artwork = await dataProvider.GetArtworkAsync(111628);

            if (artwork != null)
            {
                var starters = await chatAgent.GenerateConversationStartersAsync(artwork, ChatPersona.Curator);
                logger.LogInformation("Curator Conversation Starters: {Starters}", string.Join(", ", starters));
            }

            // Example 4: Visual analysis
            if (artwork != null)
            {
                var visualAnalysis = await chatAgent.AnalyzeArtworkVisuallyAsync(artwork,
                    "Describe the colors, patterns, and artistic techniques visible in this mask");
                logger.LogInformation("Visual Analysis: {Analysis}", visualAnalysis);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in example execution");
        }
    }
}

