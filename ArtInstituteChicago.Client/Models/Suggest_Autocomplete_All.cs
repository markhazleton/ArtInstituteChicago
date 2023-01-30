namespace ArtInstituteChicago.Client.Models
{
    public class Suggest_Autocomplete_All
    {
        public string[] input { get; set; }
        public Contexts contexts { get; set; }
        public int weight { get; set; }
    }
}
