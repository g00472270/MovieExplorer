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
            //Show where file would be saved
            await Application.Current.MainPage.DisplayAlert("File Path",
                $"Movies will be saved to: {_moviesFile}", "OK");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //If file exists, load it
            if (File.Exists(_moviesFile))
            {
                await Application.Current.MainPage.DisplayAlert("Info",
                    "Found existing movies.json file. Loading...", "OK");

                try
                {
                    string json = File.ReadAllText(_moviesFile);

                    await Application.Current.MainPage.DisplayAlert("File Size",
                        $"File has {json.Length} characters", "OK");

                    var result = JsonSerializer.Deserialize<List<Movie>>(json, options) ?? new List<Movie>();

                    await Application.Current.MainPage.DisplayAlert("Success",
                        $"Loaded {result.Count} movies from file", "OK");

                    return result;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("File Error",
                        $"Error reading file: {ex.Message}. Will try to download...", "OK");

                    //Delete bad file
                    File.Delete(_moviesFile);
                }
            }

            //Download from internet
            await Application.Current.MainPage.DisplayAlert("Info",
                "No local file found. Attempting download...", "OK");

            try
            {
                string url = "https://raw.githubusercontent.com/DonH-TTS/jsonfiles/refs/heads/main/moviesemoji.json";

                await Application.Current.MainPage.DisplayAlert("Downloading",
                    $"Downloading from: {url}", "OK");

                string json = await _client.GetStringAsync(url);

                await Application.Current.MainPage.DisplayAlert("Downloaded",
                    $"Downloaded {json.Length} characters", "OK");

                //Save to file
                File.WriteAllText(_moviesFile, json);

                await Application.Current.MainPage.DisplayAlert("Saved",
                    "File saved successfully", "OK");

                //Deserialize and return
                var result = JsonSerializer.Deserialize<List<Movie>>(json, options) ?? new List<Movie>();

                await Application.Current.MainPage.DisplayAlert("Deserialized",
                    $"Parsed {result.Count} movies", "OK");

                return result;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Download Error",
                    $"Could not download: {ex.Message}\n\nType: {ex.GetType().Name}", "OK");
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