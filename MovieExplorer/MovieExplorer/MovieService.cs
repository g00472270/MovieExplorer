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
            _client.Timeout = TimeSpan.FromSeconds(30);
            _moviesFile = Path.Combine(FileSystem.AppDataDirectory, "movies.json");
            _favouritesFile = Path.Combine(FileSystem.AppDataDirectory, "favorites.json");
        }

        //Get all movies - try download first, fallback to hardcoded
        public async Task<List<Movie>> GetMoviesAsync()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //If file exists, load it
            if (File.Exists(_moviesFile))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(_moviesFile);
                    var result = JsonSerializer.Deserialize<List<Movie>>(json, options);

                    //If file has movies, return them
                    if (result != null && result.Count > 0)
                    {
                        return result;
                    }
                }
                catch
                {
                    //If file is corrupted, delete it
                    File.Delete(_moviesFile);
                }
            }

            //Try to download from internet
            try
            {
                string url = "https://raw.githubusercontent.com/DonH-TTS/jsonfiles/refs/heads/main/moviesemoji.json";
                string json = await _client.GetStringAsync(url);

                //Save to file
                await File.WriteAllTextAsync(_moviesFile, json);

                //Deserialize and return
                var result = JsonSerializer.Deserialize<List<Movie>>(json, options);

                if (result != null && result.Count > 0)
                {
                    return result;
                }
            }
            catch
            {
                //Download failed - will use hardcoded movies below
            }

            //If download failed, show message and return hardcoded movies
            await Application.Current.MainPage.DisplayAlert("Using Offline Movies",
                "Could not download movies from internet.\nShowing 10 hardcoded movies instead.", "OK");

            return GetHardcodedMovies();
        }

        //Hardcoded movies as fallback when download fails
        private List<Movie> GetHardcodedMovies()
        {
            return new List<Movie>
            {
                new Movie { Title = "The Shawshank Redemption", Year = 1994, Genre = new[] { "Drama" }, Director = "Frank Darabont", Rating = 9.3, Emoji = "🎭" },
                new Movie { Title = "The Godfather", Year = 1972, Genre = new[] { "Crime", "Drama" }, Director = "Francis Ford Coppola", Rating = 9.2, Emoji = "🕴️" },
                new Movie { Title = "The Dark Knight", Year = 2008, Genre = new[] { "Action", "Crime", "Drama" }, Director = "Christopher Nolan", Rating = 9.0, Emoji = "🦇" },
                new Movie { Title = "Pulp Fiction", Year = 1994, Genre = new[] { "Crime", "Drama" }, Director = "Quentin Tarantino", Rating = 8.9, Emoji = "💼" },
                new Movie { Title = "Forrest Gump", Year = 1994, Genre = new[] { "Drama", "Romance" }, Director = "Robert Zemeckis", Rating = 8.8, Emoji = "🏃‍♂️" },
                new Movie { Title = "Inception", Year = 2010, Genre = new[] { "Action", "Adventure", "Sci-Fi" }, Director = "Christopher Nolan", Rating = 8.8, Emoji = "🚀" },
                new Movie { Title = "The Matrix", Year = 1999, Genre = new[] { "Action", "Sci-Fi" }, Director = "Lana Wachowski, Lilly Wachowski", Rating = 8.7, Emoji = "🕶️" },
                new Movie { Title = "The Lion King", Year = 1994, Genre = new[] { "Animation", "Adventure", "Drama" }, Director = "Roger Allers, Rob Minkoff", Rating = 8.5, Emoji = "🦁" },
                new Movie { Title = "Back to the Future", Year = 1985, Genre = new[] { "Adventure", "Comedy", "Sci-Fi" }, Director = "Robert Zemeckis", Rating = 8.5, Emoji = "🚗" },
                new Movie { Title = "Parasite", Year = 2019, Genre = new[] { "Comedy", "Drama", "Thriller" }, Director = "Bong Joon Ho", Rating = 8.6, Emoji = "🏠" }
            };
        }

        //Search movies by title
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

        //Filter movies by genre
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