using Newtonsoft.Json;

namespace SqlToCosmosFunction.Models
{
    public class Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("artistId")]
        public string ArtistId { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = "artist";

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("sortName")]
        public string SortName { get; set; } = string.Empty;

        [JsonProperty("searchName")]
        public string SearchName { get; set; } = string.Empty;

        [JsonProperty("artistType")]
        public string ArtistType { get; set; } = string.Empty;

        [JsonProperty("genre")]
        public List<string> Genre { get; set; } = new();
    }
}
