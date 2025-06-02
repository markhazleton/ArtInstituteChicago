namespace WebSpark.ArtSpark.Agent.Configuration;

public class ArtInstituteSettings
{
    public string BaseUrl { get; set; } = "https://api.artic.edu/api/v1";
    public string ImageBaseUrl { get; set; } = "https://www.artic.edu/iiif/2";
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromHours(24);
    public int MaxSearchResults { get; set; } = 100;
    public string DefaultImageSize { get; set; } = "843,";
    public List<string> RequiredFields { get; set; } = new()
    {
        "id", "title", "artist_display", "date_display", "place_of_origin",
        "medium_display", "dimensions", "description", "style_title",
        "classification_title", "image_id"
    };
}
