using ArtInstituteChicago.Client.Models.Common;
using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.DigitalScholarlyCalatogs;

/// <summary>
/// Represents a digital scholarly catalog publication
/// </summary>
public class Publication : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("site")]
    public string? Site { get; set; }

    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    [JsonPropertyName("generic_page_ids")]
    public List<string>? GenericPageIds { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}

/// <summary>
/// Represents a section within a digital scholarly catalog
/// </summary>
public class Section : BaseResource
{
    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }

    [JsonPropertyName("accession")]
    public string? Accession { get; set; }

    [JsonPropertyName("revision")]
    public string? Revision { get; set; }

    [JsonPropertyName("source_id")]
    public string? SourceId { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("publication_title")]
    public string? PublicationTitle { get; set; }

    [JsonPropertyName("publication_id")]
    public string? PublicationId { get; set; }

    [JsonPropertyName("artwork_ids")]
    public List<int>? ArtworkIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}
