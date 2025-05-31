using System.Text.Json.Serialization;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Image thumbnail information
/// </summary>
public class Thumbnail
{
    [JsonPropertyName("lqip")]
    public string? Lqip { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("alt_text")]
    public string? AltText { get; set; }
}

/// <summary>
/// Color information
/// </summary>
public class Color
{
    [JsonPropertyName("h")]
    public int? H { get; set; }

    [JsonPropertyName("l")]
    public int? L { get; set; }

    [JsonPropertyName("s")]
    public int? S { get; set; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; set; }

    [JsonPropertyName("population")]
    public int? Population { get; set; }
}
