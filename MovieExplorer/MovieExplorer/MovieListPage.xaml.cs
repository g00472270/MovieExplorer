namespace MovieExplorer
{
    public partial class MovieListPage : ContentPage
    {
        private MovieService _movieService;
        //Initialise lists
        private List<Movie> _allMovies = new List<Movie>();
        private List<Movie> _filteredMovies = new List<Movie>();

        public MovieListPage()
        {
            InitializeComponent();
            _movieService = new MovieService();
            LoadMovies();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Reload movies when page appears
            LoadMovies();
        }

        private async void LoadMovies()
        {
            MovieCountLabel.Text = "Loading movies...";

            //Get movies from service
            _allMovies = await _movieService.GetMoviesAsync();
            _filteredMovies = _allMovies;

            //Show movies in the list
            MovieCollectionView.ItemsSource = _filteredMovies;
            MovieCountLabel.Text = $"Found {_filteredMovies.Count} movies";

            SetupGenreFilter();
        }

        private void SetupGenreFilter()
        {
            //Genre list
            var genres = new List<string>
            {
                "All Genres",
                "Action",
                "Adventure",
                "Animation",
                "Biography",
                "Comedy",
                "Crime",
                "Drama",
                "Family",
                "Fantasy",
                "History",
                "Horror",
                "Music",
                "Mystery",
                "Romance",
                "Sci-Fi",
                "Thriller",
                "War",
                "Western"
            };

            GenrePicker.ItemsSource = genres;
            GenrePicker.SelectedIndex = 0;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterMovies();
        }

        private void OnGenreChanged(object sender, EventArgs e)
        {
            FilterMovies();
        }

        private void FilterMovies()
        {
            //Get the search text and selected genre
            string searchText = SearchEntry.Text ?? "";
            string selectedGenre = GenrePicker.SelectedItem as string ?? "All Genres";

            //Start with all movies
            _filteredMovies = _allMovies;

            //Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                _filteredMovies = _movieService.SearchMovies(searchText, _filteredMovies);
            }

            //Apply genre filter
            if (!string.IsNullOrWhiteSpace(selectedGenre) && selectedGenre != "All Genres")
            {
                _filteredMovies = _movieService.FilterByGenre(selectedGenre, _filteredMovies);
            }

            //Update the display
            MovieCollectionView.ItemsSource = _filteredMovies;
            MovieCountLabel.Text = $"Found {_filteredMovies.Count} movies";
        }

        private async void OnMovieSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                //Show movie details in a popup
                bool addToFavorites = await DisplayAlert(
                    selectedMovie.Title,
                    $"Year: {selectedMovie.Year}\n" +
                    $"Genre: {selectedMovie.GenreDisplay}\n" +
                    $"Director: {selectedMovie.Director}\n" +
                    $"Rating: {selectedMovie.Rating}/10\n\n" +
                    "Add to favourites?",
                    "Add to Favourites",
                    "Cancel");

                if (addToFavorites)
                {
                    //Load current favourites
                    var favourites = _movieService.LoadFavourites();

                    //Check if already in favourites
                    if (!favourites.Any(f => f.Title == selectedMovie.Title))
                    {
                        favourites.Add(selectedMovie);
                        _movieService.SaveFavourites(favourites);

                        await DisplayAlert("Success!",
                            $"{selectedMovie.Title} added to favourites!",
                            "OK");
                    }
                    else
                    {
                        await DisplayAlert("Already Added",
                            "This movie is already in your favourites.",
                            "OK");
                    }
                }

                //Clear the selection so can tap the same movie again
                MovieCollectionView.SelectedItem = null;
            }
        }
    }
}