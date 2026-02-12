using AutoMapper;
using ReservationMovieAPI.Application.DTOs.Movie;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Mappings
{
    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            // Movie → MovieDto
            CreateMap<Movie, MovieDto>()
                .ForMember(dest => dest.Cast,
                    opt => opt.MapFrom(src =>
                        JsonSerializer.Deserialize<List<string>>(
                            src.CastJson,
                            (JsonSerializerOptions?)null) ?? new()))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src => src.Genres));

            // Movie → MovieListDto
            CreateMap<Movie, MovieListDto>()
                .ForMember(dest => dest.GenreNames,
                    opt => opt.MapFrom(src => src.Genres.Select(g => g.Name).ToList()));

            // Genre → GenreDto
            CreateMap<Genre, GenreDto>();
        }
    }
}
