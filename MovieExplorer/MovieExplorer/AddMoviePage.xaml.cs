using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;

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
    }
}