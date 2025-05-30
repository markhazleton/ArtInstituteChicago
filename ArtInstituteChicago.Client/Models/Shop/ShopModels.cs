using ArtInstituteChicago.Client.Models.Common;
using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Shop;

/// <summary>
/// Represents a shop product
/// </summary>
public class Product : BaseResource
{
    [JsonPropertyName("external_sku")]
    public string? ExternalSku { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price_display")]
    public string? PriceDisplay { get; set; }

    [JsonPropertyName("min_compare_at_price")]
    public decimal? MinCompareAtPrice { get; set; }

    [JsonPropertyName("max_compare_at_price")]
    public decimal? MaxCompareAtPrice { get; set; }

    [JsonPropertyName("min_current_price")]
    public decimal? MinCurrentPrice { get; set; }

    [JsonPropertyName("max_current_price")]
    public decimal? MaxCurrentPrice { get; set; }

    [JsonPropertyName("artist_ids")]
    public List<int>? ArtistIds { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("exhibition_ids")]
    public List<int>? ExhibitionIds { get; set; }
}
