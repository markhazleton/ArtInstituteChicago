using ArtInstituteChicago.Client.Models.Common;
using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Mobile;

/// <summary>
/// Represents a mobile tour
/// </summary>
public class Tour : BaseResource
{
    [JsonPropertyName("weight")]
    public int? Weight { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("sound_ids")]
    public List<string>? SoundIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a mobile sound file
/// </summary>
public class MobileSound : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("transcript")]
    public string? Transcript { get; set; }
}
