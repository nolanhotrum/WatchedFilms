using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWatchedApp.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public double Rating { get; set; } // EnjoymentRating
        public double Cinematography { get; set; }
        public string Comments { get; set; }
        public double OverallScore { get; set; }
        public DateTime DateAdded { get; set; }
    }

}
