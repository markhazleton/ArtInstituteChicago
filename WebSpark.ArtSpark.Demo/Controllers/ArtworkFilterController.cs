using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Client.Models.Common;

namespace WebSpark.ArtSpark.Demo.Controllers;

/// <summary>
/// Controller for filtering artwork by style, medium, and classification
/// </summary>
[Authorize]
public class ArtworkFilterController(
    IArtInstituteClient _artInstituteClient,
    ILogger<ArtworkFilterController> _logger) : Controller
{

    /// <summary>
    /// Display the filter options page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Filter artwork by style
    /// </summary>
    public async Task<IActionResult> ByStyle(ArtStyle? style, int page = 1, int limit = 12)
    {
        try
        {
            if (!style.HasValue)
            {
                return View("Index");
            }

            var response = await _artInstituteClient.GetArtworksByStyleAsync(
                style.Value,
                limit: limit,
                page: page
            );

            if (response?.Data == null)
            {
                _logger.LogWarning("No artworks returned for style: {Style}", style.Value);
                ViewBag.Style = style.Value;
                ViewBag.ErrorMessage = "No artworks found for the selected style.";
                return View("FilterResults", response);
            }

            ViewBag.Style = style.Value;
            ViewBag.FilterType = "Style";
            ViewBag.FilterValue = style.Value.GetDescription();
            return View("FilterResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering artworks by style: {Style}", style);
            ViewBag.ErrorMessage = "An error occurred while filtering artworks.";
            return View("Error");
        }
    }

    /// <summary>
    /// Filter artwork by medium
    /// </summary>
    public async Task<IActionResult> ByMedium(ArtMedium? medium, int page = 1, int limit = 12)
    {
        try
        {
            if (!medium.HasValue)
            {
                return View("Index");
            }

            var response = await _artInstituteClient.GetArtworksByMediumAsync(
                medium.Value,
                limit: limit,
                page: page
            );

            if (response?.Data == null)
            {
                _logger.LogWarning("No artwork returned for medium: {Medium}", medium.Value);
                ViewBag.Medium = medium.Value;
                ViewBag.ErrorMessage = "No artwork found for the selected medium.";
                return View("FilterResults", response);
            }

            ViewBag.Medium = medium.Value;
            ViewBag.FilterType = "Medium";
            ViewBag.FilterValue = medium.Value.GetDescription();
            return View("FilterResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering artwork by medium: {Medium}", medium);
            ViewBag.ErrorMessage = "An error occurred while filtering artwork.";
            return View("Error");
        }
    }

    /// <summary>
    /// Filter artwork by classification
    /// </summary>
    public async Task<IActionResult> ByClassification(ArtworkClassification? classification, int page = 1, int limit = 12)
    {
        try
        {
            if (!classification.HasValue)
            {
                return View("Index");
            }

            var response = await _artInstituteClient.GetArtworksByClassificationAsync(
                classification.Value,
                limit: limit,
                page: page
            );

            if (response?.Data == null)
            {
                _logger.LogWarning("No artwork returned for classification: {Classification}", classification.Value);
                ViewBag.Classification = classification.Value;
                ViewBag.ErrorMessage = "No artwork found for the selected classification.";
                return View("FilterResults", response);
            }

            ViewBag.Classification = classification.Value;
            ViewBag.FilterType = "Classification";
            ViewBag.FilterValue = classification.Value.GetDescription();
            return View("FilterResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering artwork by classification: {Classification}", classification);
            ViewBag.ErrorMessage = "An error occurred while filtering artwork.";
            return View("Error");
        }
    }

    /// <summary>
    /// Filter artwork by both style and medium
    /// </summary>
    public async Task<IActionResult> ByStyleAndMedium(ArtStyle? style, ArtMedium? medium, int page = 1, int limit = 12)
    {
        try
        {
            if (!style.HasValue || !medium.HasValue)
            {
                return View("Index");
            }

            var response = await _artInstituteClient.GetArtworksByStyleAndMediumAsync(
                style.Value,
                medium.Value,
                limit: limit,
                page: page
            );

            if (response?.Data == null)
            {
                _logger.LogWarning("No artwork returned for style: {Style} and medium: {Medium}", style.Value, medium.Value);
                ViewBag.Style = style.Value;
                ViewBag.Medium = medium.Value;
                ViewBag.ErrorMessage = "No artwork found for the selected style and medium combination.";
                return View("FilterResults", response);
            }

            ViewBag.Style = style.Value;
            ViewBag.Medium = medium.Value;
            ViewBag.FilterType = "Style & Medium";
            ViewBag.FilterValue = $"{style.Value.GetDescription()} & {medium.Value.GetDescription()}";
            return View("FilterResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering artwork by style: {Style} and medium: {Medium}", style, medium);
            ViewBag.ErrorMessage = "An error occurred while filtering artwork.";
            return View("Error");
        }
    }

    /// <summary>
    /// Compare different art styles
    /// </summary>
    public async Task<IActionResult> CompareStyles(ArtStyle[]? styles, int limit = 6)
    {
        try
        {
            if (styles == null || !styles.Any())
            {
                return View("Index");
            }

            var comparisonResults = new Dictionary<ArtStyle, IEnumerable<ArtWork>>();

            foreach (var style in styles.Take(4)) // Limit to 4 styles for display purposes
            {
                var response = await _artInstituteClient.GetArtworksByStyleAsync(style, limit: limit);
                comparisonResults[style] = response?.Data ?? new List<ArtWork>();
            }

            ViewBag.ComparisonResults = comparisonResults;
            return View("StyleComparison");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing art styles");
            ViewBag.ErrorMessage = "An error occurred while comparing art styles.";
            return View("Error");
        }
    }

    /// <summary>
    /// Filter artwork by artist name
    /// </summary>
    public async Task<IActionResult> ByArtist(string artistName, int page = 1, int limit = 12)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(artistName))
            {
                return View("Index");
            }

            var response = await _artInstituteClient.GetArtworksByArtistAsync(
                artistName,
                limit: limit,
                page: page
            );

            if (response?.Data == null || !response.Data.Any())
            {
                _logger.LogWarning("No artwork returned for artist: {ArtistName}", artistName);
                ViewBag.ArtistName = artistName;
                ViewBag.ErrorMessage = "No artwork found for the specified artist.";
                return View("FilterResults", response);
            }

            ViewBag.ArtistName = artistName;
            ViewBag.FilterType = "Artist";
            ViewBag.FilterValue = artistName;
            return View("FilterResults", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering artwork by artist: {ArtistName}", artistName);
            ViewBag.ErrorMessage = "An error occurred while filtering artwork.";
            return View("Error");
        }
    }
}
