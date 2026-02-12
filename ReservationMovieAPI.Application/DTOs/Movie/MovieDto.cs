using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.DTOs.Movie
{
    public class MovieDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LocalTitle { get; set; }
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int AgeRestriction { get; set; }
        public string Director { get; set; } = string.Empty;
        public List<string> Cast { get; set; } = new();
        public string Language { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public decimal? ImdbRating { get; set; }
        public decimal AverageRating { get; set; }
        public bool IsNowShowing { get; set; }
        public bool IsComingSoon { get; set; }
        public List<GenreDto> Genres { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class MovieListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Director { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public decimal AverageRating { get; set; }
        public bool IsNowShowing { get; set; }
        public List<string> GenreNames { get; set; } = new();
    }

    public class GenreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
