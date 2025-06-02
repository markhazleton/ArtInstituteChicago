namespace WebSpark.ArtSpark.Agent.Models
{
    public class ArtworkCollection
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ArtworkData> Artworks { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
