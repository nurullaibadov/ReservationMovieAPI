using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Movies.Commands.CreateMovie
{
    public class CreateMovieCommandHandler
        : IRequestHandler<CreateMovieCommand, ApiResponse<MovieDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateMovieCommandHandler> _logger;

        public CreateMovieCommandHandler(
            IUnitOfWork uow, IMapper mapper,
            ILogger<CreateMovieCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<MovieDto>> Handle(
            CreateMovieCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating movie: {Title}", request.Title);

            // 1. Aynı isimde film var mı kontrol et
            var exists = await _uow.Movies.ExistsAsync(
                m => m.Title == request.Title &&
                     m.ReleaseDate.Year == request.ReleaseDate.Year,
                cancellationToken);

            if (exists)
                throw new BusinessRuleException(
                    $"'{request.Title}' adlı film {request.ReleaseDate.Year} yılı için zaten mevcut.");

            // 2. Genre'lerin varlığını kontrol et
            var genres = new List<Genre>();
            foreach (var genreId in request.GenreIds)
            {
                var genre = await _uow.Genres.GetByIdAsync(genreId, cancellationToken);
                if (genre == null)
                    throw new NotFoundException(nameof(Genre), genreId);
                genres.Add(genre);
            }

            // 3. Entity oluştur
            var movie = new Movie
            {
                Title = request.Title,
                LocalTitle = request.LocalTitle,
                Description = request.Description,
                LongDescription = request.LongDescription,
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                AgeRestriction = request.AgeRestriction,
                Director = request.Director,
                CastJson = JsonSerializer.Serialize(request.Cast),
                Language = request.Language,
                SubtitleLanguage = request.SubtitleLanguage,
                TrailerUrl = request.TrailerUrl,
                ImdbRating = request.ImdbRating,
                IsNowShowing = request.IsNowShowing,
                IsComingSoon = request.IsComingSoon,
                Genres = genres
            };

            // 4. Kaydet
            await _uow.Movies.AddAsync(movie, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Movie created: {MovieId}", movie.Id);

            // 5. DTO'ya çevir ve döndür
            var dto = _mapper.Map<MovieDto>(movie);
            return ApiResponse<MovieDto>.SuccessResult(dto, "Film başarıyla oluşturuldu.");
        }
    }
}
