using WebSpark.ArtSpark.Agent.Models;
namespace WebSpark.ArtSpark.Agent.Interfaces
{
    public interface IArtworkDataProvider
    {
        Task<ArtworkData?> GetArtworkAsync(int artworkId, CancellationToken cancellationToken = default);
        Task<ArtworkCollection> SearchArtworksAsync(string query, int limit = 20, CancellationToken cancellationToken = default);
        Task<List<ArtworkData>> GetFeaturedArtworksAsync(int count = 10, CancellationToken cancellationToken = default);
        Task<ArtworkData> EnrichArtworkDataAsync(ArtworkData artwork, CancellationToken cancellationToken = default);
    }
}
