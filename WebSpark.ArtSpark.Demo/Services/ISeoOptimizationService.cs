using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Services;

/// <summary>
/// Service for SEO optimization of collections and artwork items using AI
/// </summary>
public interface ISeoOptimizationService
{
    /// <summary>
    /// Optimizes a collection for SEO based on a descriptive string
    /// </summary>
    /// <param name="collectionDescription">Description of the collection to optimize</param>
    /// <param name="userId">ID of the user creating the collection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A fully optimized UserCollection instance</returns>
    Task<UserCollection> OptimizeCollectionAsync(
        string collectionDescription,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes an artwork item for SEO within a collection context
    /// </summary>
    /// <param name="artworkData">The artwork data to optimize</param>
    /// <param name="collectionId">ID of the collection the artwork belongs to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A fully optimized CollectionArtwork instance</returns>
    Task<CollectionArtwork> OptimizeCollectionArtworkAsync(
        ArtworkData artworkData,
        int collectionId,
        CancellationToken cancellationToken = default);
}
