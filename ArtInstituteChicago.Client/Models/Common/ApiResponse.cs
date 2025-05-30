using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Common;

/// <summary>
/// Generic API response wrapper for list endpoints
/// </summary>
/// <typeparam name="T">The resource type</typeparam>
public class ApiResponse<T>
{
    [JsonPropertyName("pagination")]
    public Pagination? Pagination { get; set; }

    [JsonPropertyName("data")]
    public List<T>? Data { get; set; }

    [JsonPropertyName("info")]
    public Info? Info { get; set; }

    [JsonPropertyName("config")]
    public Config? Config { get; set; }
}

/// <summary>
/// API response wrapper for single resource endpoints
/// </summary>
/// <typeparam name="T">The resource type</typeparam>
public class SingleApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("info")]
    public Info? Info { get; set; }

    [JsonPropertyName("config")]
    public Config? Config { get; set; }
}
