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
