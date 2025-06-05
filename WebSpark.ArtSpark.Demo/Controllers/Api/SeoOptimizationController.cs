using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSpark.ArtSpark.Demo.Models.Api;
using WebSpark.ArtSpark.Demo.Services;

namespace WebSpark.ArtSpark.Demo.Controllers.Api;

/// <summary>
/// API controller for SEO optimization services
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for SEO optimization
public class SeoOptimizationController : ControllerBase
{
    private readonly ISeoOptimizationService _seoService;
    private readonly ILogger<SeoOptimizationController> _logger;

    public SeoOptimizationController(
        ISeoOptimizationService seoService,
        ILogger<SeoOptimizationController> logger)
    {
        _seoService = seoService ?? throw new ArgumentNullException(nameof(seoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Optimizes a collection for SEO based on a description
    /// </summary>
    /// <param name="request">Collection optimization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimized collection data</returns>
    [HttpPost("optimize-collection")]
    [ProducesResponseType(typeof(OptimizeCollectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OptimizeCollectionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OptimizeCollectionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OptimizeCollectionResponse>> OptimizeCollection(
        [FromBody] OptimizeCollectionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for collection optimization request");
                return BadRequest(new OptimizeCollectionResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request data"
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return Unauthorized();
            }

            _logger.LogInformation("Processing collection optimization request for user {UserId}", userId);

            var optimizedCollection = await _seoService.OptimizeCollectionAsync(
                request.Description,
                userId,
                cancellationToken);

            var response = new OptimizeCollectionResponse
            {
                Success = true,
                Collection = new UserCollectionDto
                {
                    Name = optimizedCollection.Name,
                    Description = optimizedCollection.Description,
                    LongDescription = optimizedCollection.LongDescription,
                    Slug = optimizedCollection.Slug,
                    MetaTitle = optimizedCollection.MetaTitle,
                    MetaDescription = optimizedCollection.MetaDescription,
                    MetaKeywords = optimizedCollection.MetaKeywords,
                    Tags = optimizedCollection.Tags,
                    CuratorNotes = optimizedCollection.CuratorNotes,
                    IsPublic = optimizedCollection.IsPublic
                }
            };

            _logger.LogInformation("Successfully optimized collection for user {UserId}", userId);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument in collection optimization request");
            return BadRequest(new OptimizeCollectionResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during collection optimization");
            return StatusCode(StatusCodes.Status500InternalServerError, new OptimizeCollectionResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while optimizing the collection"
            });
        }
    }

    /// <summary>
    /// Optimizes an artwork for SEO within a collection context
    /// </summary>
    /// <param name="request">Artwork optimization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimized collection artwork data</returns>
    [HttpPost("optimize-artwork")]
    [ProducesResponseType(typeof(OptimizeArtworkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OptimizeArtworkResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OptimizeArtworkResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OptimizeArtworkResponse>> OptimizeArtwork(
        [FromBody] OptimizeArtworkRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for artwork optimization request");
                return BadRequest(new OptimizeArtworkResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request data"
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims");
                return Unauthorized();
            }

            _logger.LogInformation("Processing artwork optimization request for user {UserId}, artwork {ArtworkId}, collection {CollectionId}",
                userId, request.ArtworkData.Id, request.CollectionId);

            var optimizedArtwork = await _seoService.OptimizeCollectionArtworkAsync(
                request.ArtworkData,
                request.CollectionId,
                cancellationToken);

            var response = new OptimizeArtworkResponse
            {
                Success = true,
                CollectionArtwork = new CollectionArtworkDto
                {
                    CollectionId = optimizedArtwork.CollectionId,
                    ArtworkId = optimizedArtwork.ArtworkId,
                    Slug = optimizedArtwork.Slug,
                    CustomTitle = optimizedArtwork.CustomTitle,
                    CustomDescription = optimizedArtwork.CustomDescription,
                    CuratorNotes = optimizedArtwork.CuratorNotes,
                    MetaTitle = optimizedArtwork.MetaTitle,
                    MetaDescription = optimizedArtwork.MetaDescription,
                    DisplayOrder = optimizedArtwork.DisplayOrder
                }
            };

            _logger.LogInformation("Successfully optimized artwork {ArtworkId} for collection {CollectionId}",
                request.ArtworkData.Id, request.CollectionId);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument in artwork optimization request");
            return BadRequest(new OptimizeArtworkResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during artwork optimization");
            return StatusCode(StatusCodes.Status500InternalServerError, new OptimizeArtworkResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while optimizing the artwork"
            });
        }
    }
}
