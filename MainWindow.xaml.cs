using MoviesWatchedApp.DB;
using MoviesWatchedApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoviesWatchedApp
{
    public partial class MainWindow : Window
    {
        private const string ApiKey = "afae67e2"; // Replace with your actual OMDb API key
        private const string ApiUrl = "http://www.omdbapi.com/?apikey={0}&s={1}";

        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            UpdateWatchedList();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await PerformSearch();
        }

        private async void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformSearch();
            }
        }

        private async Task PerformSearch()
        {
            var query = txtSearch.Text;
            var results = await SearchMovies(query);
            lstResults.Items.Clear();
            foreach (var movie in results)
            {
                lstResults.Items.Add(new Movie
                {
                    Id = movie["imdbID"].ToString(),
                    Title = movie["Title"].ToString(),
                    PosterUrl = movie["Poster"].ToString()
                });
            }
        }

        private async Task<JArray> SearchMovies(string query)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(string.Format(ApiUrl, ApiKey, query));
                var json = JObject.Parse(response);
                return (JArray)json["Search"];
            }
        }

        private void lstResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstResults.SelectedItem == null) return;

            var movie = (Movie)lstResults.SelectedItem;

            movie.Rating = double.Parse(Prompt.ShowDialog("Rate this movie (1-10):", "Rating"));
            movie.Cinematography = double.Parse(Prompt.ShowDialog("Rate the cinematography (1-10):", "Cinematography"));
            movie.Comments = Prompt.ShowDialog("Comments:", "Comments");

            movie.OverallScore = (movie.Rating + movie.Cinematography) / 2;

            DatabaseHelper.SaveMovie(movie);
            UpdateWatchedList();
        }

        private void cmbSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSortBy.SelectedItem == null) return;

            var selectedSort = cmbSortBy.SelectedItem as ComboBoxItem;
            var sortOption = selectedSort.Content.ToString();
            var watchedList = DatabaseHelper.LoadMovies();

            if (watchedList == null || !watchedList.Any())
            {
                MessageBox.Show("No items to sort.");
                return;
            }

            switch (sortOption)
            {
                case "Most Recently Added":
                    watchedList = watchedList.OrderByDescending(m => m.DateAdded).ToList();
                    break;
                case "Cinematography Rating":
                    watchedList = watchedList.OrderByDescending(m => m.Cinematography).ToList();
                    break;
                case "Overall Rating":
                    watchedList = watchedList.OrderByDescending(m => m.Rating).ToList();
                    break;
                case "Overall Score":
                    watchedList = watchedList.OrderByDescending(m => m.OverallScore).ToList();
                    break;
                default:
                    MessageBox.Show("Invalid sorting option.");
                    return;
            }

            lstWatched.ItemsSource = watchedList;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var movieId = button?.Tag?.ToString();

            if (movieId == null) return;

            var movie = DatabaseHelper.LoadMovies().FirstOrDefault(m => m.Id == movieId);
            if (movie == null) return;

            movie.Rating = double.Parse(Prompt.ShowDialog("Edit rating (1-10):", "Edit Rating", movie.Rating.ToString()));
            movie.Cinematography = double.Parse(Prompt.ShowDialog("Edit cinematography (1-10):", "Edit Cinematography", movie.Cinematography.ToString()));
            movie.Comments = Prompt.ShowDialog("Edit comments:", "Edit Comments", movie.Comments);
            movie.OverallScore = (movie.Rating + movie.Cinematography) / 2;

            DatabaseHelper.SaveMovie(movie); // Save updated movie details
            UpdateWatchedList(); // Refresh the list
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var movieId = button?.Tag?.ToString();

            if (movieId == null) return;

            DatabaseHelper.DeleteMovie(movieId); // Delete the movie
            UpdateWatchedList(); // Refresh the list
        }

        private void UpdateWatchedList()
        {
            var watchedList = DatabaseHelper.LoadMovies();
            lstWatched.ItemsSource = watchedList;
        }
    }
}
