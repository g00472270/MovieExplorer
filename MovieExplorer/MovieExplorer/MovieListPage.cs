namespace MovieExplorer
{
    public partial class MovieListPage : ContentPage
    {
        private readonly MovieViewModel _viewModel;

        public MovieListPage()
        {
            InitializeComponent();

            //Create new ViewModel with URL
            _viewModel = new MovieViewModel("https://raw.githubusercontent.com/DonH-TTS/jsonfiles/refs/heads/main/moviesemoji.json");
            //Set BindingContext so XAML can bind to ViewModel properties
            BindingContext = _viewModel;

            LoadMovies();
            LoadTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Refresh display when page appears
            if (_viewModel.IsLoaded)
            {
                _viewModel.UpdateFilteredMovies();
            }
            //Reapply theme when returning to page
            LoadTheme();
        }

        private async void LoadMovies()
        {
            MovieCountLabel.Text = "Loading movies...";

            //Download movies from ViewModel
            await _viewModel.DownloadMovies();

            //Update movie count label
            MovieCountLabel.Text = $"Found {_viewModel.FilteredMovies.Count} movies";
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
            //Update search text in ViewModel, which automatically filters movies
            _viewModel.SearchText = e.NewTextValue ?? "";
            MovieCountLabel.Text = $"Found {_viewModel.FilteredMovies.Count} movies";
        }

        private void OnGenreChanged(object sender, EventArgs e)
        {
            if (GenrePicker.SelectedItem is string genre)
            {
                //Update selected genre in ViewModel, which automatically filters movies
                _viewModel.SelectedGenre = genre;
                MovieCountLabel.Text = $"Found {_viewModel.FilteredMovies.Count} movies";
            }
        }

        private void OnSortClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string sortBy)
            {
                //Sort movies by Title, Year, or Rating
                _viewModel.SortMovies(sortBy);
                MovieCountLabel.Text = $"Found {_viewModel.FilteredMovies.Count} movies";
            }
        }

        private async void OnAddMovieClicked(object sender, EventArgs e)
        {
            //Navigate to Add Movie page and pass ViewModel
            await Navigation.PushAsync(new AddMoviePage(_viewModel));
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
                    var movieService = new MovieService();
                    var favourites = movieService.LoadFavourites();

                    //Check if already in favourites
                    if (!favourites.Any(f => f.Title == selectedMovie.Title))
                    {
                        favourites.Add(selectedMovie);
                        movieService.SaveFavourites(favourites);

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

        //Manual theme application for this page
        private void LoadTheme()
        {
            bool isDarkTheme = Preferences.Get("IsDarkTheme", false);
            ApplyTheme(isDarkTheme);
        }

        private void ApplyTheme(bool isDarkTheme)
        {
            //Apply theme to page background
            this.BackgroundColor = isDarkTheme ? Colors.Black : Colors.White;

            //Apply theme to movie count label
            MovieCountLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            //Apply theme to search entry - TEXT AND BACKGROUND
            SearchEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            SearchEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            //Apply theme to genre picker - TEXT AND BACKGROUND
            GenrePicker.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            GenrePicker.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;
        }
    }
}