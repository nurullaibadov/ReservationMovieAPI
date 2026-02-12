using AutoMapper;
using ReservationMovieAPI.Application.DTOs.Reservation;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Mappings
{
    public class ReservationMappingProfile : Profile
    {
        public ReservationMappingProfile()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.MovieTitle,
                    opt => opt.MapFrom(src => src.Screening.Movie.Title))
                .ForMember(dest => dest.CinemaName,
                    opt => opt.MapFrom(src => src.Screening.Hall.Cinema.Name))
                .ForMember(dest => dest.HallName,
                    opt => opt.MapFrom(src => src.Screening.Hall.Name))
                .ForMember(dest => dest.ScreeningStartTime,
                    opt => opt.MapFrom(src => src.Screening.StartTime))
                .ForMember(dest => dest.Seats,
                    opt => opt.MapFrom(src => src.ReservationSeats));

            CreateMap<ReservationSeat, SeatInfoDto>()
                .ForMember(dest => dest.RowLabel,
                    opt => opt.MapFrom(src => src.Seat.RowLabel))
                .ForMember(dest => dest.SeatNumber,
                    opt => opt.MapFrom(src => src.Seat.SeatNumber))
                .ForMember(dest => dest.SeatType,
                    opt => opt.MapFrom(src => src.Seat.SeatType));
        }
    }
}
