using MoviesWatchedApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWatchedApp.DB
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=watchedMovies.db;Version=3;";

        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Movies (Id TEXT PRIMARY KEY, Title TEXT, PosterUrl TEXT, Rating REAL, Cinematography REAL, Comments TEXT, OverallScore REAL, DateAdded DATETIME)";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void SaveMovie(Movie movie)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "INSERT OR REPLACE INTO Movies (Id, Title, PosterUrl, Rating, Cinematography, Comments, OverallScore, DateAdded) VALUES (@Id, @Title, @PosterUrl, @Rating, @Cinematography, @Comments, @OverallScore, @DateAdded)";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", movie.Id);
                command.Parameters.AddWithValue("@Title", movie.Title);
                command.Parameters.AddWithValue("@PosterUrl", movie.PosterUrl);
                command.Parameters.AddWithValue("@Rating", movie.Rating);
                command.Parameters.AddWithValue("@Cinematography", movie.Cinematography);
                command.Parameters.AddWithValue("@Comments", movie.Comments);
                command.Parameters.AddWithValue("@OverallScore", movie.OverallScore);
                command.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }

        public static List<Movie> LoadMovies()
        {
            var movies = new List<Movie>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Movies";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var movie = new Movie
                        {
                            Id = reader["Id"].ToString(),
                            Title = reader["Title"].ToString(),
                            PosterUrl = reader["PosterUrl"].ToString(),
                            Rating = double.Parse(reader["Rating"].ToString()),
                            Cinematography = double.Parse(reader["Cinematography"].ToString()),
                            Comments = reader["Comments"].ToString(),
                            OverallScore = double.Parse(reader["OverallScore"].ToString()),
                            DateAdded = DateTime.Parse(reader["DateAdded"].ToString())
                        };
                        movies.Add(movie);
                    }
                }
            }
            return movies;
        }

        public static void DeleteMovie(string id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Movies WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

    }
}
