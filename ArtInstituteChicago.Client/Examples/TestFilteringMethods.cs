using ArtInstituteChicago.Client;
using ArtInstituteChicago.Client.Models.Common;
using System;
using System.Threading.Tasks;

namespace ArtInstituteChicago.Client.Examples
{
    /// <summary>
    /// Test program to demonstrate the new artwork filtering functionality
    /// </summary>
    public class TestFilteringMethods
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("ArtInstituteChicago.Client - Filtering Methods Test");
            Console.WriteLine("==================================================");

            var client = new ArtInstituteClient();

            try
            {
                // Test 1: Filter by Art Style
                Console.WriteLine("\n1. Testing GetArtworksByStyleAsync with Impressionism:");
                var impressionismArtworks = await client.GetArtworksByStyleAsync(ArtStyle.Impressionism, limit: 5);
                Console.WriteLine($"Found {impressionismArtworks.Data?.Count ?? 0} Impressionism artworks");
                if (impressionismArtworks.Data?.Count > 0)
                {
                    foreach (var artwork in impressionismArtworks.Data.Take(3))
                    {
                        Console.WriteLine($"  - {artwork.Title} ({artwork.DateDisplay})");
                    }
                }

                // Test 2: Filter by Medium
                Console.WriteLine("\n2. Testing GetArtworksByMediumAsync with Oil on Canvas:");
                var oilPaintings = await client.GetArtworksByMediumAsync(ArtMedium.OilOnCanvas, limit: 5);
                Console.WriteLine($"Found {oilPaintings.Data?.Count ?? 0} oil paintings");
                if (oilPaintings.Data?.Count > 0)
                {
                    foreach (var artwork in oilPaintings.Data.Take(3))
                    {
                        Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistTitle}");
                    }
                }

                // Test 3: Filter by Classification
                Console.WriteLine("\n3. Testing GetArtworksByClassificationAsync with Painting:");
                var paintings = await client.GetArtworksByClassificationAsync(ArtworkClassification.Painting, limit: 5);
                Console.WriteLine($"Found {paintings.Data?.Count ?? 0} paintings");
                if (paintings.Data?.Count > 0)
                {
                    foreach (var artwork in paintings.Data.Take(3))
                    {
                        Console.WriteLine($"  - {artwork.Title} ({artwork.MediumDisplay})");
                    }
                }

                // Test 4: Filter by Style and Medium combined
                Console.WriteLine("\n4. Testing GetArtworksByStyleAndMediumAsync with Modern Art + Sculpture:");
                var modernSculptures = await client.GetArtworksByStyleAndMediumAsync(
                    ArtStyle.ModernArt,
                    ArtMedium.Bronze,
                    limit: 5);
                Console.WriteLine($"Found {modernSculptures.Data?.Count ?? 0} modern bronze sculptures");
                if (modernSculptures.Data?.Count > 0)
                {
                    foreach (var artwork in modernSculptures.Data.Take(3))
                    {
                        Console.WriteLine($"  - {artwork.Title} by {artwork.ArtistTitle}");
                    }
                }

                Console.WriteLine("\n✅ All filtering methods executed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error testing filtering methods: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
