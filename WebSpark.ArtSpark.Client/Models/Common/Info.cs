using System.Text.Json.Serialization;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Information about API response
/// </summary>
public class Info
{
    [JsonPropertyName("license_text")]
    public string? LicenseText { get; set; }

    [JsonPropertyName("license_links")]
    public List<string>? LicenseLinks { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
