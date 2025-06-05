using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.ArtSpark.Client.Models.Collections;
using WebSpark.ArtSpark.Demo.Data;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Services;

public interface IPublicCollectionService
{
    /// <summary>
    /// Gets public collections with enriched artwork data for SEO-friendly display
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> GetPublicCollectionsAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets featured collections with enriched artwork data
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> GetFeaturedCollectionsAsync(int limit = 10);

    /// <summary>
    /// Gets a single public collection by slug with full artwork details
    /// </summary>
    Task<PublicCollectionDetailsViewModel?> GetPublicCollectionBySlugAsync(string slug);

    /// <summary>
    /// Gets a single public collection by ID with full artwork details
    /// </summary>
    Task<PublicCollectionDetailsViewModel?> GetPublicCollectionByIdAsync(int collectionId);

    /// <summary>
    /// Searches public collections with enriched data
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> SearchPublicCollectionsAsync(string searchTerm, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets collections by tag with enriched data
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> GetPublicCollectionsByTagAsync(string tag, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets the most recent public collections
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> GetRecentPublicCollectionsAsync(int limit = 10);

    /// <summary>
    /// Gets the most viewed public collections
    /// </summary>
    Task<IEnumerable<PublicCollectionViewModel>> GetPopularPublicCollectionsAsync(int limit = 10);

    /// <summary>
    /// Increments the view count for a collection and returns success status
    /// </summary>
    Task<bool> IncrementViewCountAsync(string slug);    /// <summary>
                                                        /// Gets all unique tags from public collections
                                                        /// </summary>
    Task<IEnumerable<string>> GetPublicCollectionTagsAsync();

    /// <summary>
    /// Gets a single collection item by collection and item slugs for public access
    /// </summary>
    Task<CollectionItemDetailsViewModel?> GetPublicCollectionItemBySlugAsync(string collectionSlug, string itemSlug);
}

public class PublicCollectionService : IPublicCollectionService
{
    private readonly ArtSparkDbContext _context;
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly ILogger<PublicCollectionService> _logger;

    public PublicCollectionService(
        ArtSparkDbContext context,
        IArtInstituteClient artInstituteClient,
        ILogger<PublicCollectionService> logger)
    {
        _context = context;
        _artInstituteClient = artInstituteClient;
        _logger = logger;
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> GetPublicCollectionsAsync(int page = 1, int pageSize = 20)
    {
        var skip = (page - 1) * pageSize;

        var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3)) // Preview artworks
            .Where(c => c.IsPublic)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> GetFeaturedCollectionsAsync(int limit = 10)
    {
        var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3)) // Preview artworks
            .Where(c => c.IsPublic && c.IsFeatured &&
                   (c.FeaturedUntil == null || c.FeaturedUntil > DateTime.UtcNow))
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<PublicCollectionDetailsViewModel?> GetPublicCollectionBySlugAsync(string slug)
    {
        var collection = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.OrderBy(a => a.DisplayOrder))
            .Include(c => c.ContentSections.Where(cs => cs.IsVisible).OrderBy(cs => cs.DisplayOrder))
            .Include(c => c.MediaItems.OrderBy(m => m.DisplayOrder))
            .Include(c => c.Links.Where(l => l.IsVisible).OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsPublic);

        if (collection == null)
        {
            return null;
        }

        return await EnrichCollectionDetailsAsync(collection);
    }

    public async Task<PublicCollectionDetailsViewModel?> GetPublicCollectionByIdAsync(int collectionId)
    {
        var collection = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.OrderBy(a => a.DisplayOrder))
            .Include(c => c.ContentSections.Where(cs => cs.IsVisible).OrderBy(cs => cs.DisplayOrder))
            .Include(c => c.MediaItems.OrderBy(m => m.DisplayOrder))
            .Include(c => c.Links.Where(l => l.IsVisible).OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.IsPublic);

        if (collection == null)
        {
            return null;
        }

