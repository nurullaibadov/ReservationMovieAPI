using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Services
{
    public interface ITokenService
    {
        /// <summary>Access token + Refresh token üretir.</summary>
        Task<TokenDto> GenerateTokensAsync(ApplicationUser user);

        /// <summary>Refresh token ile yeni access token üretir.</summary>
        Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken);

        /// <summary>Access token'dan claim'leri parse eder.</summary>
        System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(
            string token);
    }
}
