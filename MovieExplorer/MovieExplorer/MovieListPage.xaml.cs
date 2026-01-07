namespace MovieExplorer
{
    public partial class MovieListPage : ContentPage
    {
        private readonly MovieViewModel _viewModel;

        public MovieListPage()
        {
            InitializeComponent();

            //Create new ViewModel with URL to download movies
            _viewModel = new MovieViewModel("https://raw.githubusercontent.com/DonH-TTS/jsonfiles/refs/heads/main/moviesemoji.json");
            BindingContext = _viewModel;

            LoadMovies();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.IsLoaded)
            {
                _viewModel.UpdateFilteredMovies(); //Refresh display
            }
        }

        private async void LoadMovies()
        {
            MovieCountLabel.Text = "Loading movies...";

            //Download movies from URL
            await _viewModel.DownloadMovies();

            //Update MovieCountLabel
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

            //Set default genre to first item
            GenrePicker.ItemsSource = genres;
            GenrePicker.SelectedIndex = 0;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            //Update search text in ViewModel, automatically filter movies
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
                //Show movie details in a popup and ask to add to favourites
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
                    //Load current favourites list
                    var movieService = new MovieService();
                    var favourites = movieService.LoadFavourites();

                    //Check if movie is already in favourites
                    if (!favourites.Any(f => f.Title == selectedMovie.Title))
                    {
                        //Add movie to favourites and save
                        favourites.Add(selectedMovie);
                        movieService.SaveFavourites(favourites);

                        await DisplayAlert("Success!",
                            $"{selectedMovie.Title} added to favourites!",
                            "OK");
                    }
                    else
                    {
                        //Movie already in favourites
                        await DisplayAlert("Already Added",
                            "This movie is already in your favourites.",
                            "OK");
                    }
                }

                //Clear selection so user can tap same movie again
                MovieCollectionView.SelectedItem = null;
            }
        }
    }
}