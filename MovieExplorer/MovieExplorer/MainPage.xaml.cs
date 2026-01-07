namespace MovieExplorer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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
            string theme = await DisplayActionSheet("Choose Theme", "Cancel", null, "Light Mode", "Dark Mode", "Auto");

            if (theme == "Light Mode" || theme == "Dark Mode")
            {
                await DisplayAlert("Theme Changed", $"{theme} selected. App will restart to apply.", "OK");
            }
        }
    }
}