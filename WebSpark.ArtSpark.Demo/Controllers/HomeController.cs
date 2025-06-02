using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Client.Models.Common;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IArtInstituteClient _artInstituteClient;

    public HomeController(ILogger<HomeController> logger, IArtInstituteClient artInstituteClient)
    {
        _logger = logger;
        _artInstituteClient = artInstituteClient;
    }
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Home page requested at {RequestTime}", DateTime.Now);

        try
        {
            // Get featured artworks for the home page
            var query = new ApiQuery
            {
                Page = 1,
                Limit = 6,
                Fields = "id,title,artist_display,date_display,image_id,thumbnail"
            };

            var response = await _artInstituteClient.GetArtworksAsync(query);

            var featuredArtworks = response?.Data?.Where(a => !string.IsNullOrEmpty(a.ImageId)).Take(6).ToList() ?? new List<ArtWork>();

            return View(featuredArtworks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching featured artworks for home page");
            return View(new List<ArtWork>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
