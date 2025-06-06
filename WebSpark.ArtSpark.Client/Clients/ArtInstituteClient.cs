﻿using System.Text.Json;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Client.Models.Common;
using WebSpark.ArtSpark.Client.Models.DigitalScholarlyCalatogs;
using WebSpark.ArtSpark.Client.Models.Mobile;
using WebSpark.ArtSpark.Client.Models.Shop;
using WebSpark.ArtSpark.Client.Models.StaticArchive;
using WebSpark.ArtSpark.Client.Models.Website;
using WebSpark.HttpClientUtility.RequestResult;
using static WebSpark.ArtSpark.Client.Models.Common.EnumExtensions;

namespace WebSpark.ArtSpark.Client.Clients;

/// <summary>
/// Complete implementation of the Art Institute of Chicago API client
/// Provides access to all Collections, Shop, Mobile, Digital Scholarly Catalogs, Static Archive, and Website endpoints
/// </summary>
public class ArtInstituteClient : IArtInstituteClient
{
    private readonly IHttpRequestResultService _service;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string BaseUrl = "https://api.artic.edu/api/v1";
    private const string IiifBaseUrl = "https://www.artic.edu/iiif/2";

    public ArtInstituteClient(IHttpRequestResultService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

    }

    #region Private Helper Methods

