using System.Text.Json;

namespace MovieExplorer
{
    public class MovieService
    {
        private HttpClient _client;
        private string _moviesFile;
        private string _favouritesFile;

        public MovieService()
        {
            _client = new HttpClient();
            _moviesFile = Path.Combine(FileSystem.AppDataDirectory, "movies.json");
            _favouritesFile = Path.Combine(FileSystem.AppDataDirectory, "favorites.json");
        }

        //Get all movies from file or download them
        public async Task<List<Movie>> GetMoviesAsync()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //If file exists, load it
            if (File.Exists(_moviesFile))
            {
                string json = File.ReadAllText(_moviesFile);
                return JsonSerializer.Deserialize<List<Movie>>(json, options) ?? new List<Movie>();
            }

            try
            {
                string url = "https://raw.githubusercontent.com/DonH-TTS/jsonfiles/refs/heads/main/moviesemoji.json";
                string json = await _client.GetStringAsync(url);

                File.WriteAllText(_moviesFile, json);

                return JsonSerializer.Deserialize<List<Movie>>(json, options) ?? new List<Movie>();
            }
            catch
            {
                return new List<Movie>();
            }
        }

        //Loop through and check names one by one
        public List<Movie> SearchMovies(string searchText, List<Movie> movies)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return movies;

            List<Movie> results = new List<Movie>();
            foreach (var m in movies)
            {
                //Safety check: skip if title is null
                if (m.Title != null && m.Title.ToLower().Contains(searchText.ToLower()))
                {
                    results.Add(m);
                }
            }
            return results;
        }

        //Check genre list one by one
        public List<Movie> FilterByGenre(string genre, List<Movie> movies)
        {
            if (string.IsNullOrWhiteSpace(genre) || genre == "All Genres")
                return movies;

            List<Movie> results = new List<Movie>();
            foreach (var m in movies)
            {
                if (m.Genre != null)
                {
                    foreach (var g in m.Genre)
                    {
                        if (g.ToLower() == genre.ToLower())
                        {
                            results.Add(m);
                            break; //If found, move to next movie
                        }
                    }
                }
            }
            return results;
        }

        //Save favourites to file
        public void SaveFavourites(List<Movie> favorites)
        {
            string json = JsonSerializer.Serialize(favorites);
            File.WriteAllText(_favouritesFile, json);
        }

        //Load favourites from file
        public List<Movie> LoadFavourites()
        {
            if (File.Exists(_favouritesFile))
            {
                string json = File.ReadAllText(_favouritesFile);

                //PropertyNameCaseInsensitive = don't care about capital letter or small letter
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<Movie>>(json, options) ?? new List<Movie>();
            }

            return new List<Movie>();
        }
    }
}