using ArtInstituteChicago.Client.Models.Common;
using System.Text.Json.Serialization;

namespace ArtInstituteChicago.Client.Models.Collections;

/// <summary>
/// Represents a work of art in the collections
/// </summary>
public class ArtWork : BaseResource
{
    [JsonPropertyName("alt_titles")]
    public List<string>? AltTitles { get; set; }

    [JsonPropertyName("thumbnail")]
    public Thumbnail? Thumbnail { get; set; }

    [JsonPropertyName("main_reference_number")]
    public string? MainReferenceNumber { get; set; }

    [JsonPropertyName("has_not_been_viewed_much")]
    public bool? HasNotBeenViewedMuch { get; set; }

    [JsonPropertyName("boost_rank")]
    public double? BoostRank { get; set; }

    [JsonPropertyName("date_start")]
    public int? DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public int? DateEnd { get; set; }

    [JsonPropertyName("date_display")]
    public string? DateDisplay { get; set; }

    [JsonPropertyName("date_qualifier_title")]
    public string? DateQualifierTitle { get; set; }

    [JsonPropertyName("date_qualifier_id")]
    public int? DateQualifierId { get; set; }

    [JsonPropertyName("artist_display")]
    public string? ArtistDisplay { get; set; }

    [JsonPropertyName("place_of_origin")]
    public string? PlaceOfOrigin { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("dimensions")]
    public string? Dimensions { get; set; }

    [JsonPropertyName("dimensions_detail")]
    public object? DimensionsDetail { get; set; }

    [JsonPropertyName("medium_display")]
    public string? MediumDisplay { get; set; }

    [JsonPropertyName("inscriptions")]
    public string? Inscriptions { get; set; }

    [JsonPropertyName("credit_line")]
    public string? CreditLine { get; set; }

    [JsonPropertyName("catalogue_display")]
    public string? CatalogueDisplay { get; set; }

    [JsonPropertyName("publication_history")]
    public string? PublicationHistory { get; set; }

    [JsonPropertyName("exhibition_history")]
    public string? ExhibitionHistory { get; set; }

    [JsonPropertyName("provenance_text")]
    public string? ProvenanceText { get; set; }

    [JsonPropertyName("edition")]
    public string? Edition { get; set; }

    [JsonPropertyName("publishing_verification_level")]
    public string? PublishingVerificationLevel { get; set; }

    [JsonPropertyName("internal_department_id")]
    public int? InternalDepartmentId { get; set; }

    [JsonPropertyName("fiscal_year")]
    public int? FiscalYear { get; set; }

    [JsonPropertyName("fiscal_year_deaccession")]
    public int? FiscalYearDeaccession { get; set; }

    [JsonPropertyName("is_public_domain")]
    public bool? IsPublicDomain { get; set; }

    [JsonPropertyName("is_zoomable")]
    public bool? IsZoomable { get; set; }

    [JsonPropertyName("max_zoom_window_size")]
    public int? MaxZoomWindowSize { get; set; }

    [JsonPropertyName("copyright_notice")]
    public string? CopyrightNotice { get; set; }

    [JsonPropertyName("has_multimedia_resources")]
    public bool? HasMultimediaResources { get; set; }

    [JsonPropertyName("has_educational_resources")]
    public bool? HasEducationalResources { get; set; }

    [JsonPropertyName("has_advanced_imaging")]
    public bool? HasAdvancedImaging { get; set; }

    [JsonPropertyName("colorfulness")]
    public double? Colorfulness { get; set; }

    [JsonPropertyName("color")]
    public Color? Color { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("latlon")]
    public string? Latlon { get; set; }

    [JsonPropertyName("is_on_view")]
    public bool? IsOnView { get; set; }

    [JsonPropertyName("on_loan_display")]
    public string? OnLoanDisplay { get; set; }

    [JsonPropertyName("gallery_title")]
    public string? GalleryTitle { get; set; }

    [JsonPropertyName("gallery_id")]
    public int? GalleryId { get; set; }

    [JsonPropertyName("nomisma_id")]
    public string? NomismaId { get; set; }

    [JsonPropertyName("artwork_type_title")]
    public string? ArtworkTypeTitle { get; set; }

    [JsonPropertyName("artwork_type_id")]
    public int? ArtworkTypeId { get; set; }

