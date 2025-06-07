using System;
using System.Linq;
using System.Threading.Tasks;
using WebSpark.ArtSpark.Client.Clients;
using WebSpark.HttpClientUtility.Services;

namespace WebSpark.ArtSpark.Console
{
    /// <summary>
    /// Test the artist search functionality
    /// </summary>
    public class ArtistSearchTest
    {
        public static async Task RunArtistSearchTests()
        {
            Console.WriteLine("Testing Artist Search Functionality");
            Console.WriteLine("==================================");

            // Initialize the client
            var httpClientService = new HttpClientService();
            var client = new ArtInstituteClient(httpClientService);

            // Test cases
            string[] testArtists = { "Van Gogh", "Picasso", "Monet", "Cezanne", "Renoir" };

            foreach (var artist in testArtists)
            {
                Console.WriteLine($"\nTesting search for artist: {artist}");
                Console.WriteLine(new string('-', 40));

                try
                {
                    var response = await client.GetArtworksByArtistAsync(artist, limit: 5);

                    if (response?.Data != null && response.Data.Any())
                    {
                        Console.WriteLine($"✓ Found {response.Data.Count()} artworks");
                        Console.WriteLine($"  Total available: {response.Pagination?.Total ?? 0}");

                        // Show first result
                        var firstArtwork = response.Data.First();
                        Console.WriteLine($"  First result: {firstArtwork.Title}");
                        Console.WriteLine($"  Artist: {firstArtwork.ArtistDisplay}");

                        // Test pagination with artist parameter
                        if (response.Pagination?.Total > 5)
                        {
                            Console.WriteLine("  Testing pagination...");
                            var page2Response = await client.GetArtworksByArtistAsync(artist, limit: 5, page: 2);
                            if (page2Response?.Data != null && page2Response.Data.Any())
                            {
                                Console.WriteLine($"  ✓ Page 2 found {page2Response.Data.Count()} artworks");
                            }
                            else
                            {
                                Console.WriteLine("  ✗ Page 2 failed");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✗ No artworks found for {artist}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error searching for {artist}: {ex.Message}");
                }

                await Task.Delay(500); // Rate limiting
            }

            Console.WriteLine("\n==================================");
            Console.WriteLine("Artist search tests completed!");
        }
    }
}
