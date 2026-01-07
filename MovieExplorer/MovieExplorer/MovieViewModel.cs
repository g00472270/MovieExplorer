using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieExplorer
{
    public class MovieViewModel : INotifyPropertyChanged
    {
        private string _url;
        private Movie _selectedMovie;
        private string _searchText = "";
        private string _selectedGenre = "All Genres";
        private MovieService _movieService;

        //Observable collection of all movies
        private ObservableCollection<Movie> _movies;
        public ObservableCollection<Movie> Movies
        {
            get => _movies;
            set
            {
                _movies = value;
                OnPropertyChanged();
                UpdateFilteredMovies(); //Update filtered list when movies change
            }
        }

        //Observable collection of filtered movies(after search/genre filter)
        private ObservableCollection<Movie> _filteredMovies;
        public ObservableCollection<Movie> FilteredMovies
        {
            get => _filteredMovies;
            set
            {
                _filteredMovies = value;
                OnPropertyChanged();
            }
        }

        //Track if movies loaded
        public bool IsLoaded { get; private set; } = false;

        //Download movies from URL using MovieService
        public async Task DownloadMovies()
        {
            if (!IsLoaded)
            {
                //Use existing MovieService which already works
                var moviesList = await _movieService.GetMoviesAsync();
                Movies = new ObservableCollection<Movie>(moviesList);
                IsLoaded = true;
            }
        }

        //Sort movies by Title, Year, or Rating
        public void SortMovies(string sortBy)
        {
            switch (sortBy)
            {
                case "Title":
                    Movies = new ObservableCollection<Movie>(Movies.OrderBy(m => m.Title));
                    break;
                case "Year":
                    Movies = new ObservableCollection<Movie>(Movies.OrderBy(m => m.Year));
                    break;
                case "Rating":
                    Movies = new ObservableCollection<Movie>(Movies.OrderByDescending(m => m.Rating));
                    break;
            }
        }

        //Save movies to local file
        public async Task SaveMovies()
        {
            //Convert ObservableCollection back to List and save
            var moviesList = Movies.ToList();

            //Save using JsonSerializer directly
            string filename = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "movies.json");
            string jsonContents = System.Text.Json.JsonSerializer.Serialize(moviesList);
            await File.WriteAllTextAsync(filename, jsonContents);
        }

        //Search text property with automatic filtering
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    UpdateFilteredMovies(); //Auto-filter when search text changes
                }
            }
        }

        //Selected genre property with automatic filtering
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged();
                    UpdateFilteredMovies(); //Auto filter when genre changes
                }
            }
        }

        //Update filtered movies based on search text and genre
        public void UpdateFilteredMovies()
        {
            if (Movies == null)
            {
                FilteredMovies = new ObservableCollection<Movie>();
                return;
            }

            var filtered = Movies.AsEnumerable();

            //Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(m => m.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            //Apply genre filter
            if (!string.IsNullOrWhiteSpace(SelectedGenre) && SelectedGenre != "All Genres")
            {
                filtered = filtered.Where(m => m.Genre != null && m.Genre.Any(g => g.Equals(SelectedGenre, StringComparison.OrdinalIgnoreCase)));
            }

            FilteredMovies = new ObservableCollection<Movie>(filtered);
        }

        //Selected movie property
        public Movie SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged();
                }
            }
        }

        //Initialise ViewModel with url
        public MovieViewModel(string url)
        {
            _url = url;
            _movieService = new MovieService(); //Use existing MovieService
            _movies = new ObservableCollection<Movie>();
            _filteredMovies = new ObservableCollection<Movie>();
        }

        //Property changed event for data binding
        public event PropertyChangedEventHandler? PropertyChanged;

        //Notify UI when property changes
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}