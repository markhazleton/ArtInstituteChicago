using ArtInstituteChicago.Client.Models.ArtWorks;
using System.Text.Json;

namespace ArtInstituteChicago.Console.Clients;
public class ArtInstituteClient
{
    public async Task<ArtWorksResponse> GetArtWorks(HttpClient client)
    {
        var response = await client.GetAsync("https://api.artic.edu/api/v1/artworks");
        using var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<ArtWorksResponse>(responseStream);
    }
}