        return await EnrichCollectionDetailsAsync(collection);
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> SearchPublicCollectionsAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        var skip = (page - 1) * pageSize;
        var lowerSearchTerm = searchTerm.ToLower(); var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3))
            .Where(c => c.IsPublic &&
                   (c.Name.ToLower().Contains(lowerSearchTerm) ||
                    (c.Description != null && c.Description.ToLower().Contains(lowerSearchTerm)) ||
                    (c.Tags != null && c.Tags.ToLower().Contains(lowerSearchTerm))))
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> GetPublicCollectionsByTagAsync(string tag, int page = 1, int pageSize = 20)
    {
        var skip = (page - 1) * pageSize;
        var lowerTag = tag.ToLower(); var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3))
            .Where(c => c.IsPublic && c.Tags != null && c.Tags.ToLower().Contains(lowerTag))
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> GetRecentPublicCollectionsAsync(int limit = 10)
    {
        var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3))
            .Where(c => c.IsPublic)
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<IEnumerable<PublicCollectionViewModel>> GetPopularPublicCollectionsAsync(int limit = 10)
    {
        var collections = await _context.Collections
            .Include(c => c.User)
            .Include(c => c.Artworks.Take(3))
            .Where(c => c.IsPublic)
            .OrderByDescending(c => c.ViewCount)
            .ThenByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return await EnrichCollectionsAsync(collections);
    }

    public async Task<bool> IncrementViewCountAsync(string slug)
    {
        try
        {
            var collection = await _context.Collections
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsPublic);

            if (collection == null)
            {
                return false;
            }

            collection.ViewCount++;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing view count for collection slug: {Slug}", slug);
            return false;
        }
    }
    public async Task<IEnumerable<string>> GetPublicCollectionTagsAsync()
    {
        var tags = await _context.Collections
            .Where(c => c.IsPublic && !string.IsNullOrEmpty(c.Tags))
            .Select(c => c.Tags)
            .ToListAsync();

        return tags
            .Where(tagString => !string.IsNullOrEmpty(tagString))
            .SelectMany(tagString => tagString!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrEmpty(tag))
            .Distinct()
            .OrderBy(tag => tag)
            .ToList();
    }

    public async Task<CollectionItemDetailsViewModel?> GetPublicCollectionItemBySlugAsync(string collectionSlug, string itemSlug)
    {
        try
        {
            var collection = await _context.Collections
                .Include(c => c.Artworks)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Slug == collectionSlug && c.IsPublic);

            if (collection == null)
            {
                return null;
            }

            var collectionArtwork = collection.Artworks.FirstOrDefault(a => a.Slug == itemSlug);
            if (collectionArtwork == null)
            {
                return null;
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
                Keywords = $"{artwork?.ArtistDisplay}, {artwork?.Title}, {collection.Name}",
                SocialImageUrl = !string.IsNullOrEmpty(artwork?.ImageId)
                    ? $"https://www.artic.edu/iiif/2/{artwork.ImageId}/full/1200,630/0/default.jpg"
                    : null,

                // Collection info
                CollectionSlug = collection.Slug,
                CollectionTitle = collection.Name,
                CollectionDescription = collection.Description,

                // Timestamps
                UpdatedAt = collectionArtwork.AddedAt,

                // Control flags - public collections are not editable through this interface
                CanEdit = false
            };

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public collection item. Collection: {CollectionSlug}, Item: {ItemSlug}", collectionSlug, itemSlug);
            return null;
        }
    }

    #region Private Helper Methods

    private async Task<IEnumerable<PublicCollectionViewModel>> EnrichCollectionsAsync(IEnumerable<UserCollection> collections)
    {
        var enrichedCollections = new List<PublicCollectionViewModel>();

        foreach (var collection in collections)
        {
            try
            {
                var enrichedCollection = new PublicCollectionViewModel
                {
                    Collection = collection,
                    CreatorDisplayName = collection.User?.DisplayName ?? "Unknown",
                    ArtworkCount = collection.Artworks.Count(),
                    PreviewArtworks = new List<EnrichedArtworkViewModel>(),
                    Tags = ParseTags(collection.Tags)
                };

                // Enrich preview artworks with API data
                var previewArtworkIds = collection.Artworks.Take(3).Select(a => a.ArtworkId).ToList();
                foreach (var artworkId in previewArtworkIds)
                {
                    var enrichedArtwork = await GetEnrichedArtworkAsync(artworkId);
                    if (enrichedArtwork != null)
                    {
                        enrichedCollection.PreviewArtworks.Add(enrichedArtwork);
                    }
                }

                enrichedCollections.Add(enrichedCollection);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error enriching collection {CollectionId}", collection.Id);

                // Add basic collection without enrichment to avoid losing data
                enrichedCollections.Add(new PublicCollectionViewModel
                {
                    Collection = collection,
                    CreatorDisplayName = collection.User?.DisplayName ?? "Unknown",
                    ArtworkCount = collection.Artworks.Count(),
                    PreviewArtworks = new List<EnrichedArtworkViewModel>(),
                    Tags = ParseTags(collection.Tags)
                });
            }
        }

        return enrichedCollections;
    }

    private async Task<PublicCollectionDetailsViewModel> EnrichCollectionDetailsAsync(UserCollection collection)
    {
        var enrichedArtworks = new List<EnrichedArtworkViewModel>();

        // Enrich all artworks with API data
        foreach (var collectionArtwork in collection.Artworks)
        {
            try
            {
                var enrichedArtwork = await GetEnrichedArtworkAsync(collectionArtwork.ArtworkId);
                if (enrichedArtwork != null)
                {
                    // Add collection-specific metadata
                    enrichedArtwork.CollectionCustomTitle = collectionArtwork.CustomTitle;
                    enrichedArtwork.CollectionCustomDescription = collectionArtwork.CustomDescription;
                    enrichedArtwork.CuratorNotes = collectionArtwork.CuratorNotes;
                    enrichedArtwork.IsHighlighted = collectionArtwork.IsHighlighted;
                    enrichedArtwork.DisplayOrder = collectionArtwork.DisplayOrder;
                    enrichedArtwork.AddedAt = collectionArtwork.AddedAt;
                    enrichedArtwork.Slug = collectionArtwork.Slug ?? collectionArtwork.ArtworkId.ToString(); // Add slug, fallback to ArtworkId

                    enrichedArtworks.Add(enrichedArtwork);
                }
                else
                {
                    // Create placeholder for missing artwork
                    enrichedArtworks.Add(new EnrichedArtworkViewModel
                    {
                        ArtworkId = collectionArtwork.ArtworkId,
                        Title = collectionArtwork.CustomTitle ?? "Artwork Not Available",
                        Artist = "Unknown",
                        CollectionCustomTitle = collectionArtwork.CustomTitle,
                        CollectionCustomDescription = collectionArtwork.CustomDescription,
                        CuratorNotes = collectionArtwork.CuratorNotes,
                        IsHighlighted = collectionArtwork.IsHighlighted,
                        DisplayOrder = collectionArtwork.DisplayOrder,
                        AddedAt = collectionArtwork.AddedAt,
                        Slug = collectionArtwork.Slug ?? collectionArtwork.ArtworkId.ToString() // Add slug, fallback to ArtworkId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error enriching artwork {ArtworkId} for collection {CollectionId}",
                                 collectionArtwork.ArtworkId, collection.Id);
            }
        }
        return new PublicCollectionDetailsViewModel
        {
            Collection = collection,
            CreatorDisplayName = collection.User?.DisplayName ?? "Unknown",
            EnrichedArtworks = enrichedArtworks.OrderBy(a => a.DisplayOrder).ThenByDescending(a => a.AddedAt).ToList(),
            ContentSections = collection.ContentSections,
            MediaItems = collection.MediaItems,
            Links = collection.Links,
            Tags = ParseTags(collection.Tags),
            ArtworkCount = enrichedArtworks.Count
        };
    }
    private async Task<EnrichedArtworkViewModel?> GetEnrichedArtworkAsync(int artworkId)
    {
        try
        {
            var artwork = await _artInstituteClient.GetArtworkAsync(artworkId);
            if (artwork == null)
            {
                return null;
            }
            var enrichedArtwork = new EnrichedArtworkViewModel
            {
                ArtworkId = artwork.Id switch
                {
                    int id => id,
                    JsonElement jsonElement when jsonElement.TryGetInt32(out var id) => id,
                    string idStr when int.TryParse(idStr, out var id) => id,
                    _ => 0
                },
                Title = artwork.Title ?? "Untitled",
                Artist = artwork.ArtistDisplay ?? "Unknown Artist",
                DateDisplay = artwork.DateDisplay,
                MediumDisplay = artwork.MediumDisplay,
                DimensionsDisplay = artwork.Dimensions,
                PlaceOfOrigin = artwork.PlaceOfOrigin,
                Description = artwork.Description,
                ImageId = artwork.ImageId,
                ImageUrl = !string.IsNullOrEmpty(artwork.ImageId)
                  ? $"https://www.artic.edu/iiif/2/{artwork.ImageId}/full/843,/0/default.jpg"
                  : null,
                ThumbnailUrl = !string.IsNullOrEmpty(artwork.ImageId)
                  ? $"https://www.artic.edu/iiif/2/{artwork.ImageId}/full/400,/0/default.jpg"
                  : null,
                StyleTitle = artwork.StyleTitle,
                ClassificationTitle = artwork.ClassificationTitle,
                DepartmentTitle = artwork.DepartmentTitle,
                ArtworkTypeTitle = artwork.ArtworkTypeTitle,
                IsPublicDomain = artwork.IsPublicDomain,
                CreditLine = artwork.CreditLine,
                PublicationHistory = artwork.PublicationHistory,
                ExhibitionHistory = artwork.ExhibitionHistory,
                ProvenanceText = artwork.ProvenanceText
            };

            return enrichedArtwork;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error fetching artwork details for ID {ArtworkId}", artworkId);
            return null;
        }
    }

    private static List<string> ParseTags(string? tagsString)
    {
        if (string.IsNullOrEmpty(tagsString))
        {
            return new List<string>();
        }

        return tagsString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrEmpty(tag))
            .ToList();
    }

    #endregion
}
