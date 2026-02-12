using AutoMapper;
using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Movies.Queries.GetAllMovies
{
    public class GetAllMoviesQueryHandler
       : IRequestHandler<GetAllMoviesQuery,
           ApiResponse<PaginatedResponse<MovieListDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllMoviesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<MovieListDto>>> Handle(
            GetAllMoviesQuery request, CancellationToken cancellationToken)
        {
            // Dinamik filtre oluştur
            System.Linq.Expressions.Expression<Func<Movie, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                predicate = m => m.Title.ToLower().Contains(term) ||
                                 m.Director.ToLower().Contains(term);
            }

            if (request.IsNowShowing.HasValue)
            {
                var isNow = request.IsNowShowing.Value;
                predicate = predicate == null
                    ? m => m.IsNowShowing == isNow
                    : predicate.And(m => m.IsNowShowing == isNow);
            }

            if (request.IsComingSoon.HasValue)
            {
                var isSoon = request.IsComingSoon.Value;
                predicate = predicate == null
                    ? m => m.IsComingSoon == isSoon
                    : predicate.And(m => m.IsComingSoon == isSoon);
            }

            // Sıralama
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>> orderBy =
                request.SortBy?.ToLower() switch
                {
                    "releasedate" => request.SortDescending
                    ? q => q.OrderByDescending(m => m.ReleaseDate)
                    : q => q.OrderBy(m => m.ReleaseDate),
                    "rating" => request.SortDescending
                    ? q => q.OrderByDescending(m => m.AverageRating)
                    : q => q.OrderBy(m => m.AverageRating),
                    _ => request.SortDescending
                    ? q => q.OrderByDescending(m => m.Title)
                    : q => q.OrderBy(m => m.Title)
                };

            var (items, totalCount) = await _uow.Movies.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                predicate: predicate,
                orderBy: orderBy,
                includes: q => q.Include(m => m.Genres),
                cancellationToken);

            var dtos = _mapper.Map<List<MovieListDto>>(items);

            var paged = PaginatedResponse<MovieListDto>.Create(
                dtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponse<PaginatedResponse<MovieListDto>>.SuccessResult(paged);
        }
    }
}
