namespace MovieExplorer
{
    public partial class FavouritesPage : ContentPage
    {
        private MovieService _movieService;
        private List<Movie> _favourites;

        public FavouritesPage()
        {
            InitializeComponent();
            _movieService = new MovieService();
            LoadFavourites();
            LoadTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Reload favourites when page appears(in case user added new ones)
            LoadFavourites();
            //Reapply theme when returning to page
            LoadTheme();
        }

        private void LoadFavourites()
        {
            _favourites = _movieService.LoadFavourites();

            if (_favourites.Count == 0)
            {
                //No favourites --> show empty message
                FavouritesCollectionView.IsVisible = false;
                EmptyLabel.IsVisible = true;
            }
            else
            {
                //Show favourites list
                FavouritesCollectionView.ItemsSource = _favourites;
                FavouritesCollectionView.IsVisible = true;
                EmptyLabel.IsVisible = false;
            }
        }

        private async void OnRemoveFavouriteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Movie movie)
            {
                //Ask user to confirm
                bool confirm = await DisplayAlert("Remove Favourite",
                    $"Remove {movie.Title} from favourites?",
                    "Remove", "Cancel");

                if (confirm)
                {
                    //Remove from list
                    _favourites.RemoveAll(m => m.Title == movie.Title);

                    //Save updated list
                    _movieService.SaveFavourites(_favourites);

                    //Refresh display
                    LoadFavourites();

                    await DisplayAlert("Removed",
                        $"{movie.Title} removed from favourites.",
                        "OK");
                }
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

            //Apply theme to title label
            TitleLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            //Apply theme to empty label
            EmptyLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
        }
    }
}