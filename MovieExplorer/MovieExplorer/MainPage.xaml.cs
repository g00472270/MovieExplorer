namespace MovieExplorer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /*********    !!!!!!!!NOTE TO SELF--- REMEMBER TO CREATE MovieListPage()!!!!!!!!!!!!!
        private async void OnBrowseMoviesClicked(object sender, EventArgs e)
        {
            //navigation (in navigation lecture)
            await Navigation.PushAsync(new MovieListPage());
        }
        **********************************************/

        /*********    !!!!!!!!NOTE TO SELF--- REMEMBER TO CREATE FavoritesPage()!!!!!!!!!!!!!!!
        private async void OnFavoritesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FavoritesPage());
        }
        ******************************************/

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            //simple popup (in async lecture)
            string theme = await DisplayActionSheet("Choose Theme", "Cancel", null,
                "Light Mode", "Dark Mode", "Auto");

            if (theme == "Light Mode" || theme == "Dark Mode")
            {
                await DisplayAlert("Theme Changed",
                    $"{theme} selected. App will restart to apply.", "OK");
            }
        }
    }
}
