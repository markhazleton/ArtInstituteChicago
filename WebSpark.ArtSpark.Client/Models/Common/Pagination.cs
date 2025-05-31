using System.Text.Json.Serialization;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Pagination information for API responses
/// </summary>
public class Pagination
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("prev_url")]
    public string? PrevUrl { get; set; }

    [JsonPropertyName("next_url")]
    public string? NextUrl { get; set; }
}
