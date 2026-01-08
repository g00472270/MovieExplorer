using System.Collections.ObjectModel;

namespace MovieExplorer
{
    public partial class AddMoviePage : ContentPage
    {
        //Observable collection of genres(like the tracks in week 11 album list)
        public ObservableCollection<string> Genres { get; } = new ObservableCollection<string>();

        private readonly MovieViewModel _viewModel;

        public AddMoviePage(MovieViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            //Set binding context for genre collection
            GenresCollectionView.BindingContext = this;
            LoadTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Reapply theme when returning to page
            LoadTheme();
        }

        private void AddGenreClicked(object sender, EventArgs e)
        {
            //Add genre to collection if entry is not empty
            if (!string.IsNullOrWhiteSpace(NewGenreEntry.Text))
            {
                Genres.Add(NewGenreEntry.Text.Trim());
                NewGenreEntry.Text = string.Empty; //Clear entry field
            }
        }

        private void RemoveGenreClicked(object sender, EventArgs e)
        {
            //Remove genre from collection when X button is clicked
            if (sender is Button btn && btn.BindingContext is string genre)
            {
                Genres.Remove(genre);
            }
        }

        private async void AddMovieClicked(object sender, EventArgs e)
        {
            //Validate title - required field
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Error", "Movie title is required.", "OK");
                return;
            }

            //Validate year - must be valid integer
            if (!int.TryParse(YearEntry.Text?.Trim(), out int year))
            {
                await DisplayAlert("Error", "Invalid year given.", "OK");
                return;
            }

            //Validate rating - must be valid double
            if (!double.TryParse(RatingEntry.Text?.Trim(), out double rating))
            {
                await DisplayAlert("Error", "Invalid rating given.", "OK");
                return;
            }

            //Create new movie object
            var newMovie = new Movie
            {
                Title = TitleEntry.Text.Trim(),
                Year = year,
                Genre = Genres.ToArray(), //Change ObservableCollection to array
                Director = DirectorEntry.Text?.Trim() ?? "",
                Rating = rating,
                Emoji = EmojiEntry.Text?.Trim() ?? "" //Default emoji if empty
            };

            //Add movie to ViewModel's observable collection
            _viewModel.Movies.Add(newMovie);

            //Save movies to file
            await _viewModel.SaveMovies();

            //Show success message and go back
            await DisplayAlert("Success", $"{newMovie.Title} has been added!", "OK");
            await Navigation.PopAsync();
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

            //Apply theme to all labels
            TitleLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            TitleLabelText.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            YearLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            DirectorLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            RatingLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            EmojiLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            GenreLabel.TextColor = isDarkTheme ? Colors.White : Colors.Black;

            //Apply theme to all entries
            TitleEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            TitleEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            YearEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            YearEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            DirectorEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            DirectorEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            RatingEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            RatingEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            EmojiEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            EmojiEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            NewGenreEntry.TextColor = isDarkTheme ? Colors.White : Colors.Black;
            NewGenreEntry.BackgroundColor = isDarkTheme ? Colors.DarkGray : Colors.LightGray;

            //Apply theme to buttons
            AddGenreButton.BackgroundColor = isDarkTheme ? Colors.Blue : Colors.Purple;
            AddGenreButton.TextColor = Colors.White;

            AddMovieButton.BackgroundColor = Colors.Green;
            AddMovieButton.TextColor = Colors.White;
        }
    }
}