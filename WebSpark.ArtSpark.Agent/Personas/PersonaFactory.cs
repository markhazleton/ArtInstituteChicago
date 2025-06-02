namespace WebSpark.ArtSpark.Agent.Personas
{
    // 6. Personas/PersonaFactory.cs
    using WebSpark.ArtSpark.Agent.Interfaces;
    using WebSpark.ArtSpark.Agent.Models;

    public class PersonaFactory : IPersonaFactory
    {
        public IPersonaHandler CreatePersona(ChatPersona persona, ArtworkData artwork)
        {
            return persona switch
            {
                ChatPersona.Artwork => new ArtworkPersona(),
                ChatPersona.Artist => new ArtistPersona(),
                ChatPersona.Curator => new CuratorPersona(),
                ChatPersona.Historian => new HistorianPersona(),
                _ => throw new ArgumentException($"Unknown persona type: {persona}")
            };
        }
    }
}