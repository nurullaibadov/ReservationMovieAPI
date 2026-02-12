using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Movies.Commands.CreateMovie
{
    public record CreateMovieCommand : IRequest<ApiResponse<MovieDto>>
    {
        public string Title { get; init; } = string.Empty;
        public string? LocalTitle { get; init; }
        public string Description { get; init; } = string.Empty;
        public string? LongDescription { get; init; }
        public int DurationMinutes { get; init; }
        public DateTime ReleaseDate { get; init; }
        public int AgeRestriction { get; init; } = 0;
        public string Director { get; init; } = string.Empty;
        public List<string> Cast { get; init; } = new();
        public string Language { get; init; } = "en";
        public string? SubtitleLanguage { get; init; }
        public string? TrailerUrl { get; init; }
        public decimal? ImdbRating { get; init; }
        public bool IsNowShowing { get; init; } = false;
        public bool IsComingSoon { get; init; } = true;
        public List<Guid> GenreIds { get; init; } = new();
    }
}
