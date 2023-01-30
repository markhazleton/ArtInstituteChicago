namespace ArtInstituteChicago.Client.Models
{
    // https://api.artic.edu/docs/#conventions
    /// <summary>
    /// Art Works End Point Response
    /// </summary>
    public class ArtWorksResponse
    {
        public Pagination pagination { get; set; }
        public Datum[] data { get; set; }
        public Info info { get; set; }
        public Config config { get; set; }
    }
}
