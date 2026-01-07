using System.Text.Json.Serialization;

namespace MovieExplorer
{
    public class Movie
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("genre")]
        public string[] Genre { get; set; } = Array.Empty<string>();

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = string.Empty;

        // Display properties
        public string DisplayInfo => $"{Title} ({Year})";
        public string RatingDisplay => $"⭐ {Rating}/10";
        public string GenreDisplay => Genre != null && Genre.Length > 0 ? string.Join(", ", Genre) : "Unknown";
    }
}