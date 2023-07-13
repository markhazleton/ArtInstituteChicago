using ArtInstituteChicago.Client.Models.ArtWorks;
using System.Text.Json;

namespace ArtInstituteChicago.Console.Clients;
public class ArtInstituteClient
{
    public async Task<ArtWorksResponse> GetArtWorks(HttpClient client)
    {
        ArtWorksResponse myart = new ArtWorksResponse();
        try
        {
            var response = await client.GetAsync("https://api.artic.edu/api/v1/artworks");
            using var responseStream = await response.Content.ReadAsStreamAsync();
            myart = await JsonSerializer.DeserializeAsync<ArtWorksResponse>(responseStream);
        }
        catch (Exception ex)
        {
            myart = new ArtWorksResponse();
        }


        return myart;
    }
}
