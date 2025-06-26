using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Client.Models.Common;

namespace WebSpark.ArtSpark.Demo.Controllers.Api;

/// <summary>
/// API controller for artwork-related operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArtworkApiController : ControllerBase
{
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ILogger<ArtworkApiController> _logger;

    public ArtworkApiController(
        IArtInstituteClient artInstituteClient,
        ILogger<ArtworkApiController> logger)
    {
        _artInstituteClient = artInstituteClient ?? throw new ArgumentNullException(nameof(artInstituteClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a specific artwork by ID
    /// </summary>
    /// <param name="id">Artwork ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="include">Optional related data to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Artwork details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArtWork), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ArtWork>> GetArtwork(
        int id,
        [FromQuery] string[]? fields = null,
        [FromQuery] string[]? include = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var artwork = await _artInstituteClient.GetArtworkAsync(id, fields, include, cancellationToken);

            if (artwork == null)
            {
                _logger.LogWarning("Artwork with ID {ArtworkId} not found", id);
                return NotFound($"Artwork with ID {id} not found");
            }

            return Ok(artwork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork with ID {ArtworkId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the artwork");
        }
    }

    /// <summary>
    /// Get all artworks with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of artworks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ArtWork>>> GetArtworks(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetArtworksAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artworks");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving artworks");
        }
    }

    /// <summary>
    /// Search artworks by artist name
    /// </summary>
    /// <param name="artistName">Artist name to search for</param>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks by artist</returns>
    [HttpGet("by-artist/{artistName}")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> GetArtworksByArtist(
        string artistName,
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(artistName))
            {
                return BadRequest("Artist name is required");
            }

            var result = await _artInstituteClient.GetArtworksByArtistAsync(artistName, limit, page, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks by artist {ArtistName}", artistName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Search artworks by classification
    /// </summary>
    /// <param name="classification">Artwork classification</param>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks by classification</returns>
    [HttpGet("by-classification/{classification}")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> GetArtworksByClassification(
        ArtworkClassification classification,
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _artInstituteClient.GetArtworksByClassificationAsync(classification, limit, page, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks by classification {Classification}", classification);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Search artworks by medium
    /// </summary>
    /// <param name="medium">Art medium</param>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks by medium</returns>
    [HttpGet("by-medium/{medium}")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> GetArtworksByMedium(
        ArtMedium medium,
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _artInstituteClient.GetArtworksByMediumAsync(medium, limit, page, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks by medium {Medium}", medium);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Search artworks by style
    /// </summary>
    /// <param name="style">Art style</param>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks by style</returns>
    [HttpGet("by-style/{style}")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> GetArtworksByStyle(
        ArtStyle style,
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _artInstituteClient.GetArtworksByStyleAsync(style, limit, page, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks by style {Style}", style);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Search artworks by style and medium
    /// </summary>
    /// <param name="style">Art style</param>
    /// <param name="medium">Art medium</param>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks by style and medium</returns>
    [HttpGet("by-style-and-medium/{style}/{medium}")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> GetArtworksByStyleAndMedium(
        ArtStyle style,
        ArtMedium medium,
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _artInstituteClient.GetArtworksByStyleAndMediumAsync(style, medium, limit, page, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks by style {Style} and medium {Medium}", style, medium);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Get artwork manifest for IIIF
    /// </summary>
    /// <param name="id">Artwork ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IIIF manifest as string</returns>
    [HttpGet("{id:int}/manifest")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GetArtworkManifest(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var manifest = await _artInstituteClient.GetArtworkManifestAsync(id, cancellationToken);

            if (string.IsNullOrEmpty(manifest))
            {
                _logger.LogWarning("Manifest for artwork with ID {ArtworkId} not found", id);
                return NotFound($"Manifest for artwork with ID {id} not found");
            }

            return Ok(manifest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving manifest for artwork with ID {ArtworkId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the artwork manifest");
        }
    }

    /// <summary>
    /// Search artworks using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for artworks</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchResponse<ArtWork>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<ArtWork>>> SearchArtworks(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchArtworksAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching artworks");
        }
    }

    /// <summary>
    /// Get artwork types
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of artwork types</returns>
    [HttpGet("types")]
    [ProducesResponseType(typeof(ApiResponse<ArtworkType>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ArtworkType>>> GetArtworkTypes(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetArtworkTypesAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork types");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving artwork types");
        }
    }

    /// <summary>
    /// Get a specific artwork type by ID
    /// </summary>
    /// <param name="id">Artwork type ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Artwork type details</returns>
    [HttpGet("types/{id:int}")]
    [ProducesResponseType(typeof(ArtworkType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ArtworkType>> GetArtworkType(
        int id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var artworkType = await _artInstituteClient.GetArtworkTypeAsync(id, fields, cancellationToken);

            if (artworkType == null)
            {
                _logger.LogWarning("Artwork type with ID {ArtworkTypeId} not found", id);
                return NotFound($"Artwork type with ID {id} not found");
            }

            return Ok(artworkType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork type with ID {ArtworkTypeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the artwork type");
        }
    }

    /// <summary>
    /// Get artwork date qualifiers
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of artwork date qualifiers</returns>
    [HttpGet("date-qualifiers")]
    [ProducesResponseType(typeof(ApiResponse<ArtworkDateQualifier>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ArtworkDateQualifier>>> GetArtworkDateQualifiers(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetArtworkDateQualifiersAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork date qualifiers");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving artwork date qualifiers");
        }
    }

    /// <summary>
    /// Get artwork place qualifiers
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of artwork place qualifiers</returns>
    [HttpGet("place-qualifiers")]
    [ProducesResponseType(typeof(ApiResponse<ArtworkPlaceQualifier>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ArtworkPlaceQualifier>>> GetArtworkPlaceQualifiers(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetArtworkPlaceQualifiersAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork place qualifiers");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving artwork place qualifiers");
        }
    }
}
