using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Demo.Models;
using WebSpark.ArtSpark.Demo.Services;

namespace WebSpark.ArtSpark.Demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ICollectionService _collectionService;
    private readonly IPublicCollectionService _publicCollectionService;

    public HomeController(ILogger<HomeController> logger, IArtInstituteClient artInstituteClient, ICollectionService collectionService, IPublicCollectionService publicCollectionService)
    {
        _logger = logger;
        _artInstituteClient = artInstituteClient;
        _collectionService = collectionService;
        _publicCollectionService = publicCollectionService;
    }
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Home page requested at {RequestTime}", DateTime.Now);
        PublicCollectionDetailsViewModel randomCollection = new();

        try
        {
            // Get a random public collection to showcase on the home page
            randomCollection = await _publicCollectionService.GetRandomPublicCollectionAsync() ?? new();

            if (randomCollection != null)
            {
                // Increment view count for the showcased collection
                await _publicCollectionService.IncrementViewCountAsync(randomCollection.Collection.Slug);

                return View(randomCollection);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching random collection for home page");

        }
        randomCollection = new PublicCollectionDetailsViewModel
        {
            Collection = new UserCollection
            {
                Id = 0,
                Description = "Currently, there are no featured collections available. Please check back later.",
                Slug = "no-featured-collection",
                SocialImageUrl = "/images/no-featured-collection.jpg",
                MetaTitle = "No Featured Collection",
                MetaDescription = "There are no featured collections available at this time. Please check back later for updates.",
                MediaItems = [],
                Artworks = [],
                ContentSections = [],
                Tags = "No Tags",
            },

        };
        return View(randomCollection);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Display all public collections
    /// </summary>
    public async Task<IActionResult> Collections(int page = 1, string? search = null, string? tag = null)
    {
        try
        {
            var collections = await _collectionService.GetPublicCollectionsAsync(page, 12, search, tag);

            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Tag = tag;
            ViewBag.Title = "Explore Collections";
            ViewBag.MetaDescription = "Discover curated art collections created by our community. Browse featured collections and find inspiration.";

            return View(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public collections");
            return View(new List<UserCollection>());
        }
    }    /// <summary>
         /// Display featured collections only
         /// </summary>
    public async Task<IActionResult> FeaturedCollections()
    {
        try
        {
            var featuredCollections = await _collectionService.GetFeaturedCollectionsAsync(12);

            ViewBag.Title = "Featured Collections";
            ViewBag.MetaDescription = "Explore our hand-picked featured art collections showcasing the best curated artwork from around the world.";

            return View("Collections", featuredCollections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching featured collections");
            return View("Collections", new List<UserCollection>());
        }
    }

    /// <summary>
    /// Display collections by tag
    /// </summary>
    public async Task<IActionResult> CollectionsByTag(string tag, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return RedirectToAction(nameof(Collections));
        }

        try
        {
            var collections = await _collectionService.GetPublicCollectionsAsync(page, 12, null, tag);

            ViewBag.CurrentPage = page;
            ViewBag.Tag = tag;
            ViewBag.Title = $"Collections tagged with '{tag}'";
            ViewBag.MetaDescription = $"Browse art collections tagged with '{tag}' and discover curated artwork collections.";

            return View("Collections", collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching collections by tag: {Tag}", tag);
            return View("Collections", new List<UserCollection>());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
