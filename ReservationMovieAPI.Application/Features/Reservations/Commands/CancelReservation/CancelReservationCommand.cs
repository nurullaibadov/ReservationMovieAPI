using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Reservations.Commands.CancelReservation
{
    public record CancelReservationCommand : IRequest<ApiResponse<bool>>
    {
        public Guid ReservationId { get; init; }
        public string UserId { get; init; } = string.Empty;
        public bool IsAdmin { get; init; } = false;
        public string? CancellationNote { get; init; }
    }
}
