using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Common;

/// <summary>
/// Configuration information from API responses
/// </summary>
public class Config
{
    [JsonPropertyName("iiif_url")]
    public string? IiifUrl { get; set; }

    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }
}
