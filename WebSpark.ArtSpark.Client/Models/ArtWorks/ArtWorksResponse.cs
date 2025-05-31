namespace WebSpark.ArtSpark.Client.Models.ArtWorks
{
    /// <summary>
    /// Art Works End Point Response
    // https://api.artic.edu/api/v1/artworks
    /// </summary>
    public class ArtWorksResponse
    {
        public Pagination? pagination { get; set; }
        public Datum[]? data { get; set; }
        public Info? info { get; set; }
        public Config? config { get; set; }
    }
}
