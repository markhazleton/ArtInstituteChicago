using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Client.Models.Common;
using WebSpark.ArtSpark.Client.Models.DigitalScholarlyCalatogs;
using WebSpark.ArtSpark.Client.Models.Mobile;
using WebSpark.ArtSpark.Client.Models.Shop;
using WebSpark.ArtSpark.Client.Models.StaticArchive;
using WebSpark.ArtSpark.Client.Models.Website;

namespace WebSpark.ArtSpark.Client.Interfaces;

/// <summary>
/// Interface for the Art Institute of Chicago API client
/// Provides access to all API endpoints including Collections, Shop, Mobile, Digital Scholarly Catalogs, Static Archive, and Website resources
/// </summary>
public interface IArtInstituteClient
{
    #region Collections

    // Artworks
    Task<ApiResponse<ArtWork>> GetArtworksAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<ArtWork?> GetArtworkAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<ArtWork>> SearchArtworksAsync(SearchQuery query, CancellationToken cancellationToken = default);
    Task<string> GetArtworkManifestAsync(int id, CancellationToken cancellationToken = default);

    // Artwork filtering by style, medium, and classification
    Task<SearchResponse<ArtWork>> GetArtworksByStyleAsync(ArtStyle style, int? limit = null, int? page = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<ArtWork>> GetArtworksByMediumAsync(ArtMedium medium, int? limit = null, int? page = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<ArtWork>> GetArtworksByClassificationAsync(ArtworkClassification classification, int? limit = null, int? page = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<ArtWork>> GetArtworksByStyleAndMediumAsync(ArtStyle style, ArtMedium medium, int? limit = null, int? page = null, CancellationToken cancellationToken = default);

    // Agents (Artists, People, Organizations)
    Task<ApiResponse<Agent>> GetAgentsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Agent?> GetAgentAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Agent>> SearchAgentsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Places
    Task<ApiResponse<Place>> GetPlacesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Place?> GetPlaceAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Place>> SearchPlacesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Galleries
    Task<ApiResponse<Gallery>> GetGalleriesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Gallery?> GetGalleryAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Gallery>> SearchGalleriesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Exhibitions
    Task<ApiResponse<Exhibition>> GetExhibitionsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Exhibition?> GetExhibitionAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Exhibition>> SearchExhibitionsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Agent Types
    Task<ApiResponse<AgentType>> GetAgentTypesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<AgentType?> GetAgentTypeAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);

    // Agent Roles
    Task<ApiResponse<AgentRole>> GetAgentRolesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<AgentRole?> GetAgentRoleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);

    // Artwork Place Qualifiers
    Task<ApiResponse<ArtworkPlaceQualifier>> GetArtworkPlaceQualifiersAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<ArtworkPlaceQualifier?> GetArtworkPlaceQualifierAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);

