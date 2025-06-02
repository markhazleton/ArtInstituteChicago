using WebSpark.ArtSpark.Agent.Models;

namespace WebSpark.ArtSpark.Agent.Utils;

/// <summary>
/// Utility class for analyzing and categorizing artworks
/// </summary>
public static class ArtworkAnalyzer
{
    /// <summary>
    /// Categorizes artwork by culture/region
    /// </summary>
    public static string GetCulturalCategory(ArtworkData artwork)
    {
        if (string.IsNullOrEmpty(artwork.PlaceOfOrigin))
            return "Unknown";

        var origin = artwork.PlaceOfOrigin.ToLowerInvariant();

        return origin switch
        {
            var s when s.Contains("africa") => "African",
            var s when s.Contains("sierra leone") || s.Contains("liberia") ||
                      s.Contains("guinea") || s.Contains("ivory coast") ||
                      s.Contains("ghana") => "West African",
            var s when s.Contains("congo") || s.Contains("cameroon") ||
                      s.Contains("gabon") => "Central African",
            var s when s.Contains("nigeria") || s.Contains("benin") => "Nigerian",
            var s when s.Contains("mali") || s.Contains("burkina faso") => "Sahel Region",
            var s when s.Contains("egypt") || s.Contains("sudan") => "Nile Valley",
            var s when s.Contains("china") || s.Contains("chinese") => "Chinese",
            var s when s.Contains("japan") || s.Contains("japanese") => "Japanese",
            var s when s.Contains("india") || s.Contains("indian") => "Indian",
            var s when s.Contains("europe") || s.Contains("france") ||
                      s.Contains("italy") || s.Contains("germany") => "European",
            var s when s.Contains("america") || s.Contains("united states") => "American",
            _ => "Other"
        };
    }

    /// <summary>
    /// Determines the primary art type/medium
    /// </summary>
    public static string GetArtType(ArtworkData artwork)
    {
        if (string.IsNullOrEmpty(artwork.Medium) && string.IsNullOrEmpty(artwork.Classification))
            return "Unknown";

        var medium = (artwork.Medium + " " + artwork.Classification).ToLowerInvariant();

        return medium switch
        {
            var s when s.Contains("mask") => "Mask",
            var s when s.Contains("sculpture") || s.Contains("carving") => "Sculpture",
            var s when s.Contains("painting") || s.Contains("paint") => "Painting",
            var s when s.Contains("textile") || s.Contains("fabric") || s.Contains("cloth") => "Textile",
            var s when s.Contains("ceramic") || s.Contains("pottery") => "Ceramics",
            var s when s.Contains("metal") || s.Contains("bronze") || s.Contains("iron") => "Metalwork",
            var s when s.Contains("wood") || s.Contains("wooden") => "Woodwork",
            var s when s.Contains("jewelry") || s.Contains("ornament") => "Jewelry",
            var s when s.Contains("vessel") || s.Contains("container") => "Vessel",
            var s when s.Contains("figure") || s.Contains("figurine") => "Figure",
            _ => "Mixed Media"
        };
    }

    /// <summary>
    /// Estimates the time period category
    /// </summary>
    public static string GetTimePeriod(ArtworkData artwork)
    {
        if (string.IsNullOrEmpty(artwork.DateDisplay))
            return "Unknown Period";

        var date = artwork.DateDisplay.ToLowerInvariant();

        return date switch
        {
            var s when s.Contains("ancient") || s.Contains("bc") => "Ancient",
            var s when s.Contains("medieval") || s.Contains("middle ages") => "Medieval",
            var s when s.Contains("15th") || s.Contains("16th") || s.Contains("17th") => "Early Modern",
            var s when s.Contains("18th") || s.Contains("19th") => "Modern",
            var s when s.Contains("20th") || s.Contains("1900") || s.Contains("1950") => "Contemporary",
            var s when s.Contains("21st") || s.Contains("2000") => "Current",
            _ => "Historical"
        };
    }

    /// <summary>
    /// Generates contextual keywords for artwork discovery
    /// </summary>
    public static List<string> GenerateDiscoveryKeywords(ArtworkData artwork)
    {
        var keywords = new List<string>();

        // Add cultural keywords
        keywords.Add(GetCulturalCategory(artwork));

        // Add type keywords
        keywords.Add(GetArtType(artwork));

        // Add temporal keywords
        keywords.Add(GetTimePeriod(artwork));

        // Add specific cultural terms
        if (!string.IsNullOrEmpty(artwork.CulturalContext))
        {
            keywords.Add(artwork.CulturalContext);
        }

        // Add material-based keywords
        if (!string.IsNullOrEmpty(artwork.Medium))
        {
            var materials = ExtractMaterials(artwork.Medium);
            keywords.AddRange(materials);
        }

        // Add function-based keywords
        var functions = InferFunction(artwork);
        keywords.AddRange(functions);

        return keywords.Distinct().Where(k => !string.IsNullOrEmpty(k)).ToList();
    }

    private static List<string> ExtractMaterials(string medium)
    {
        var materials = new List<string>();
        var mediumLower = medium.ToLowerInvariant();

        var materialKeywords = new Dictionary<string, string>
        {
            { "wood", "Wood" },
            { "metal", "Metal" },
            { "bronze", "Bronze" },
            { "iron", "Iron" },
            { "clay", "Clay" },
            { "ceramic", "Ceramic" },
            { "stone", "Stone" },
            { "ivory", "Ivory" },
            { "bone", "Bone" },
            { "textile", "Textile" },
            { "cotton", "Cotton" },
            { "silk", "Silk" },
            { "leather", "Leather" },
            { "paper", "Paper" },
            { "glass", "Glass" }
        };

        foreach (var keyword in materialKeywords)
        {
            if (mediumLower.Contains(keyword.Key))
            {
                materials.Add(keyword.Value);
            }
        }

        return materials;
    }

    private static List<string> InferFunction(ArtworkData artwork)
    {
        var functions = new List<string>();
        var context = (artwork.Title + " " + artwork.Description + " " + artwork.Medium).ToLowerInvariant();

        var functionKeywords = new Dictionary<string, string>
        {
            { "mask", "Ceremonial" },
            { "ritual", "Ritual" },
            { "ceremony", "Ceremonial" },
            { "spiritual", "Spiritual" },
            { "religious", "Religious" },
            { "decorative", "Decorative" },
            { "functional", "Functional" },
            { "vessel", "Utilitarian" },
            { "container", "Utilitarian" },
            { "ornament", "Ornamental" },
            { "jewelry", "Personal Adornment" },
            { "sculpture", "Artistic" },
            { "portrait", "Commemorative" }
        };

        foreach (var keyword in functionKeywords)
        {
            if (context.Contains(keyword.Key))
            {
                functions.Add(keyword.Value);
            }
        }

        if (!functions.Any())
        {
            functions.Add("Cultural Object");
        }

        return functions;
    }
}

