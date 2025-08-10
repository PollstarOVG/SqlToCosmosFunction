namespace SqlToCosmosFunction.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public required string ArtistName { get; set; }
        public required string SortName { get; set; }
        public required string SearchName { get; set; }
        public required string Description { get; set; }    
        public required List<string> Genre { get; set; } 
    }
}
