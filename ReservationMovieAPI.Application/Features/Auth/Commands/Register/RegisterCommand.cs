using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand : IRequest<ApiResponse<TokenDto>>
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public DateTime? DateOfBirth { get; init; }
    }
}
