using System.Text.Json.Serialization;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Base class for all API resources
/// </summary>
public abstract class BaseResource
{
    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("api_model")]
    public string? ApiModel { get; set; }

    [JsonPropertyName("api_link")]
    public string? ApiLink { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("source_updated_at")]
    public DateTime? SourceUpdatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
}
