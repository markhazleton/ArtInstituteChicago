using System.Text.Json.Serialization;
using WebSpark.ArtSpark.Client.Models.Common;

namespace WebSpark.ArtSpark.Client.Models.Collections;

/// <summary>
/// Represents a person or organization (artist, etc.)
/// </summary>
public class Agent : BaseResource
{
    [JsonPropertyName("sort_title")]
    public string? SortTitle { get; set; }

    [JsonPropertyName("alt_titles")]
    public List<string>? AltTitles { get; set; }

    [JsonPropertyName("is_artist")]
    public bool? IsArtist { get; set; }

    [JsonPropertyName("birth_date")]
    public int? BirthDate { get; set; }

    [JsonPropertyName("death_date")]
    public int? DeathDate { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("ulan_id")]
    public int? UlanId { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a place
/// </summary>
public class Place : BaseResource
{
    [JsonPropertyName("tgn_id")]
    public int? TgnId { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a gallery
/// </summary>
public class Gallery : BaseResource
{
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("tgn_id")]
    public int? TgnId { get; set; }

    [JsonPropertyName("is_closed")]
    public bool? IsClosed { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    [JsonPropertyName("latlon")]
    public string? Latlon { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an exhibition
/// </summary>
public class Exhibition : BaseResource
{
    [JsonPropertyName("is_featured")]
    public bool? IsFeatured { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("aic_start_at")]
    public DateTime? AicStartAt { get; set; }

    [JsonPropertyName("aic_end_at")]
    public DateTime? AicEndAt { get; set; }

    [JsonPropertyName("gallery_id")]
    public int? GalleryId { get; set; }

    [JsonPropertyName("gallery_title")]
    public string? GalleryTitle { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("artwork_titles")]
    public List<string>? ArtworkTitles { get; set; }

    [JsonPropertyName("artist_ids")]
    public List<int>? ArtistIds { get; set; }

    [JsonPropertyName("site_ids")]
    public List<int>? SiteIds { get; set; }

    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }

    [JsonPropertyName("alt_image_ids")]
    public List<string>? AltImageIds { get; set; }

    [JsonPropertyName("document_ids")]
    public List<string>? DocumentIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an agent type (classification of agents)
/// </summary>
public class AgentType : BaseResource
{
    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an agent role (role an agent plays in relation to an artwork)
/// </summary>
public class AgentRole : BaseResource
{
    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a qualifier for artwork places
/// </summary>
public class ArtworkPlaceQualifier : BaseResource
{
    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a qualifier for artwork dates
/// </summary>
public class ArtworkDateQualifier : BaseResource
{
    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an artwork type
/// </summary>
public class ArtworkType : BaseResource
{
    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a category term
/// </summary>
public class CategoryTerm : BaseResource
{
    [JsonPropertyName("subtype")]
    public string? Subtype { get; set; }

    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an image asset
/// </summary>
public class Image : BaseResource
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("alt_text")]
    public string? AltText { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("iiif_url")]
    public string? IiifUrl { get; set; }
}

/// <summary>
/// Represents a video asset
/// </summary>
public class Video : BaseResource
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("alt_text")]
    public string? AltText { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("is_multimedia")]
    public bool? IsMultimedia { get; set; }

    [JsonPropertyName("is_educational_resource")]
    public bool? IsEducationalResource { get; set; }

    [JsonPropertyName("is_teacher_resource")]
    public bool? IsTeacherResource { get; set; }
}

/// <summary>
/// Represents a sound asset
/// </summary>
public class Sound : BaseResource
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("alt_text")]
    public string? AltText { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("is_multimedia")]
    public bool? IsMultimedia { get; set; }

    [JsonPropertyName("is_educational_resource")]
    public bool? IsEducationalResource { get; set; }

    [JsonPropertyName("is_teacher_resource")]
    public bool? IsTeacherResource { get; set; }
}

/// <summary>
/// Represents a text asset
/// </summary>
public class Text : BaseResource
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("is_multimedia")]
    public bool? IsMultimedia { get; set; }

    [JsonPropertyName("is_educational_resource")]
    public bool? IsEducationalResource { get; set; }

    [JsonPropertyName("is_teacher_resource")]
    public bool? IsTeacherResource { get; set; }
}
