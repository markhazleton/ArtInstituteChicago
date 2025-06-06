using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Demo.Models;
using WebSpark.ArtSpark.Demo.Utilities;

namespace WebSpark.ArtSpark.Demo.Services;

/// <summary>
/// Service for SEO optimization of collections and artwork items using AI
/// </summary>
public class SeoOptimizationService : ISeoOptimizationService
{
    private readonly ILogger<SeoOptimizationService> _logger;
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    public SeoOptimizationService(
        ILogger<SeoOptimizationService> logger,
        Kernel kernel)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    /// <summary>
    /// Optimizes a collection for SEO based on a descriptive string
    /// </summary>
    public async Task<UserCollection> OptimizeCollectionAsync(
        string collectionDescription,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionDescription))
            throw new ArgumentException("Collection description cannot be null or empty", nameof(collectionDescription));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        try
        {
            _logger.LogInformation("Starting collection SEO optimization for user {UserId}", userId);

            var prompt = CreateCollectionOptimizationPrompt(collectionDescription);
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(prompt);

            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                cancellationToken: cancellationToken);

            var optimizedData = ParseCollectionResponse(response.Content);

            var collection = new UserCollection
            {
                UserId = userId,
                Name = optimizedData.Name,
                Description = optimizedData.Description,
                LongDescription = optimizedData.LongDescription,
                Slug = SlugGenerator.GenerateSlug(optimizedData.Name),
                MetaTitle = optimizedData.MetaTitle,
                MetaDescription = optimizedData.MetaDescription,
                MetaKeywords = optimizedData.MetaKeywords,
                Tags = optimizedData.Tags,
                CuratorNotes = optimizedData.CuratorNotes,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Successfully optimized collection with name: {CollectionName}", collection.Name);
            return collection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing collection for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Optimizes an artwork item for SEO within a collection context
    /// </summary>
    public async Task<CollectionArtwork> OptimizeCollectionArtworkAsync(
        ArtworkData artworkData,
        int collectionId,
        CancellationToken cancellationToken = default)
    {
        if (artworkData == null)
            throw new ArgumentNullException(nameof(artworkData));

        if (collectionId <= 0)
            throw new ArgumentException("Collection ID must be greater than 0", nameof(collectionId));

        try
        {
            _logger.LogInformation("Starting artwork SEO optimization for artwork {ArtworkId} in collection {CollectionId}",
                artworkData.Id, collectionId);

            var prompt = CreateArtworkOptimizationPrompt(artworkData);
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(prompt);

            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                cancellationToken: cancellationToken);

            var optimizedData = ParseArtworkResponse(response.Content);

            var collectionArtwork = new CollectionArtwork
            {
                CollectionId = collectionId,
                ArtworkId = artworkData.Id,
                Slug = SlugGenerator.GenerateSlug(optimizedData.CustomTitle ?? artworkData.Title),
                CustomTitle = optimizedData.CustomTitle,
                CustomDescription = optimizedData.CustomDescription,
                CuratorNotes = optimizedData.CuratorNotes,
                MetaTitle = optimizedData.MetaTitle,
                MetaDescription = optimizedData.MetaDescription,
                DisplayOrder = 0,
                AddedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Successfully optimized artwork {ArtworkId} for collection {CollectionId}",
                artworkData.Id, collectionId);
            return collectionArtwork;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing artwork {ArtworkId} for collection {CollectionId}",
                artworkData.Id, collectionId);
            throw;
        }
    }

    /// <summary>
    /// Creates the AI prompt for collection optimization
    /// </summary>
    private static string CreateCollectionOptimizationPrompt(string collectionDescription)
    {
        return $@"You are an expert SEO copywriter and art curator specializing in creating engaging, search-engine-optimized content for art collections.

Based on the following collection description, create comprehensive SEO-optimized content for an art collection:

Collection Description: {collectionDescription}

Please provide a JSON response with the following structure:
{{
    ""name"": ""collection name (max 100 chars)"",
    ""description"": ""brief description (max 500 chars)"",
    ""longDescription"": ""detailed description (1000-2000 chars)"",
    ""metaTitle"": ""SEO title (max 60 chars)"",
    ""metaDescription"": ""SEO meta description (max 160 chars)"",
    ""metaKeywords"": ""comma-separated keywords (max 255 chars)"",
    ""tags"": ""comma-separated tags for categorization"",
    ""curatorNotes"": ""professional curator insights and context""
}}

Guidelines:
- Create compelling, keyword-rich content that appeals to art enthusiasts
- Ensure meta titles and descriptions are optimized for search engines
- Use relevant art terminology and cultural context
- Make descriptions engaging and informative
- Include relevant keywords naturally in the content
- Ensure all character limits are respected
- Focus on making the collection discoverable and appealing

Respond only with valid JSON.";
    }

    /// <summary>
    /// Creates the AI prompt for artwork optimization
    /// </summary>
    private static string CreateArtworkOptimizationPrompt(ArtworkData artworkData)
    {
        return $@"You are an expert SEO copywriter and art curator specializing in creating engaging, search-engine-optimized content for an individual artwork.

Based on the following artwork information, create comprehensive SEO-optimized content:

Title: {artworkData.Title}
Artist: {artworkData.ArtistDisplay}
Date: {artworkData.DateDisplay}
Medium: {artworkData.Medium}
Dimensions: {artworkData.Dimensions}
Origin: {artworkData.PlaceOfOrigin}
Description: {artworkData.Description}
Cultural Context: {artworkData.CulturalContext}
Style: {artworkData.StyleTitle}
Classification: {artworkData.Classification}

Please provide a JSON response with the following structure:
{{
    ""customTitle"": ""enhanced title for collection context (optional, max 200 chars)"",
    ""customDescription"": ""engaging description for this artwork in collection context (max 1000 chars)"",
    ""curatorNotes"": ""professional insights about significance, technique, cultural significance, and its appearance in popular culture"",
    ""metaTitle"": ""SEO-optimized title (max 60 chars)"",
    ""metaDescription"": ""SEO meta description (max 160 chars)""
}}

Guidelines:
- Create a fun, compelling, keyword-rich description
- Provide professional curatorial insights
- Reference the artwork's historical and cultural significance and appearance in popular culture
- Ensure meta content is optimized for search engines
- Use relevant art terminology and historical context
- Make descriptions accessible to both art experts and general audiences
- Include information about technique, style, cultural significance, and its appearance in popular culture
- Ensure all character limits are respected
- Focus on making the artwork discoverable and appealing
- Ensure the descriptions are engaging and informative

Respond only with valid JSON.";
    }

    /// <summary>
    /// Parses the AI response for collection optimization
    /// </summary>
    private CollectionOptimizationData ParseCollectionResponse(string? response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new InvalidOperationException("Received empty response from AI service");

        try
        {
            // Clean the response to extract JSON
            var jsonContent = ExtractJsonFromResponse(response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<CollectionOptimizationData>(jsonContent, options);

            if (result == null)
                throw new InvalidOperationException("Failed to deserialize collection optimization response");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(result.Name))
                throw new InvalidOperationException("AI response missing required 'name' field");

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse collection optimization response: {Response}", response);
            throw new InvalidOperationException("Failed to parse AI response as JSON", ex);
        }
    }

    /// <summary>
    /// Parses the AI response for artwork optimization
    /// </summary>
    private ArtworkOptimizationData ParseArtworkResponse(string? response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new InvalidOperationException("Received empty response from AI service");

        try
        {
            // Clean the response to extract JSON
            var jsonContent = ExtractJsonFromResponse(response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ArtworkOptimizationData>(jsonContent, options);

            if (result == null)
                throw new InvalidOperationException("Failed to deserialize artwork optimization response");

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse artwork optimization response: {Response}", response);
            throw new InvalidOperationException("Failed to parse AI response as JSON", ex);
        }
    }

    /// <summary>
    /// Extracts JSON content from AI response, handling potential markdown formatting
    /// </summary>
    private static string ExtractJsonFromResponse(string response)
    {
        var trimmed = response.Trim();

        // Handle markdown code blocks
        if (trimmed.StartsWith("```json") && trimmed.EndsWith("```"))
        {
            var startIndex = trimmed.IndexOf('\n') + 1;
            var endIndex = trimmed.LastIndexOf("\n```");
            return trimmed.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (trimmed.StartsWith("```") && trimmed.EndsWith("```"))
        {
            var startIndex = trimmed.IndexOf('\n') + 1;
            var endIndex = trimmed.LastIndexOf("\n```");
            return trimmed.Substring(startIndex, endIndex - startIndex).Trim();
        }

        // Find JSON object boundaries
        var jsonStart = trimmed.IndexOf('{');
        var jsonEnd = trimmed.LastIndexOf('}');

        if (jsonStart >= 0 && jsonEnd > jsonStart)
        {
            return trimmed.Substring(jsonStart, jsonEnd - jsonStart + 1);
        }

        return trimmed;
    }

    /// <summary>
    /// Data structure for collection optimization results
    /// </summary>
    private class CollectionOptimizationData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string MetaKeywords { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string CuratorNotes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data structure for artwork optimization results
    /// </summary>
    private class ArtworkOptimizationData
    {
        public string? CustomTitle { get; set; }
        public string CustomDescription { get; set; } = string.Empty;
        public string CuratorNotes { get; set; } = string.Empty;
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
    }
}
