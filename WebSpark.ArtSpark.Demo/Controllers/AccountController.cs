using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Demo.Models;
using WebSpark.ArtSpark.Demo.Services;
using WebSpark.ArtSpark.Client.Interfaces;

namespace WebSpark.ArtSpark.Demo.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IFavoriteService _favoriteService;
    private readonly ICollectionService _collectionService;
    private readonly IArtInstituteClient _artInstituteClient; public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger,
        IFavoriteService favoriteService,
        ICollectionService collectionService,
        IArtInstituteClient artInstituteClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _favoriteService = favoriteService;
        _collectionService = collectionService;
        _artInstituteClient = artInstituteClient;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            DisplayName = user.DisplayName,
            Email = user.Email!,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            CreatedAt = user.CreatedAt
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        user.DisplayName = model.DisplayName;
        user.Bio = model.Bio;
        user.ProfileImageUrl = model.ProfileImageUrl;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Favorites()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var favorites = await _favoriteService.GetUserFavoritesAsync(user.Id);
        return View(favorites);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Collections()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var collections = await _collectionService.GetUserCollectionsAsync(user.Id);
        return View(collections);
    }

    [Authorize]
    [HttpGet]
    public IActionResult CreateCollection()
    {
        return View(new CreateCollectionViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCollection(CreateCollectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User); if (user == null)
        {
            return NotFound();
        }

        await _collectionService.CreateCollectionAsync(model, user.Id);
        TempData["SuccessMessage"] = "Collection created successfully!";
        return RedirectToAction(nameof(Collections));
    }
    [Authorize]
    [HttpGet]
    [Route("Account/Collection/{id:int}")]
    public async Task<IActionResult> CollectionDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var collection = await _collectionService.GetCollectionByIdAsync(id, user.Id);
        if (collection == null)
        {
            return NotFound();
        }

        var artworks = await _collectionService.GetCollectionArtworksAsync(id, user.Id);

        var model = new CollectionDetailsViewModel
        {
            Collection = collection,
            Artworks = artworks
        };

        return View(model);
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var success = await _collectionService.DeleteCollectionAsync(id, user.Id);
        if (success)
        {
            TempData["SuccessMessage"] = "Collection deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to delete collection.";
        }

        return RedirectToAction(nameof(Collections));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveArtworkFromCollection(int collectionId, int artworkId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var success = await _collectionService.RemoveArtworkFromCollectionAsync(collectionId, artworkId, user.Id);
        if (success)
        {
            TempData["SuccessMessage"] = "Artwork removed from collection successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to remove artwork from collection.";
        }
        return RedirectToAction(nameof(CollectionDetails), new { id = collectionId });
    }

    #region SEO-Friendly Collection Routes    /// <summary>
    /// SEO-friendly collection details view accessible by slug
    /// </summary>
    /// <param name="slug">Collection slug</param>
    /// <returns>Collection details view</returns>
    [HttpGet]
    [Route("Collection/{slug}")]
    public async Task<IActionResult> CollectionDetails(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var model = await _collectionService.GetCollectionDetailsBySlugAsync(slug, userId);

        if (model == null)
        {
            return NotFound();
        }

        // Increment view count for public access or collection owner
        if (model.Collection.IsPublic || model.CanEdit)
        {
            await _collectionService.IncrementViewCountAsync(slug);
        }

        return View("CollectionDetails", model);
    }

    /// <summary>
    /// SEO-friendly collection item details view
    /// </summary>
    /// <param name="collectionSlug">Collection slug</param>
    /// <param name="itemSlug">Item slug</param>
    /// <returns>Collection item details view</returns>    [HttpGet]
    public async Task<IActionResult> CollectionItemDetails(string collectionSlug, string itemSlug)
    {
        if (string.IsNullOrEmpty(collectionSlug) || string.IsNullOrEmpty(itemSlug))
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var collection = await _collectionService.GetCollectionBySlugAsync(collectionSlug, userId);

        if (collection == null)
        {
            return NotFound();
        }

        var collectionArtwork = collection.Artworks.FirstOrDefault(a => a.Slug == itemSlug);
        if (collectionArtwork == null)
        {
            return NotFound();
        }

        // Increment view count for public collections
        if (collection.IsPublic || collection.UserId == userId)
        {
            await _collectionService.IncrementViewCountAsync(collectionSlug);
        }

        // Fetch artwork details from the API
        var artwork = await _artInstituteClient.GetArtworkAsync(collectionArtwork.ArtworkId);

        var model = new CollectionItemDetailsViewModel
        {
            // Basic item info from API
            Id = collectionArtwork.ArtworkId,
            Title = artwork?.Title ?? "Unknown Title",
            Artist = artwork?.ArtistDisplay ?? "Unknown Artist",
            Year = artwork?.DateStart,
            Medium = artwork?.MediumDisplay,
            Dimensions = artwork?.Dimensions,
            Description = artwork?.Description,
            ImageUrl = !string.IsNullOrEmpty(artwork?.ImageId)
                ? $"https://www.artic.edu/iiif/2/{artwork.ImageId}/full/843,/0/default.jpg"
                : null,
            ExternalUrl = artwork != null ? $"https://www.artic.edu/artworks/{artwork.Id}" : null,

            // Collection item specific info
            Slug = collectionArtwork.Slug ?? string.Empty,
            CustomTitle = collectionArtwork.CustomTitle,
            CustomDescription = collectionArtwork.CustomDescription,
            CuratorNotes = collectionArtwork.CuratorNotes,
            DisplayOrder = collectionArtwork.DisplayOrder,
            IsHighlighted = collectionArtwork.IsHighlighted,

            // SEO fields for the collection item
            MetaTitle = collectionArtwork.MetaTitle,
            MetaDescription = collectionArtwork.MetaDescription,
            // Collection info
            CollectionSlug = collection.Slug,
            CollectionTitle = collection.Name,
            CollectionDescription = collection.Description,

            // Control flags
            CanEdit = userId != null && collection.UserId == userId
        };

        return View(model);
    }

    #endregion
}
