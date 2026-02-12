using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Movies.Queries.GetAllMovies
{
    public record GetAllMoviesQuery : IRequest<ApiResponse<PaginatedResponse<MovieListDto>>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SearchTerm { get; init; }
        public Guid? GenreId { get; init; }
        public bool? IsNowShowing { get; init; }
        public bool? IsComingSoon { get; init; }
        public string? SortBy { get; init; } = "Title";
        public bool SortDescending { get; init; } = false;
    }
}