    [JsonPropertyName("department_title")]
    public string? DepartmentTitle { get; set; }
    [JsonPropertyName("department_id")]
    public string? DepartmentId { get; set; }

    [JsonPropertyName("artist_id")]
    public int? ArtistId { get; set; }

    [JsonPropertyName("artist_title")]
    public string? ArtistTitle { get; set; }

    [JsonPropertyName("alt_artist_ids")]
    public List<int>? AltArtistIds { get; set; }

    [JsonPropertyName("artist_ids")]
    public List<int>? ArtistIds { get; set; }

    [JsonPropertyName("artist_titles")]
    public List<string>? ArtistTitles { get; set; }

    [JsonPropertyName("category_ids")]
    public List<string>? CategoryIds { get; set; }

    [JsonPropertyName("category_titles")]
    public List<string>? CategoryTitles { get; set; }

    [JsonPropertyName("term_titles")]
    public List<string>? TermTitles { get; set; }

    [JsonPropertyName("style_id")]
    public string? StyleId { get; set; }

    [JsonPropertyName("style_title")]
    public string? StyleTitle { get; set; }

    [JsonPropertyName("alt_style_ids")]
    public List<string>? AltStyleIds { get; set; }

    [JsonPropertyName("style_ids")]
    public List<string>? StyleIds { get; set; }

    [JsonPropertyName("style_titles")]
    public List<string>? StyleTitles { get; set; }

    [JsonPropertyName("classification_id")]
    public string? ClassificationId { get; set; }

    [JsonPropertyName("classification_title")]
    public string? ClassificationTitle { get; set; }

    [JsonPropertyName("alt_classification_ids")]
    public List<string>? AltClassificationIds { get; set; }

    [JsonPropertyName("classification_ids")]
    public List<string>? ClassificationIds { get; set; }

    [JsonPropertyName("classification_titles")]
    public List<string>? ClassificationTitles { get; set; }

    [JsonPropertyName("subject_id")]
    public string? SubjectId { get; set; }

    [JsonPropertyName("alt_subject_ids")]
    public List<string>? AltSubjectIds { get; set; }

    [JsonPropertyName("subject_ids")]
    public List<string>? SubjectIds { get; set; }

    [JsonPropertyName("subject_titles")]
    public List<string>? SubjectTitles { get; set; }

    [JsonPropertyName("material_id")]
    public string? MaterialId { get; set; }

    [JsonPropertyName("alt_material_ids")]
    public List<string>? AltMaterialIds { get; set; }

    [JsonPropertyName("material_ids")]
    public List<string>? MaterialIds { get; set; }

    [JsonPropertyName("material_titles")]
    public List<string>? MaterialTitles { get; set; }

    [JsonPropertyName("technique_id")]
    public string? TechniqueId { get; set; }

    [JsonPropertyName("alt_technique_ids")]
    public List<string>? AltTechniqueIds { get; set; }

    [JsonPropertyName("technique_ids")]
    public List<string>? TechniqueIds { get; set; }

    [JsonPropertyName("technique_titles")]
    public List<string>? TechniqueTitles { get; set; }

    [JsonPropertyName("theme_titles")]
    public List<string>? ThemeTitles { get; set; }

    [JsonPropertyName("image_id")]
    public string? ImageId { get; set; }

    [JsonPropertyName("alt_image_ids")]
    public List<string>? AltImageIds { get; set; }

    [JsonPropertyName("document_ids")]
    public List<string>? DocumentIds { get; set; }

    [JsonPropertyName("sound_ids")]
    public List<string>? SoundIds { get; set; }

    [JsonPropertyName("video_ids")]
    public List<string>? VideoIds { get; set; }

    [JsonPropertyName("text_ids")]
    public List<string>? TextIds { get; set; }

    [JsonPropertyName("section_ids")]
    public List<int>? SectionIds { get; set; }

    [JsonPropertyName("section_titles")]
    public List<string>? SectionTitles { get; set; }

    [JsonPropertyName("site_ids")]
    public List<int>? SiteIds { get; set; }

    [JsonPropertyName("suggest_autocomplete_boosted")]
    public object? SuggestAutocompleteBoosted { get; set; }

    [JsonPropertyName("suggest_autocomplete_all")]
    public object? SuggestAutocompleteAll { get; set; }
}
