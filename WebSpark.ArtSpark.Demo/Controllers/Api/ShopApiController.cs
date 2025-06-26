using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Common;
using WebSpark.ArtSpark.Client.Models.Shop;

namespace WebSpark.ArtSpark.Demo.Controllers.Api;

/// <summary>
/// API controller for shop-related operations (products)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShopApiController : ControllerBase
{
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ILogger<ShopApiController> _logger;

    public ShopApiController(
        IArtInstituteClient artInstituteClient,
        ILogger<ShopApiController> logger)
    {
        _artInstituteClient = artInstituteClient ?? throw new ArgumentNullException(nameof(artInstituteClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    [HttpGet("products/{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Product>> GetProduct(
        int id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _artInstituteClient.GetProductAsync(id, fields, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product");
        }
    }

    /// <summary>
    /// Get all products with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet("products")]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Product>>> GetProducts(
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

            var result = await _artInstituteClient.GetProductsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products");
        }
    }

    /// <summary>
    /// Search products using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for products</returns>
    [HttpPost("products/search")]
    [ProducesResponseType(typeof(SearchResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<Product>>> SearchProducts(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchProductsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching products");
        }
    }
}
