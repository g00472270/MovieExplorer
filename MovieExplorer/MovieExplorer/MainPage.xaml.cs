namespace MovieExplorer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Reapply theme when returning to page
            LoadTheme();
        }

        private async void OnBrowseMoviesClicked(object sender, EventArgs e)
        {
            //Navigate to movie list page
            await Navigation.PushAsync(new MovieListPage());
        }

        private async void OnFavoritesClicked(object sender, EventArgs e)
        {
            //Navigate to favourites page
            await Navigation.PushAsync(new FavouritesPage());
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            //Theme selector popup
            string theme = await DisplayActionSheet("Choose Theme", "Cancel", null, "Light Mode", "Dark Mode");

            if (theme == "Light Mode")
            {
                Preferences.Set("IsDarkTheme", false);
                ApplyTheme(false);
            }
            else if (theme == "Dark Mode")
            {
                Preferences.Set("IsDarkTheme", true);
                ApplyTheme(true);
            }
        }

        private void LoadTheme()
        {
            //Load saved theme preference
            bool isDarkTheme = Preferences.Get("IsDarkTheme", false);
            ApplyTheme(isDarkTheme);
        }

        private void ApplyTheme(bool isDarkTheme)
        {
            //Apply theme to page background
            this.BackgroundColor = isDarkTheme ? Colors.Black : Colors.White;

            //Apply theme to title label
            TitleLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            //Apply theme to all buttons with proper colors
            BrowseButton.BackgroundColor = isDarkTheme ? Colors.Blue : Colors.Purple;
            BrowseButton.TextColor = Colors.White;

            FavouritesButton.BackgroundColor = isDarkTheme ? Colors.Blue : Colors.Purple;
            FavouritesButton.TextColor = Colors.White;

            SettingsButton.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;
            SettingsButton.TextColor = isDarkTheme ? Colors.White : Colors.Black;
        }
    }
}