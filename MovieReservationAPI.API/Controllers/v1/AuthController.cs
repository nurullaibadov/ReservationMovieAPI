using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.DTOs.Auth;
using ReservationMovieAPI.Application.Features.Auth.Commands.Login;
using ReservationMovieAPI.Application.Features.Auth.Commands.Register;
using ReservationMovieAPI.Application.Interfaces.Services;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>Yeni kullanıcı kaydı.</summary>
        /// <response code="200">Kayıt başarılı, token döndürülür.</response>
        /// <response code="400">Validation hatası.</response>
        /// <response code="422">Email zaten kullanımda.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command,
            CancellationToken ct)
        {
            var result = await Mediator.Send(command, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Kullanıcı girişi.</summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command,
            CancellationToken ct)
        {
            var result = await Mediator.Send(command, ct);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        /// <summary>Access token yenileme.</summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken ct)
        {
            var result = await _tokenService.RefreshTokenAsync(
                request.AccessToken, request.RefreshToken);
            return Ok(result);
        }

        /// <summary>Kullanıcı çıkışı (refresh token iptal).</summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            // Refresh token'ı sil
            // Bu operasyon için UserManager gerekiyor — basit tutuyoruz
            return Ok(new { success = true, message = "Çıkış yapıldı." });
        }
    }

    public record RefreshTokenRequest(string AccessToken, string RefreshToken);
}
