using WebSpark.ArtSpark.Client.Clients;
using WebSpark.ArtSpark.Client.Models.Common;

namespace WebSpark.ArtSpark.Client.Examples;

/// <summary>
/// Test program to demonstrate the new artist search functionality
/// </summary>
public class TestArtistSearch
{
    public static async Task Main(string[] args)
    {
        var client = new ArtInstituteClient();

        try
        {
            Console.WriteLine("Testing Artist Search Functionality");
            Console.WriteLine("==================================");

            // Test 1: Search for Van Gogh
            Console.WriteLine("\n1. Testing GetArtworksByArtistAsync with 'Van Gogh':");
            var vanGoghWorks = await client.GetArtworksByArtistAsync("Van Gogh", limit: 5, page: 1);
            Console.WriteLine($"Found {vanGoghWorks.Data?.Count ?? 0} Van Gogh artworks on page 1");
            if (vanGoghWorks.Data?.Count > 0)
            {
                foreach (var artwork in vanGoghWorks.Data.Take(3))
                {
                    Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistDisplay}");
                }
            }

            // Test 2: Search for Monet with pagination
            Console.WriteLine("\n2. Testing GetArtworksByArtistAsync with 'Monet' - Page 1:");
            var monetPage1 = await client.GetArtworksByArtistAsync("Monet", limit: 3, page: 1);
            Console.WriteLine($"Found {monetPage1.Data?.Count ?? 0} Monet artworks on page 1");
            if (monetPage1.Data?.Count > 0)
            {
                foreach (var artwork in monetPage1.Data)
                {
                    Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistDisplay}");
                }
            }

            Console.WriteLine("\n3. Testing GetArtworksByArtistAsync with 'Monet' - Page 2:");
            var monetPage2 = await client.GetArtworksByArtistAsync("Monet", limit: 3, page: 2);
            Console.WriteLine($"Found {monetPage2.Data?.Count ?? 0} Monet artworks on page 2");
            if (monetPage2.Data?.Count > 0)
            {
                foreach (var artwork in monetPage2.Data)
                {
                    Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistDisplay}");
                }
            }

            // Test 3: Search for Picasso
            Console.WriteLine("\n4. Testing GetArtworksByArtistAsync with 'Picasso':");
            var picassoWorks = await client.GetArtworksByArtistAsync("Picasso", limit: 5);
            Console.WriteLine($"Found {picassoWorks.Data?.Count ?? 0} Picasso artworks");
            if (picassoWorks.Data?.Count > 0)
            {
                foreach (var artwork in picassoWorks.Data.Take(3))
                {
                    Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistDisplay}");
                }
            }

            // Test 4: Test error handling with empty artist name
            Console.WriteLine("\n5. Testing error handling with empty artist name:");
            try
            {
                var emptySearch = await client.GetArtworksByArtistAsync("", limit: 5);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ✓ Correctly caught ArgumentException: {ex.Message}");
            }

            Console.WriteLine("\n✅ Artist search functionality tests completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error during testing: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
