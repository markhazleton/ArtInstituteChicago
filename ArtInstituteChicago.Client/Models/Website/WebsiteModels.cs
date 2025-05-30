using ArtInstituteChicago.Client.Models.Common;
using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Website;

/// <summary>
/// Represents an event
/// </summary>
public class Event : BaseResource
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("header_description")]
    public string? HeaderDescription { get; set; }

    [JsonPropertyName("list_description")]
    public string? ListDescription { get; set; }

    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime? EndDate { get; set; }

    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    [JsonPropertyName("is_admission_required")]
    public bool? IsAdmissionRequired { get; set; }

    [JsonPropertyName("is_member_exclusive")]
    public bool? IsMemberExclusive { get; set; }

    [JsonPropertyName("is_sold_out")]
    public bool? IsSoldOut { get; set; }

    [JsonPropertyName("is_free")]
    public bool? IsFree { get; set; }

    [JsonPropertyName("is_private")]
    public bool? IsPrivate { get; set; }

    [JsonPropertyName("is_registration_required")]
    public bool? IsRegistrationRequired { get; set; }

    [JsonPropertyName("is_ticketed")]
    public bool? IsTicketed { get; set; }

    [JsonPropertyName("is_after_hours")]
    public bool? IsAfterHours { get; set; }

    [JsonPropertyName("is_virtual_event")]
    public bool? IsVirtualEvent { get; set; }

    [JsonPropertyName("event_type_id")]
    public int? EventTypeId { get; set; }

    [JsonPropertyName("alt_event_type_ids")]
    public List<int>? AltEventTypeIds { get; set; }

    [JsonPropertyName("audience_id")]
    public int? AudienceId { get; set; }

    [JsonPropertyName("alt_audience_ids")]
    public List<int>? AltAudienceIds { get; set; }

    [JsonPropertyName("program_ids")]
    public List<int>? ProgramIds { get; set; }

    [JsonPropertyName("program_titles")]
    public List<string>? ProgramTitles { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("artwork_titles")]
    public List<string>? ArtworkTitles { get; set; }

    [JsonPropertyName("gallery_id")]
    public int? GalleryId { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("hero_caption")]
    public string? HeroCaption { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("buy_button_text")]
    public string? BuyButtonText { get; set; }

    [JsonPropertyName("buy_button_caption")]
    public string? BuyButtonCaption { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("event_host")]
    public string? EventHost { get; set; }

    [JsonPropertyName("survey_url")]
    public string? SurveyUrl { get; set; }

    [JsonPropertyName("search_tags")]
    public List<string>? SearchTags { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an event occurrence (specific instance of an event)
/// </summary>
public class EventOccurrence : BaseResource
{
    [JsonPropertyName("start_at")]
    public DateTime? StartAt { get; set; }

    [JsonPropertyName("end_at")]
    public DateTime? EndAt { get; set; }

    [JsonPropertyName("event_id")]
    public int? EventId { get; set; }

    [JsonPropertyName("button_url")]
    public string? ButtonUrl { get; set; }

    [JsonPropertyName("button_text")]
    public string? ButtonText { get; set; }

    [JsonPropertyName("button_caption")]
    public string? ButtonCaption { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonPropertyName("is_private")]
    public bool? IsPrivate { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }
}

/// <summary>
/// Represents an event program
/// </summary>
public class EventProgram : BaseResource
{
    [JsonPropertyName("is_affiliate_group")]
    public bool? IsAffiliateGroup { get; set; }

    [JsonPropertyName("is_event_host")]
    public bool? IsEventHost { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }
}

/// <summary>
/// Represents an article
/// </summary>
public class Article : BaseResource
{
    [JsonPropertyName("copy")]
    public string? Copy { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("intro")]
    public string? Intro { get; set; }

    [JsonPropertyName("header_copy")]
    public string? HeaderCopy { get; set; }

    [JsonPropertyName("listing_description")]
    public string? ListingDescription { get; set; }

    [JsonPropertyName("short_copy")]
    public string? ShortCopy { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("category_title")]
    public string? CategoryTitle { get; set; }

    [JsonPropertyName("author_display")]
    public string? AuthorDisplay { get; set; }

    [JsonPropertyName("main_image_id")]
    public string? MainImageId { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("artwork_titles")]
    public List<string>? ArtworkTitles { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a highlight
/// </summary>
public class Highlight : BaseResource
{
    [JsonPropertyName("copy")]
    public string? Copy { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("is_featured")]
    public bool? IsFeatured { get; set; }

    [JsonPropertyName("weight")]
    public int? Weight { get; set; }

    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("category_title")]
    public string? CategoryTitle { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a static page
/// </summary>
public class StaticPage : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a generic page
/// </summary>
public class GenericPage : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("search_tags")]
    public List<string>? SearchTags { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a press release
/// </summary>
public class PressRelease : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("copy")]
    public string? Copy { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents an educator resource
/// </summary>
public class EducatorResource : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("copy")]
    public string? Copy { get; set; }

    [JsonPropertyName("resource_type")]
    public string? ResourceType { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a digital publication
/// </summary>
public class DigitalPublication : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("listing_description")]
    public string? ListingDescription { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a digital publication article/section
/// </summary>
public class DigitalPublicationArticle : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("copy")]
    public string? Copy { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("author_display")]
    public string? AuthorDisplay { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("publication_id")]
    public int? PublicationId { get; set; }

    [JsonPropertyName("publication_title")]
    public string? PublicationTitle { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a printed publication
/// </summary>
public class PrintedPublication : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("price_display")]
    public string? PriceDisplay { get; set; }

    [JsonPropertyName("publication_date")]
    public DateTime? PublicationDate { get; set; }

    [JsonPropertyName("isbn")]
    public string? Isbn { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}
