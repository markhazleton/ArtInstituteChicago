using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;
namespace WebSpark.ArtSpark.Agent.Interfaces
{
    public interface IPersonaFactory
    {
        IPersonaHandler CreatePersona(ChatPersona persona, ArtworkData artwork);
    }
}
