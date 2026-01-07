using System.Text.Json.Serialization;

namespace MovieExplorer
{
    public class Movie
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("genre")]
        public string[] Genre { get; set; } = Array.Empty<string>();  //String[] because in the JSON file it's array

        [JsonPropertyName("director")]
        public string Director { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("emoji")]
        public string Emoji { get; set; }

        //Display properties
        public string DisplayInfo => $"{Title} ({Year})";
        public string RatingDisplay => $"⭐ {Rating}/10";

        //Convert array to string for display
        public string GenreDisplay => Genre != null ? string.Join(", ", Genre) : "";
    }
}