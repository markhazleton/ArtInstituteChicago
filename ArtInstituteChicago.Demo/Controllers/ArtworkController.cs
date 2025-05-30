using ArtInstituteChicago.Client.Interfaces;
using ArtInstituteChicago.Client.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ArtInstituteChicago.Demo.Controllers;

/// <summary>
/// Controller for displaying artwork from the Art Institute of Chicago
/// </summary>
public class ArtworkController : Controller
{
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ILogger<ArtworkController> _logger;

    public ArtworkController(IArtInstituteClient artInstituteClient, ILogger<ArtworkController> logger)
    {
        _artInstituteClient = artInstituteClient ?? throw new ArgumentNullException(nameof(artInstituteClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
}