    // Artwork Date Qualifiers
    Task<ApiResponse<ArtworkDateQualifier>> GetArtworkDateQualifiersAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<ArtworkDateQualifier?> GetArtworkDateQualifierAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);

    // Artwork Types
    Task<ApiResponse<ArtworkType>> GetArtworkTypesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<ArtworkType?> GetArtworkTypeAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);

    // Category Terms
    Task<ApiResponse<CategoryTerm>> GetCategoryTermsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<CategoryTerm?> GetCategoryTermAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<CategoryTerm>> SearchCategoryTermsAsync(SearchQuery query, CancellationToken cancellationToken = default);    // Images
    Task<ApiResponse<Image>> GetImagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Image?> GetImageAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Image>> SearchImagesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Videos
    Task<ApiResponse<Video>> GetVideosAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Video?> GetVideoAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Video>> SearchVideosAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Sounds
    Task<ApiResponse<Sound>> GetSoundsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Sound?> GetSoundAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Sound>> SearchSoundsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Texts
    Task<ApiResponse<Text>> GetTextsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Text?> GetTextAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Text>> SearchTextsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Shop

    // Products
    Task<ApiResponse<Product>> GetProductsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Product>> SearchProductsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Mobile

    // Tours
    Task<ApiResponse<Tour>> GetToursAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Tour?> GetTourAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Tour>> SearchToursAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Mobile Sounds
    Task<ApiResponse<MobileSound>> GetMobileSoundsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<MobileSound?> GetMobileSoundAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<MobileSound>> SearchMobileSoundsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Digital Scholarly Catalogs

    // Publications
    Task<ApiResponse<Publication>> GetPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Publication?> GetPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Publication>> SearchPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Sections
    Task<ApiResponse<Section>> GetSectionsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Section?> GetSectionAsync(long id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Section>> SearchSectionsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Static Archive

    // Sites
    Task<ApiResponse<Site>> GetSitesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Site?> GetSiteAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Site>> SearchSitesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Website

    // Events
    Task<ApiResponse<Event>> GetEventsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Event?> GetEventAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Event>> SearchEventsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Event Occurrences
    Task<ApiResponse<EventOccurrence>> GetEventOccurrencesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<EventOccurrence?> GetEventOccurrenceAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<EventOccurrence>> SearchEventOccurrencesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Event Programs
    Task<ApiResponse<EventProgram>> GetEventProgramsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<EventProgram?> GetEventProgramAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<EventProgram>> SearchEventProgramsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Articles
    Task<ApiResponse<Article>> GetArticlesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Article?> GetArticleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Article>> SearchArticlesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Highlights
    Task<ApiResponse<Highlight>> GetHighlightsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<Highlight?> GetHighlightAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<Highlight>> SearchHighlightsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Static Pages
    Task<ApiResponse<StaticPage>> GetStaticPagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<StaticPage?> GetStaticPageAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<StaticPage>> SearchStaticPagesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Generic Pages
    Task<ApiResponse<GenericPage>> GetGenericPagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<GenericPage?> GetGenericPageAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<GenericPage>> SearchGenericPagesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Press Releases
    Task<ApiResponse<PressRelease>> GetPressReleasesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<PressRelease?> GetPressReleaseAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<PressRelease>> SearchPressReleasesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Educator Resources
    Task<ApiResponse<EducatorResource>> GetEducatorResourcesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<EducatorResource?> GetEducatorResourceAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<EducatorResource>> SearchEducatorResourcesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Digital Publications
    Task<ApiResponse<DigitalPublication>> GetDigitalPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<DigitalPublication?> GetDigitalPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<DigitalPublication>> SearchDigitalPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Digital Publication Articles
    Task<ApiResponse<DigitalPublicationArticle>> GetDigitalPublicationArticlesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<DigitalPublicationArticle?> GetDigitalPublicationArticleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<DigitalPublicationArticle>> SearchDigitalPublicationArticlesAsync(SearchQuery query, CancellationToken cancellationToken = default);

    // Printed Publications
    Task<ApiResponse<PrintedPublication>> GetPrintedPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default);
    Task<PrintedPublication?> GetPrintedPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default);
    Task<SearchResponse<PrintedPublication>> SearchPrintedPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default);

    #endregion

    #region Utility Methods

    /// <summary>
    /// Constructs IIIF Image URL for a given image ID
    /// </summary>
    /// <param name="imageId">The image identifier</param>
    /// <param name="size">Size parameter (default: 843,)</param>
    /// <param name="format">Image format (default: jpg)</param>
    /// <returns>Complete IIIF URL for the image</returns>
    string BuildIiifUrl(string imageId, string size = "843,", string format = "jpg");

    /// <summary>
    /// Gets multiple resources by IDs in a single request
    /// </summary>
    /// <typeparam name="T">Resource type</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="ids">Array of resource IDs</param>
    /// <param name="fields">Fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response with multiple resources</returns>
    Task<ApiResponse<T>> GetResourcesByIdsAsync<T>(string endpoint, object[] ids, string[]? fields = null, CancellationToken cancellationToken = default);

    #endregion
}
