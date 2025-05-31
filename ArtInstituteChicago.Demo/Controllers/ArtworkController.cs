using ArtInstituteChicago.Client.Interfaces;
using ArtInstituteChicago.Client.Models.Common;
using ArtInstituteChicago.Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ArtInstituteChicago.Demo.Controllers;

/// <summary>
/// Controller for displaying artwork from the Art Institute of Chicago
/// </summary>
public class ArtworkController : Controller
{
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ILogger<ArtworkController> _logger;
    private readonly IReviewService _reviewService;
    private readonly IFavoriteService _favoriteService;

    public ArtworkController(
        IArtInstituteClient artInstituteClient,
        ILogger<ArtworkController> logger,
        IReviewService reviewService,
        IFavoriteService favoriteService)
    {
        _artInstituteClient = artInstituteClient ?? throw new ArgumentNullException(nameof(artInstituteClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        _favoriteService = favoriteService ?? throw new ArgumentNullException(nameof(favoriteService));
    }

    /// <summary>
    /// Display a paginated list of artworks
    /// </summary>
    public async Task<IActionResult> Index(int page = 1, int limit = 12)
    {
        try
        {
            var query = new ApiQuery
            {
                Page = page,
                Limit = limit,
                Fields = "id,title,artist_display,date_display,medium_display,image_id"
            };

            var response = await _artInstituteClient.GetArtworksAsync(query);

            if (response?.Data == null)
            {
                _logger.LogWarning("No artworks returned from API");
                return View("Error");
            }

            return View(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching artworks");
            return View("Error");
        }
    }

    /// <summary>
    /// Display details for a specific artwork
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var artwork = await _artInstituteClient.GetArtworkAsync(id);

            if (artwork == null)
            {
                _logger.LogWarning("Artwork with ID {ArtworkId} not found", id);
                return NotFound();
            }

            return View(artwork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching artwork details for ID {ArtworkId}", id);
            return View("Error");
        }
    }

    /// <summary>
    /// Search for artworks
    /// </summary>
    public async Task<IActionResult> Search(string q, int page = 1, int limit = 12)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var searchQuery = new SearchQuery
            {
                Q = q,
                From = (page - 1) * limit,
                Size = limit,
                Fields = "id,title,artist_display,date_display,medium_display,image_id"
            };

            var response = await _artInstituteClient.SearchArtworksAsync(searchQuery);

            if (response?.Data == null)
            {
                _logger.LogWarning("No search results returned for query: {SearchQuery}", q);
                ViewBag.SearchQuery = q;
                return View("SearchResults", response);
            }

            ViewBag.SearchQuery = q;
            return View("SearchResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching artworks for query: {SearchQuery}", q);
            return View("Error");
        }
    }

    /// <summary>
    /// Display featured/highlighted artworks
    /// </summary>
    public async Task<IActionResult> Featured()
    {
        try
        {
            // Get a selection of featured artworks with images
            var query = new ApiQuery
            {
                Page = 1,
                Limit = 20,
                Fields = "id,title,artist_display,date_display,medium_display,image_id,thumbnail"
            };

            var response = await _artInstituteClient.GetArtworksAsync(query);

            if (response?.Data == null)
            {
                _logger.LogWarning("No featured artworks returned from API");
                return View("Error");
            }

            // Filter to only show items with images
            var featuredArtworks = response.Data.Where(a => !string.IsNullOrEmpty(a.ImageId)).Take(12).ToList();

            return View(featuredArtworks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching featured artworks");
            return View("Error");
        }
    }

    /// <summary>
    /// Submit or update a review for an artwork
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(int artworkId, int rating, string? reviewText)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await _reviewService.CreateOrUpdateReviewAsync(artworkId, userId, rating, reviewText);

            TempData["SuccessMessage"] = "Your review has been submitted successfully!";
            return RedirectToAction("Details", new { id = artworkId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting review for artwork {ArtworkId}", artworkId);
            TempData["ErrorMessage"] = "There was an error submitting your review. Please try again.";
            return RedirectToAction("Details", new { id = artworkId });
        }
    }

    /// <summary>
    /// Get reviews for an artwork (AJAX endpoint)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReviews(int artworkId)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsForArtworkAsync(artworkId);
            var averageRating = await _reviewService.GetAverageRatingAsync(artworkId);
            var reviewCount = await _reviewService.GetReviewCountAsync(artworkId);

            var result = new
            {
                reviews = reviews.Select(r => new
                {
                    id = r.Id,
                    userName = r.User.DisplayName,
                    rating = r.Rating,
                    reviewText = r.ReviewText,
                    createdAt = r.CreatedAt.ToString("MMM dd, yyyy"),
                    isOwner = User.FindFirstValue(ClaimTypes.NameIdentifier) == r.UserId
                }),
                averageRating = Math.Round(averageRating, 1),
                reviewCount = reviewCount
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reviews for artwork {ArtworkId}", artworkId);
            return Json(new { error = "Failed to load reviews" });
        }
    }

    /// <summary>
    /// Toggle favorite status for an artwork
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int artworkId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var isFavorited = await _favoriteService.IsArtworkFavoritedAsync(artworkId, userId);
            bool success;

            if (isFavorited)
            {
                success = await _favoriteService.RemoveFromFavoritesAsync(artworkId, userId);
            }
            else
            {
                success = await _favoriteService.AddToFavoritesAsync(artworkId, userId);
            }

            return Json(new { success = success, isFavorited = !isFavorited });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for artwork {ArtworkId}", artworkId);
            return Json(new { success = false, error = "Failed to update favorite status" });
        }
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var success = await _reviewService.DeleteReviewAsync(reviewId, userId);

            return Json(new { success = success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
            return Json(new { success = false, error = "Failed to delete review" });
        }
    }
}
