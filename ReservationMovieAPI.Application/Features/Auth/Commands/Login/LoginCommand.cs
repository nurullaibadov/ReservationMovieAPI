using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : IRequest<ApiResponse<TokenDto>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
