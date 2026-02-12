using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.DTOs.Reservation
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public ReservationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public DateTime ScreeningStartTime { get; set; }

        public List<SeatInfoDto> Seats { get; set; } = new();
    }

    public class SeatInfoDto
    {
        public Guid SeatId { get; set; }
        public string RowLabel { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public SeatType SeatType { get; set; }
        public decimal Price { get; set; }
    }
}