    // Generic methods to handle API requests and responses IHttpRequestResultService service instance
    // These methods handle the common logic for making requests and parsing responses for different resource types
    private async Task<ApiResponse<T>> GetResourcesAsync<T>(
        string endpoint,
        ApiQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = BuildUrl(endpoint, query);
            var artDetailsRequest = new HttpRequestResult<ApiResponse<T>> { RequestPath = url, CacheDurationMinutes = 60, Retries = 1 };
            artDetailsRequest = await _service.HttpSendRequestResultAsync(artDetailsRequest, ct: cancellationToken).ConfigureAwait(false);
            return artDetailsRequest.ResponseResults ?? new ApiResponse<T> { Data = [] };
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException("Request error occurred", httpEx);
        }
        catch (JsonException jsonEx)
        {
            throw new JsonException("JSON parsing error occurred", jsonEx);
        }
        catch (Exception)
        {
            throw;
        }
    }


    private async Task<T?> GetResourceAsync<T>(
        string endpoint,
        object id,
        string[]? fields = null,
        string[]? include = null,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var query = new ApiQuery();
            if (fields?.Length > 0) query.Fields = string.Join(",", fields);
            if (include?.Length > 0) query.Include = string.Join(",", include);
            var url = BuildUrl($"{endpoint}/{id}", query);

            var artDetailsRequest = new HttpRequestResult<SingleApiResponse<T>> { RequestPath = url, CacheDurationMinutes = 60, Retries = 1 };
            artDetailsRequest = await _service.HttpSendRequestResultAsync(artDetailsRequest, ct: cancellationToken).ConfigureAwait(false);
            return artDetailsRequest.ResponseResults?.Data;
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException("Request error occurred", httpEx);
        }
        catch (JsonException jsonEx)
        {
            throw new JsonException("JSON parsing error occurred", jsonEx);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<SearchResponse<T>> SearchResourcesAsync<T>(
        string endpoint,
        SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = BuildSearchUrl(endpoint, query);
            var searchRequest = new HttpRequestResult<SearchResponse<T>> { RequestPath = url, CacheDurationMinutes = 60, Retries = 1 };
            searchRequest = await _service.HttpSendRequestResultAsync(searchRequest, ct: cancellationToken).ConfigureAwait(false);
            return searchRequest.ResponseResults ?? new SearchResponse<T> { Data = new List<T>() };
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException("Request error occurred", httpEx);
        }
        catch (JsonException jsonEx)
        {
            throw new JsonException("JSON parsing error occurred", jsonEx);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private string BuildUrl(string endpoint, ApiQuery? query = null)
    {
        var url = $"{BaseUrl}/{endpoint}";

        if (query == null) return url;

        var queryParams = new List<string>();

        if (query.Page.HasValue) queryParams.Add($"page={query.Page}");
        if (query.Limit.HasValue) queryParams.Add($"limit={query.Limit}"); if (!string.IsNullOrEmpty(query.Fields)) queryParams.Add($"fields={query.Fields}");
        if (!string.IsNullOrEmpty(query.Include)) queryParams.Add($"include={query.Include}");
        if (!string.IsNullOrEmpty(query.Ids)) queryParams.Add($"ids={query.Ids}");

        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        return url;
    }
    private string BuildSearchUrl(string endpoint, SearchQuery query)
    {
        var url = $"{BaseUrl}/{endpoint}/search";
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(query.Q)) queryParams.Add($"q={Uri.EscapeDataString(query.Q)}");
        if (query.Query != null) queryParams.Add($"query={Uri.EscapeDataString(JsonSerializer.Serialize(query.Query))}");
        if (query.From.HasValue) queryParams.Add($"from={query.From}");
        if (query.Size.HasValue) queryParams.Add($"size={query.Size}");
        if (!string.IsNullOrEmpty(query.Fields)) queryParams.Add($"fields={query.Fields}");
        if (!string.IsNullOrEmpty(query.Facets)) queryParams.Add($"facets={query.Facets}");
        if (!string.IsNullOrEmpty(query.Sort)) queryParams.Add($"sort={query.Sort}");

        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        return url;
    }

    #endregion

    #region Collections - Artworks

    public async Task<ApiResponse<ArtWork>> GetArtworksAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<ArtWork>("artworks", query, cancellationToken);

    public async Task<ArtWork?> GetArtworkAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<ArtWork>("artworks", id, fields, include, cancellationToken);

    public async Task<SearchResponse<ArtWork>> SearchArtworksAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<ArtWork>("artworks", query, cancellationToken);

    public async Task<string> GetArtworkManifestAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/artworks/{id}/manifest.json";
            var manifestRequest = new HttpRequestResult<string> { RequestPath = url, CacheDurationMinutes = 60, Retries = 1 };
            manifestRequest = await _service.HttpSendRequestResultAsync(manifestRequest, ct: cancellationToken).ConfigureAwait(false);
            return manifestRequest.ResponseResults ?? string.Empty;
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpRequestException("Request error occurred", httpEx);
        }
        catch (JsonException jsonEx)
        {
            throw new JsonException("JSON parsing error occurred", jsonEx);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Search for artworks by art style (e.g., Abstract, Impressionism, Cubism)
    /// </summary>
    /// <param name="style">The art style to search for</param>
    /// <param name="limit">Maximum number of results to return (default 25, max 100)</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response containing artworks matching the style</returns>
    public async Task<SearchResponse<ArtWork>> GetArtworksByStyleAsync(ArtStyle style, int? limit = null, int? page = null, CancellationToken cancellationToken = default)
    {
        var query = new SearchQuery
        {
            Q = style.GetDescription(),
            Size = limit ?? 25,
            From = page.HasValue && page > 1 ? (page.Value - 1) * (limit ?? 25) : 0,
            Fields = "id,title,artist_display,date_display,medium_display,dimensions,image_id,thumbnail,style_title,style_titles,classification_title,place_of_origin",
            Query = new
            {
                bool_ = new
                {
                    should = new object[]
                    {
                        new { match = new { style_title = style.GetDescription() } },
                        new { match = new { style_titles = style.GetDescription() } }
                    }
                }
            }
        };

        return await SearchArtworksAsync(query, cancellationToken);
    }

    /// <summary>
    /// Search for artworks by medium (e.g., Oil on canvas, Watercolor, Bronze)
    /// </summary>
    /// <param name="medium">The medium to search for</param>
    /// <param name="limit">Maximum number of results to return (default 25, max 100)</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response containing artworks with the specified medium</returns>
    public async Task<SearchResponse<ArtWork>> GetArtworksByMediumAsync(ArtMedium medium, int? limit = null, int? page = null, CancellationToken cancellationToken = default)
    {
        var query = new SearchQuery
        {
            Q = medium.GetDescription(),
            Size = limit ?? 25,
            From = page.HasValue && page > 1 ? (page.Value - 1) * (limit ?? 25) : 0,
            Fields = "id,title,artist_display,date_display,medium_display,dimensions,image_id,thumbnail,style_title,style_titles,classification_title,place_of_origin",
            Query = new
            {
                bool_ = new
                {
                    should = new object[]
                    {
                        new { match = new { medium_display = medium.GetDescription() } },
                        new { match = new { material_titles = medium.GetDescription() } }
                    }
                }
            }
        };

        return await SearchArtworksAsync(query, cancellationToken);
    }

    /// <summary>
    /// Search for artworks by classification (e.g., Painting, Sculpture, Drawing)
    /// </summary>
    /// <param name="classification">The artwork classification to search for</param>
    /// <param name="limit">Maximum number of results to return (default 25, max 100)</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response containing artworks with the specified classification</returns>
    public async Task<SearchResponse<ArtWork>> GetArtworksByClassificationAsync(ArtworkClassification classification, int? limit = null, int? page = null, CancellationToken cancellationToken = default)
    {
        var query = new SearchQuery
        {
            Q = classification.GetDescription(),
            Size = limit ?? 25,
            From = page.HasValue && page > 1 ? (page.Value - 1) * (limit ?? 25) : 0,
            Fields = "id,title,artist_display,date_display,medium_display,dimensions,image_id,thumbnail,style_title,style_titles,classification_title,place_of_origin",
            Query = new
            {
                bool_ = new
                {
                    should = new object[]
                {
                    new { match = new { classification_title = classification.GetDescription() } },
                    new { match = new { classification_titles = classification.GetDescription() } },
                    new { match = new { artwork_type_title = classification.GetDescription() } }
                }
                }
            }
        };

        return await SearchArtworksAsync(query, cancellationToken);
    }

    /// <summary>
    /// Search for artworks by both style and medium
    /// </summary>
    /// <param name="style">The art style to search for</param>
    /// <param name="medium">The medium to search for</param>
    /// <param name="limit">Maximum number of results to return (default 25, max 100)</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response containing artworks matching both style and medium</returns>
    public async Task<SearchResponse<ArtWork>> GetArtworksByStyleAndMediumAsync(ArtStyle style, ArtMedium medium, int? limit = null, int? page = null, CancellationToken cancellationToken = default)
    {
        var query = new SearchQuery
        {
            Q = $"{style.GetDescription()} {medium.GetDescription()}",
            Size = limit ?? 25,
            From = page.HasValue && page > 1 ? (page.Value - 1) * (limit ?? 25) : 0,
            Fields = "id,title,artist_display,date_display,medium_display,dimensions,image_id,thumbnail,style_title,style_titles,classification_title,place_of_origin",
            Query = new
            {
                bool_ = new
                {
                    must = new[]
                    {                        new
                        {
                            bool_ = new
                            {
                                should = new object[]
                                {
                                    new { match = new { style_title = style.GetDescription() } },
                                    new { match = new { style_titles = style.GetDescription() } }
                                }
                            }
                        },
                        new
                        {
                            bool_ = new
                            {
                                should = new object[]
                                {
                                    new { match = new { medium_display = medium.GetDescription() } },
                                    new { match = new { material_titles = medium.GetDescription() } }
                                }
                            }
                        }
                    }
                }
            }
        }; return await SearchArtworksAsync(query, cancellationToken);
    }

    /// <summary>
    /// Search for artworks by artist name
    /// </summary>
    /// <param name="artistName">The artist name to search for</param>
    /// <param name="limit">Maximum number of results to return (default 25, max 100)</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response containing artworks by the specified artist</returns>
    public async Task<SearchResponse<ArtWork>> GetArtworksByArtistAsync(string artistName, int? limit = null, int? page = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(artistName))
        {
            throw new ArgumentException("Artist name cannot be null or empty", nameof(artistName));
        }

        var query = new SearchQuery
        {
            Q = artistName,
            Size = limit ?? 25,
            From = page.HasValue && page > 1 ? (page.Value - 1) * (limit ?? 25) : 0,
            Fields = "id,title,artist_display,date_display,medium_display,dimensions,image_id,thumbnail,style_title,style_titles,classification_title,place_of_origin",
            Query = new
            {
                bool_ = new
                {
                    should = new object[]
                    {
                        new { match = new { artist_display = new { query = artistName, boost = 2.0 } } },
                        new { match = new { artist_title = new { query = artistName, boost = 1.5 } } },
                        new { match = new { artist_titles = new { query = artistName, boost = 1.0 } } }
                    }
                }
            }
        };

        return await SearchArtworksAsync(query, cancellationToken);
    }
    #endregion

    #region Collections - Agents

    public async Task<ApiResponse<Agent>> GetAgentsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Agent>("agents", query, cancellationToken);

    public async Task<Agent?> GetAgentAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Agent>("agents", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Agent>> SearchAgentsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Agent>("agents", query, cancellationToken);

    #endregion

    #region Collections - Places

    public async Task<ApiResponse<Place>> GetPlacesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Place>("places", query, cancellationToken);

    public async Task<Place?> GetPlaceAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Place>("places", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Place>> SearchPlacesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Place>("places", query, cancellationToken);

    #endregion

    #region Collections - Galleries

    public async Task<ApiResponse<Gallery>> GetGalleriesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Gallery>("galleries", query, cancellationToken);

    public async Task<Gallery?> GetGalleryAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Gallery>("galleries", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Gallery>> SearchGalleriesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Gallery>("galleries", query, cancellationToken);

    #endregion

    #region Collections - Exhibitions

    public async Task<ApiResponse<Exhibition>> GetExhibitionsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Exhibition>("exhibitions", query, cancellationToken);

    public async Task<Exhibition?> GetExhibitionAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Exhibition>("exhibitions", id, fields, include, cancellationToken);

    public async Task<SearchResponse<Exhibition>> SearchExhibitionsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Exhibition>("exhibitions", query, cancellationToken);

    #endregion

    #region Collections - Remaining Collections Types

    public async Task<ApiResponse<AgentType>> GetAgentTypesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<AgentType>("agent-types", query, cancellationToken);

    public async Task<AgentType?> GetAgentTypeAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<AgentType>("agent-types", id, fields, null, cancellationToken);

    public async Task<ApiResponse<AgentRole>> GetAgentRolesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<AgentRole>("agent-roles", query, cancellationToken);

    public async Task<AgentRole?> GetAgentRoleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<AgentRole>("agent-roles", id, fields, null, cancellationToken);

    public async Task<ApiResponse<ArtworkPlaceQualifier>> GetArtworkPlaceQualifiersAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<ArtworkPlaceQualifier>("artwork-place-qualifiers", query, cancellationToken);

    public async Task<ArtworkPlaceQualifier?> GetArtworkPlaceQualifierAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<ArtworkPlaceQualifier>("artwork-place-qualifiers", id, fields, null, cancellationToken);

    public async Task<ApiResponse<ArtworkDateQualifier>> GetArtworkDateQualifiersAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<ArtworkDateQualifier>("artwork-date-qualifiers", query, cancellationToken);

    public async Task<ArtworkDateQualifier?> GetArtworkDateQualifierAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<ArtworkDateQualifier>("artwork-date-qualifiers", id, fields, null, cancellationToken);

    public async Task<ApiResponse<ArtworkType>> GetArtworkTypesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<ArtworkType>("artwork-types", query, cancellationToken);

    public async Task<ArtworkType?> GetArtworkTypeAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<ArtworkType>("artwork-types", id, fields, null, cancellationToken);

    public async Task<ApiResponse<CategoryTerm>> GetCategoryTermsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<CategoryTerm>("category-terms", query, cancellationToken);

    public async Task<CategoryTerm?> GetCategoryTermAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<CategoryTerm>("category-terms", id, fields, null, cancellationToken);

    public async Task<SearchResponse<CategoryTerm>> SearchCategoryTermsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<CategoryTerm>("category-terms", query, cancellationToken);

    public async Task<ApiResponse<Image>> GetImagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Image>("images", query, cancellationToken);

    public async Task<Image?> GetImageAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Image>("images", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Image>> SearchImagesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Image>("images", query, cancellationToken);

    public async Task<ApiResponse<Video>> GetVideosAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Video>("videos", query, cancellationToken);

    public async Task<Video?> GetVideoAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Video>("videos", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Video>> SearchVideosAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Video>("videos", query, cancellationToken);

    public async Task<ApiResponse<Sound>> GetSoundsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Sound>("sounds", query, cancellationToken);

    public async Task<Sound?> GetSoundAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Sound>("sounds", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Sound>> SearchSoundsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Sound>("sounds", query, cancellationToken);

    public async Task<ApiResponse<Text>> GetTextsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Text>("texts", query, cancellationToken);

    public async Task<Text?> GetTextAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Text>("texts", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Text>> SearchTextsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Text>("texts", query, cancellationToken);

    #endregion

    #region Shop

    public async Task<ApiResponse<Product>> GetProductsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Product>("products", query, cancellationToken);

    public async Task<Product?> GetProductAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Product>("products", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Product>> SearchProductsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Product>("products", query, cancellationToken);

    #endregion

    #region Mobile

    public async Task<ApiResponse<Tour>> GetToursAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Tour>("tours", query, cancellationToken);

    public async Task<Tour?> GetTourAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Tour>("tours", id, fields, include, cancellationToken);

    public async Task<SearchResponse<Tour>> SearchToursAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Tour>("tours", query, cancellationToken);

    public async Task<ApiResponse<MobileSound>> GetMobileSoundsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<MobileSound>("mobile-sounds", query, cancellationToken);

    public async Task<MobileSound?> GetMobileSoundAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<MobileSound>("mobile-sounds", id, fields, null, cancellationToken);

    public async Task<SearchResponse<MobileSound>> SearchMobileSoundsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<MobileSound>("mobile-sounds", query, cancellationToken);

    #endregion

    #region Digital Scholarly Catalogs

    public async Task<ApiResponse<Publication>> GetPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Publication>("publications", query, cancellationToken);

    public async Task<Publication?> GetPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Publication>("publications", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Publication>> SearchPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Publication>("publications", query, cancellationToken);

    public async Task<ApiResponse<Section>> GetSectionsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Section>("sections", query, cancellationToken);

    public async Task<Section?> GetSectionAsync(long id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Section>("sections", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Section>> SearchSectionsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Section>("sections", query, cancellationToken);

    #endregion

    #region Static Archive

    public async Task<ApiResponse<Site>> GetSitesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Site>("sites", query, cancellationToken);

    public async Task<Site?> GetSiteAsync(int id, string[]? fields = null, string[]? include = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Site>("sites", id, fields, include, cancellationToken);

    public async Task<SearchResponse<Site>> SearchSitesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Site>("sites", query, cancellationToken);

    #endregion

    #region Website

    public async Task<ApiResponse<Event>> GetEventsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Event>("events", query, cancellationToken);

    public async Task<Event?> GetEventAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Event>("events", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Event>> SearchEventsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Event>("events", query, cancellationToken);

    public async Task<ApiResponse<EventOccurrence>> GetEventOccurrencesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<EventOccurrence>("event-occurrences", query, cancellationToken);

    public async Task<EventOccurrence?> GetEventOccurrenceAsync(string id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<EventOccurrence>("event-occurrences", id, fields, null, cancellationToken);

    public async Task<SearchResponse<EventOccurrence>> SearchEventOccurrencesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<EventOccurrence>("event-occurrences", query, cancellationToken);

    public async Task<ApiResponse<EventProgram>> GetEventProgramsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<EventProgram>("event-programs", query, cancellationToken);

    public async Task<EventProgram?> GetEventProgramAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<EventProgram>("event-programs", id, fields, null, cancellationToken);

    public async Task<SearchResponse<EventProgram>> SearchEventProgramsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<EventProgram>("event-programs", query, cancellationToken);

    public async Task<ApiResponse<Article>> GetArticlesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Article>("articles", query, cancellationToken);

    public async Task<Article?> GetArticleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Article>("articles", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Article>> SearchArticlesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Article>("articles", query, cancellationToken);

    public async Task<ApiResponse<Highlight>> GetHighlightsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<Highlight>("highlights", query, cancellationToken);

    public async Task<Highlight?> GetHighlightAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<Highlight>("highlights", id, fields, null, cancellationToken);

    public async Task<SearchResponse<Highlight>> SearchHighlightsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<Highlight>("highlights", query, cancellationToken);

    public async Task<ApiResponse<StaticPage>> GetStaticPagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<StaticPage>("static-pages", query, cancellationToken);

    public async Task<StaticPage?> GetStaticPageAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<StaticPage>("static-pages", id, fields, null, cancellationToken);

    public async Task<SearchResponse<StaticPage>> SearchStaticPagesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<StaticPage>("static-pages", query, cancellationToken);

    public async Task<ApiResponse<GenericPage>> GetGenericPagesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<GenericPage>("generic-pages", query, cancellationToken);

    public async Task<GenericPage?> GetGenericPageAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<GenericPage>("generic-pages", id, fields, null, cancellationToken);

    public async Task<SearchResponse<GenericPage>> SearchGenericPagesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<GenericPage>("generic-pages", query, cancellationToken);

    public async Task<ApiResponse<PressRelease>> GetPressReleasesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<PressRelease>("press-releases", query, cancellationToken);

    public async Task<PressRelease?> GetPressReleaseAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<PressRelease>("press-releases", id, fields, null, cancellationToken);

    public async Task<SearchResponse<PressRelease>> SearchPressReleasesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<PressRelease>("press-releases", query, cancellationToken);

    public async Task<ApiResponse<EducatorResource>> GetEducatorResourcesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<EducatorResource>("educator-resources", query, cancellationToken);

    public async Task<EducatorResource?> GetEducatorResourceAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<EducatorResource>("educator-resources", id, fields, null, cancellationToken);

    public async Task<SearchResponse<EducatorResource>> SearchEducatorResourcesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<EducatorResource>("educator-resources", query, cancellationToken);

    public async Task<ApiResponse<DigitalPublication>> GetDigitalPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<DigitalPublication>("digital-publications", query, cancellationToken);

    public async Task<DigitalPublication?> GetDigitalPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<DigitalPublication>("digital-publications", id, fields, null, cancellationToken);

    public async Task<SearchResponse<DigitalPublication>> SearchDigitalPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<DigitalPublication>("digital-publications", query, cancellationToken);

    public async Task<ApiResponse<DigitalPublicationArticle>> GetDigitalPublicationArticlesAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<DigitalPublicationArticle>("digital-publication-sections", query, cancellationToken);

    public async Task<DigitalPublicationArticle?> GetDigitalPublicationArticleAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<DigitalPublicationArticle>("digital-publication-sections", id, fields, null, cancellationToken);

    public async Task<SearchResponse<DigitalPublicationArticle>> SearchDigitalPublicationArticlesAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<DigitalPublicationArticle>("digital-publication-sections", query, cancellationToken);

    public async Task<ApiResponse<PrintedPublication>> GetPrintedPublicationsAsync(ApiQuery? query = null, CancellationToken cancellationToken = default)
        => await GetResourcesAsync<PrintedPublication>("printed-publications", query, cancellationToken);

    public async Task<PrintedPublication?> GetPrintedPublicationAsync(int id, string[]? fields = null, CancellationToken cancellationToken = default)
        => await GetResourceAsync<PrintedPublication>("printed-publications", id, fields, null, cancellationToken);

    public async Task<SearchResponse<PrintedPublication>> SearchPrintedPublicationsAsync(SearchQuery query, CancellationToken cancellationToken = default)
        => await SearchResourcesAsync<PrintedPublication>("printed-publications", query, cancellationToken);

    #endregion

    #region Utility Methods

    public string BuildIiifUrl(string imageId, string size = "843,", string format = "jpg")
    {
        return $"{IiifBaseUrl}/{imageId}/full/{size}/0/default.{format}";
    }
    public async Task<ApiResponse<T>> GetResourcesByIdsAsync<T>(string endpoint, object[] ids, string[]? fields = null, CancellationToken cancellationToken = default)
    {
        var query = new ApiQuery
        {
            Ids = string.Join(",", ids),
            Fields = fields != null ? string.Join(",", fields) : null,
            Limit = ids.Length
        };

        return await GetResourcesAsync<T>(endpoint, query, cancellationToken);
    }

    #endregion
}
