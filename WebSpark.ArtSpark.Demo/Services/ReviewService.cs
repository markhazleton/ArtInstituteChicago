using Microsoft.EntityFrameworkCore;
using WebSpark.ArtSpark.Demo.Data;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Services;

public interface IReviewService
{
    Task<IEnumerable<ArtworkReview>> GetReviewsForArtworkAsync(int artworkId);
    Task<ArtworkReview?> GetUserReviewAsync(int artworkId, string userId);
    Task<ArtworkReview> CreateOrUpdateReviewAsync(int artworkId, string userId, int rating, string? reviewText);
    Task<bool> DeleteReviewAsync(int reviewId, string userId);
    Task<double> GetAverageRatingAsync(int artworkId);
    Task<int> GetReviewCountAsync(int artworkId);
}

public class ReviewService : IReviewService
{
    private readonly ArtSparkDbContext _context;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(ArtSparkDbContext context, ILogger<ReviewService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ArtworkReview>> GetReviewsForArtworkAsync(int artworkId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ArtworkId == artworkId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<ArtworkReview?> GetUserReviewAsync(int artworkId, string userId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.ArtworkId == artworkId && r.UserId == userId);
    }

    public async Task<ArtworkReview> CreateOrUpdateReviewAsync(int artworkId, string userId, int rating, string? reviewText)
    {
        var existingReview = await GetUserReviewAsync(artworkId, userId);

        if (existingReview != null)
        {
            existingReview.Rating = rating;
            existingReview.ReviewText = reviewText;
            existingReview.UpdatedAt = DateTime.UtcNow;
            _context.Reviews.Update(existingReview);
        }
        else
        {
            existingReview = new ArtworkReview
            {
                ArtworkId = artworkId,
                UserId = userId,
                Rating = rating,
                ReviewText = reviewText
            };
            _context.Reviews.Add(existingReview);
        }

        await _context.SaveChangesAsync();
        return existingReview;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double> GetAverageRatingAsync(int artworkId)
    {
        var ratings = await _context.Reviews
            .Where(r => r.ArtworkId == artworkId)
            .Select(r => r.Rating)
            .ToListAsync();

        return ratings.Any() ? ratings.Average() : 0;
    }

    public async Task<int> GetReviewCountAsync(int artworkId)
    {
        return await _context.Reviews
            .CountAsync(r => r.ArtworkId == artworkId);
    }
}

public interface IFavoriteService
{
    Task<bool> AddToFavoritesAsync(int artworkId, string userId);
    Task<bool> RemoveFromFavoritesAsync(int artworkId, string userId);
    Task<bool> IsArtworkFavoritedAsync(int artworkId, string userId);
    Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(string userId);
}

public class FavoriteService : IFavoriteService
{
    private readonly ArtSparkDbContext _context;

    public FavoriteService(ArtSparkDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddToFavoritesAsync(int artworkId, string userId)
    {
        var existing = await _context.Favorites
            .FirstOrDefaultAsync(f => f.ArtworkId == artworkId && f.UserId == userId);

        if (existing != null) return false;

        _context.Favorites.Add(new UserFavorite
        {
            ArtworkId = artworkId,
            UserId = userId
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromFavoritesAsync(int artworkId, string userId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.ArtworkId == artworkId && f.UserId == userId);

        if (favorite == null) return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsArtworkFavoritedAsync(int artworkId, string userId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.ArtworkId == artworkId && f.UserId == userId);
    }
    public async Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
}

public interface ICollectionService
{
    Task<IEnumerable<UserCollection>> GetUserCollectionsAsync(string userId);
    Task<UserCollection?> GetCollectionByIdAsync(int collectionId, string userId);
    Task<UserCollection> CreateCollectionAsync(string userId, string name, string? description, bool isPublic);
    Task<bool> UpdateCollectionAsync(int collectionId, string userId, string name, string? description, bool isPublic);
    Task<bool> DeleteCollectionAsync(int collectionId, string userId);
    Task<bool> AddArtworkToCollectionAsync(int collectionId, int artworkId, string userId);
    Task<bool> RemoveArtworkFromCollectionAsync(int collectionId, int artworkId, string userId);
    Task<IEnumerable<CollectionArtwork>> GetCollectionArtworksAsync(int collectionId, string userId);
    Task<bool> IsArtworkInCollectionAsync(int collectionId, int artworkId);
}

public class CollectionService : ICollectionService
{
    private readonly ArtSparkDbContext _context;
    private readonly ILogger<CollectionService> _logger;

    public CollectionService(ArtSparkDbContext context, ILogger<CollectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserCollection>> GetUserCollectionsAsync(string userId)
    {
        return await _context.Collections
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserCollection?> GetCollectionByIdAsync(int collectionId, string userId)
    {
        return await _context.Collections
            .Include(c => c.Artworks)
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);
    }

    public async Task<UserCollection> CreateCollectionAsync(string userId, string name, string? description, bool isPublic)
    {
        var collection = new UserCollection
        {
            UserId = userId,
            Name = name,
            Description = description,
            IsPublic = isPublic
        };

        _context.Collections.Add(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task<bool> UpdateCollectionAsync(int collectionId, string userId, string name, string? description, bool isPublic)
    {
        var collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);

        if (collection == null) return false;

        collection.Name = name;
        collection.Description = description;
        collection.IsPublic = isPublic;

        _context.Collections.Update(collection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCollectionAsync(int collectionId, string userId)
    {
        var collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);

        if (collection == null) return false;

        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddArtworkToCollectionAsync(int collectionId, int artworkId, string userId)
    {
        var collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);

        if (collection == null) return false;

        var existing = await _context.CollectionArtworks
            .FirstOrDefaultAsync(ca => ca.CollectionId == collectionId && ca.ArtworkId == artworkId);

        if (existing != null) return false;

        _context.CollectionArtworks.Add(new CollectionArtwork
        {
            CollectionId = collectionId,
            ArtworkId = artworkId
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveArtworkFromCollectionAsync(int collectionId, int artworkId, string userId)
    {
        var collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);

        if (collection == null) return false;

        var collectionArtwork = await _context.CollectionArtworks
            .FirstOrDefaultAsync(ca => ca.CollectionId == collectionId && ca.ArtworkId == artworkId);

        if (collectionArtwork == null) return false;

        _context.CollectionArtworks.Remove(collectionArtwork);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CollectionArtwork>> GetCollectionArtworksAsync(int collectionId, string userId)
    {
        var collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId && c.UserId == userId);

        if (collection == null) return new List<CollectionArtwork>();

        return await _context.CollectionArtworks
            .Where(ca => ca.CollectionId == collectionId)
            .OrderByDescending(ca => ca.AddedAt)
            .ToListAsync();
    }

    public async Task<bool> IsArtworkInCollectionAsync(int collectionId, int artworkId)
    {
        return await _context.CollectionArtworks
            .AnyAsync(ca => ca.CollectionId == collectionId && ca.ArtworkId == artworkId);
    }
}
