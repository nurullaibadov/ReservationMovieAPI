using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Reservations.Commands.CreateReservation
{

    public record CreateReservationCommand : IRequest<ApiResponse<ReservationDto>>
    {
        /// <summary>Hangi seans için rezervasyon.</summary>
        public Guid ScreeningId { get; init; }

        /// <summary>Seçilen koltuk ID'leri.</summary>
        public List<Guid> SeatIds { get; init; } = new();

        /// <summary>Rezervasyonu yapan kullanıcı (JWT'den alınır).</summary>
        public string UserId { get; init; } = string.Empty;
    }
}
