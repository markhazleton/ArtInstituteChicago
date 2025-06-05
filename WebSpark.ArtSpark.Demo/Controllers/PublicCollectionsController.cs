using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Demo.Services;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Controllers;

/// <summary>
/// Controller for displaying public collections with SEO-friendly experiences
/// </summary>
public class PublicCollectionsController : Controller
{
    private readonly IPublicCollectionService _publicCollectionService;
    private readonly ILogger<PublicCollectionsController> _logger;

    public PublicCollectionsController(
        IPublicCollectionService publicCollectionService,
        ILogger<PublicCollectionsController> logger)
    {
        _publicCollectionService = publicCollectionService;
        _logger = logger;
    }

    /// <summary>
    /// Display all public collections with search and filtering
    /// </summary>
    /// <param name="page">Page number for pagination</param>
    /// <param name="search">Search term for filtering collections</param>
    /// <param name="tag">Tag for filtering collections</param>
    /// <returns>Public collections view</returns>
    [HttpGet]
    [Route("explore/collections")]
    public async Task<IActionResult> Index(int page = 1, string? search = null, string? tag = null)
    {
        try
        {
            IEnumerable<PublicCollectionViewModel> collections;

            if (!string.IsNullOrWhiteSpace(search))
            {
                collections = await _publicCollectionService.SearchPublicCollectionsAsync(search, page, 12);
            }
            else if (!string.IsNullOrWhiteSpace(tag))
            {
                collections = await _publicCollectionService.GetPublicCollectionsByTagAsync(tag, page, 12);
            }
            else
            {
                collections = await _publicCollectionService.GetPublicCollectionsAsync(page, 12);
            }

            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Tag = tag;
            ViewBag.Title = "Explore Collections";
            ViewBag.MetaDescription = "Discover curated art collections created by our community. Browse featured collections and find inspiration.";

            return View("Collections", collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public collections");
            return View("Collections", new List<PublicCollectionViewModel>());
        }
    }

    /// <summary>
    /// Display featured collections only
    /// </summary>
    /// <returns>Featured collections view</returns>
    [HttpGet]
    [Route("explore/featured")]
    public async Task<IActionResult> Featured()
    {
        try
        {
            var featuredCollections = await _publicCollectionService.GetFeaturedCollectionsAsync(12);

            ViewBag.Title = "Featured Collections";
            ViewBag.MetaDescription = "Explore our hand-picked featured art collections showcasing the best curated artwork from around the world.";

            return View("Collections", featuredCollections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching featured collections");
            return View("Collections", new List<PublicCollectionViewModel>());
        }
    }

    /// <summary>
    /// Display recent public collections
    /// </summary>
    /// <param name="limit">Number of collections to return</param>
    /// <returns>Recent collections view</returns>
    [HttpGet]
    [Route("explore/recent")]
    public async Task<IActionResult> Recent(int limit = 20)
    {
        try
        {
            var recentCollections = await _publicCollectionService.GetRecentPublicCollectionsAsync(limit);

            ViewBag.Title = "Recent Collections";
            ViewBag.MetaDescription = "Browse the latest art collections shared by our community members.";

            return View("Collections", recentCollections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent collections");
            return View("Collections", new List<PublicCollectionViewModel>());
        }
    }

    /// <summary>
    /// Display popular public collections
    /// </summary>
    /// <param name="limit">Number of collections to return</param>
    /// <returns>Popular collections view</returns>
    [HttpGet]
    [Route("explore/popular")]
    public async Task<IActionResult> Popular(int limit = 20)
    {
        try
        {
            var popularCollections = await _publicCollectionService.GetPopularPublicCollectionsAsync(limit);

            ViewBag.Title = "Popular Collections";
            ViewBag.MetaDescription = "Discover the most viewed and appreciated art collections from our community.";

            return View("Collections", popularCollections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching popular collections");
            return View("Collections", new List<PublicCollectionViewModel>());
        }
    }

    /// <summary>
    /// Display collections filtered by tag
    /// </summary>
    /// <param name="tag">Tag to filter by</param>
    /// <param name="page">Page number for pagination</param>
    /// <returns>Collections filtered by tag</returns>
    [HttpGet]
    [Route("explore/collections/tag/{tag}")]
    public async Task<IActionResult> ByTag(string tag, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var collections = await _publicCollectionService.GetPublicCollectionsByTagAsync(tag, page, 12);

            ViewBag.CurrentPage = page;
            ViewBag.Tag = tag;
            ViewBag.Title = $"Collections tagged with '{tag}'";
            ViewBag.MetaDescription = $"Browse art collections tagged with '{tag}' and discover curated artwork collections.";

            return View("Collections", collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching collections by tag: {Tag}", tag);
            return View("Collections", new List<PublicCollectionViewModel>());
        }
    }    /// <summary>
         /// SEO-friendly collection details view accessible by slug
         /// </summary>
         /// <param name="slug">Collection slug</param>
         /// <returns>Collection details view</returns>
    [HttpGet]
    [Route("collection/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        try
        {
            var collectionDetails = await _publicCollectionService.GetPublicCollectionBySlugAsync(slug);

            if (collectionDetails == null)
            {
                return NotFound();
            }

            // Increment view count for public access
            await _publicCollectionService.IncrementViewCountAsync(slug);

            return View("CollectionDetails", collectionDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching collection details for slug: {Slug}", slug);
            return NotFound();
        }
    }

    /// <summary>
    /// SEO-friendly collection item details view for public collections
    /// </summary>
    /// <param name="collectionSlug">Collection slug</param>
    /// <param name="itemSlug">Item slug</param>
    /// <returns>Collection item details view</returns>
    [HttpGet]
    [Route("collection/{collectionSlug}/item/{itemSlug}")]
    public async Task<IActionResult> ItemDetails(string collectionSlug, string itemSlug)
    {
        if (string.IsNullOrEmpty(collectionSlug) || string.IsNullOrEmpty(itemSlug))
        {
            return NotFound();
        }

        try
        {
            var itemDetails = await _publicCollectionService.GetPublicCollectionItemBySlugAsync(collectionSlug, itemSlug);

            if (itemDetails == null)
            {
                return NotFound();
            }

            // Increment view count for the collection
            await _publicCollectionService.IncrementViewCountAsync(collectionSlug);

            return View("CollectionItemDetails", itemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching collection item details. Collection: {CollectionSlug}, Item: {ItemSlug}", collectionSlug, itemSlug);
            return NotFound();
        }
    }

    /// <summary>
    /// Get available tags for public collections (API endpoint)
    /// </summary>
    /// <returns>JSON array of available tags</returns>
    [HttpGet]
    [Route("api/public-collections/tags")]
    public async Task<IActionResult> GetTags()
    {
        try
        {
            var tags = await _publicCollectionService.GetPublicCollectionTagsAsync();
            return Json(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching collection tags");
            return Json(new List<string>());
        }
    }
}
