using ArtInstituteChicago.Console.Clients;

var client = new ArtInstituteClient();
var response = await client.GetArtWorks(HttpClientFactory.Create());
Console.WriteLine(response.config.website_url);
response.data.ToList().ForEach(x => Console.WriteLine(x.title));
Console.ReadLine();



