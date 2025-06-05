using Microsoft.EntityFrameworkCore;
using WebSpark.ArtSpark.Demo.Data;
using WebSpark.ArtSpark.Demo.Models;
using WebSpark.ArtSpark.Demo.Services;
using WebSpark.ArtSpark.Demo.Utilities;

namespace WebSpark.ArtSpark.Demo.Tests;

/// <summary>
/// Test class to verify collection routing and slug functionality
/// </summary>
public class CollectionRoutingTest
{
    /// <summary>
    /// Test method to verify that collections have proper slugs and routing works
    /// </summary>
    public static async Task TestCollectionSlugsAsync()
    {
        var options = new DbContextOptionsBuilder<ArtSparkDbContext>()
            .UseSqlite("Data Source=app.db")
            .Options;

        using var context = new ArtSparkDbContext(options);

        // Check if there are any collections with empty slugs
        var collectionsWithEmptySlugs = await context.Collections
            .Where(c => string.IsNullOrEmpty(c.Slug))
            .CountAsync();

        Console.WriteLine($"Collections with empty slugs: {collectionsWithEmptySlugs}");

        // Check all collections and their slugs
        var allCollections = await context.Collections
            .Select(c => new { c.Id, c.Name, c.Slug })
            .ToListAsync();

        Console.WriteLine($"Total collections: {allCollections.Count}");

        foreach (var collection in allCollections)
        {
            Console.WriteLine($"Collection ID: {collection.Id}, Name: '{collection.Name}', Slug: '{collection.Slug}'");

            if (string.IsNullOrEmpty(collection.Slug))
            {
                Console.WriteLine($"  ERROR: Collection {collection.Id} has empty slug!");
            }
            else if (collection.Slug.Contains("/") || collection.Slug.Contains(" "))
            {
                Console.WriteLine($"  WARNING: Collection {collection.Id} has invalid slug characters!");
            }
            else
            {
                Console.WriteLine($"  OK: Collection {collection.Id} has valid slug");
            }
        }

        // Test the slug generation utility
        TestSlugGeneration();
    }

    private static void TestSlugGeneration()
    {
        Console.WriteLine("\n--- Testing Slug Generation ---");

        var testCases = new[]
        {
            "My Favorite Artworks",
            "Modern Art Collection #1",
            "Abstract & Contemporary",
            "Collection with Special Characters!@#$%",
            "   Spaces   and   More   Spaces   ",
            "Émile's Accented Collection",
            ""
        };

        foreach (var testCase in testCases)
        {
            var slug = SlugGenerator.GenerateSlug(testCase);
            Console.WriteLine($"Input: '{testCase}' -> Slug: '{slug}'");
        }
    }
}
