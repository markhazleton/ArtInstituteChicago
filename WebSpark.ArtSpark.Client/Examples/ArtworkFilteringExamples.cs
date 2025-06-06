// Examples of using the new Style and Medium filtering methods

using ArtInstituteChicago.Client.Clients;
using ArtInstituteChicago.Client.Models.Common;

// Example usage of the new convenience methods for filtering artworks

public class ArtworkFilteringExamples
{
    private readonly IArtInstituteClient _client;

    public ArtworkFilteringExamples(IArtInstituteClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Example: Get Impressionist artworks
    /// </summary>
    public async Task GetImpressionistArtworksExample()
    {
        // Get first 20 Impressionist artworks
        var impressionistArtworks = await _client.GetArtworksByStyleAsync(
            ArtStyle.Impressionism,
            limit: 20
        );

        Console.WriteLine($"Found {impressionistArtworks.Data?.Count() ?? 0} Impressionist artworks");

        foreach (var artwork in impressionistArtworks.Data ?? [])
        {
            Console.WriteLine($"- {artwork.Title} by {artwork.ArtistDisplay}");
            Console.WriteLine($"  Style: {artwork.StyleTitle}");
            Console.WriteLine($"  Medium: {artwork.MediumDisplay}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example: Get oil paintings
    /// </summary>
    public async Task GetOilPaintingsExample()
    {
        // Get oil on canvas artworks
        var oilPaintings = await _client.GetArtworksByMediumAsync(
            ArtMedium.OilOnCanvas,
            limit: 15
        );

        Console.WriteLine($"Found {oilPaintings.Data?.Count() ?? 0} oil on canvas artworks");

        foreach (var artwork in oilPaintings.Data ?? [])
        {
            Console.WriteLine($"- {artwork.Title}");
            Console.WriteLine($"  Artist: {artwork.ArtistDisplay}");
            Console.WriteLine($"  Medium: {artwork.MediumDisplay}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example: Get sculptures
    /// </summary>
    public async Task GetSculpturesExample()
    {
        // Get sculpture artworks
        var sculptures = await _client.GetArtworksByClassificationAsync(
            ArtworkClassification.Sculpture,
            limit: 10
        );

        Console.WriteLine($"Found {sculptures.Data?.Count() ?? 0} sculptures");

        foreach (var artwork in sculptures.Data ?? [])
        {
            Console.WriteLine($"- {artwork.Title}");
            Console.WriteLine($"  Artist: {artwork.ArtistDisplay}");
            Console.WriteLine($"  Classification: {artwork.ClassificationTitle}");
            Console.WriteLine($"  Medium: {artwork.MediumDisplay}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example: Get Abstract oil paintings
    /// </summary>
    public async Task GetAbstractOilPaintingsExample()
    {
        // Get Abstract artworks that are oil paintings
        var abstractOils = await _client.GetArtworksByStyleAndMediumAsync(
            ArtStyle.Abstract,
            ArtMedium.OilOnCanvas,
            limit: 10
        );

        Console.WriteLine($"Found {abstractOils.Data?.Count() ?? 0} Abstract oil paintings");

        foreach (var artwork in abstractOils.Data ?? [])
        {
            Console.WriteLine($"- {artwork.Title}");
            Console.WriteLine($"  Artist: {artwork.ArtistDisplay}");
            Console.WriteLine($"  Style: {artwork.StyleTitle}");
            Console.WriteLine($"  Medium: {artwork.MediumDisplay}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example: Get watercolor paintings with pagination
    /// </summary>
    public async Task GetWatercolorPaintingsWithPaginationExample()
    {
        const int pageSize = 5;
        int currentPage = 1;
        bool hasMore = true;

        Console.WriteLine("Watercolor artworks (paginated):");
        Console.WriteLine("================================");

        while (hasMore && currentPage <= 3) // Limit to first 3 pages for example
        {
            var watercolors = await _client.GetArtworksByMediumAsync(
                ArtMedium.Watercolor,
                limit: pageSize,
                page: currentPage
            );

            Console.WriteLine($"\nPage {currentPage}:");
            Console.WriteLine($"Found {watercolors.Data?.Count() ?? 0} watercolor artworks on this page");

            foreach (var artwork in watercolors.Data ?? [])
            {
                Console.WriteLine($"- {artwork.Title} by {artwork.ArtistDisplay}");
            }

            hasMore = (watercolors.Data?.Count() ?? 0) == pageSize;
            currentPage++;
        }
    }

    /// <summary>
    /// Example: Compare different art styles
    /// </summary>
    public async Task CompareArtStylesExample()
    {
        var styles = new[]
        {
            ArtStyle.Impressionism,
            ArtStyle.Cubism,
            ArtStyle.AbstractExpressionism
        };

        Console.WriteLine("Art Style Comparison:");
        Console.WriteLine("===================");

        foreach (var style in styles)
        {
            var artworks = await _client.GetArtworksByStyleAsync(style, limit: 5);
            var count = artworks.Data?.Count() ?? 0;

            Console.WriteLine($"{style.GetDescription()}: {count} artworks found");

            if (count > 0)
            {
                var firstArtwork = artworks.Data!.First();
                Console.WriteLine($"  Example: {firstArtwork.Title} by {firstArtwork.ArtistDisplay}");
            }
        }
    }

    /// <summary>
    /// Example: Search for artworks by artist name
    /// </summary>
    public async Task GetArtworksByArtistExample()
    {
        var artistNames = new[] { "Van Gogh", "Picasso", "Monet", "Renoir" };

        Console.WriteLine("Artworks by Famous Artists:");
        Console.WriteLine("===========================");

        foreach (var artistName in artistNames)
        {
            var artworks = await _client.GetArtworksByArtistAsync(artistName, limit: 5);
            var count = artworks.Data?.Count() ?? 0;

            Console.WriteLine($"\n{artistName}: {count} artworks found");

            if (count > 0)
            {
                foreach (var artwork in artworks.Data!.Take(3))
                {
                    Console.WriteLine($"  - {artwork.Title}");
                    Console.WriteLine($"    Artist: {artwork.ArtistDisplay}");
                    Console.WriteLine($"    Date: {artwork.DateDisplay}");
                    Console.WriteLine($"    Medium: {artwork.MediumDisplay}");
                    Console.WriteLine();
                }
            }
        }
    }

    /// <summary>
    /// Example: Search for artworks by artist with pagination
    /// </summary>
    public async Task GetArtworksByArtistWithPaginationExample()
    {
        const string artistName = "Picasso";
        const int pageSize = 10;
        int currentPage = 1;
        bool hasMore = true;

        Console.WriteLine($"Artworks by {artistName} (paginated):");
        Console.WriteLine("=====================================");

        while (hasMore && currentPage <= 3) // Limit to first 3 pages for example
        {
            var artworks = await _client.GetArtworksByArtistAsync(
                artistName,
                limit: pageSize,
                page: currentPage
            );

            Console.WriteLine($"\nPage {currentPage}:");
            Console.WriteLine($"Found {artworks.Data?.Count() ?? 0} artworks on this page");

            foreach (var artwork in artworks.Data ?? [])
            {
                Console.WriteLine($"- {artwork.Title} ({artwork.DateDisplay})");
            }

            hasMore = (artworks.Data?.Count() ?? 0) == pageSize;
            currentPage++;
        }
    }

    /// <summary>
    /// Example: Browse available enum values
    /// </summary>
    public void ShowAvailableFiltersExample()
    {
        Console.WriteLine("Available Art Styles:");
        Console.WriteLine("====================");
        foreach (ArtStyle style in Enum.GetValues<ArtStyle>())
        {
            Console.WriteLine($"- {style.GetDescription()}");
        }

        Console.WriteLine("\nAvailable Art Mediums:");
        Console.WriteLine("=====================");
        foreach (ArtMedium medium in Enum.GetValues<ArtMedium>())
        {
            Console.WriteLine($"- {medium.GetDescription()}");
        }

        Console.WriteLine("\nAvailable Classifications:");
        Console.WriteLine("=========================");
        foreach (ArtworkClassification classification in Enum.GetValues<ArtworkClassification>())
        {
            Console.WriteLine($"- {classification.GetDescription()}");
        }
    }
}
