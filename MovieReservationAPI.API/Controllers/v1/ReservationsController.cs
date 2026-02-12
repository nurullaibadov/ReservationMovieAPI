using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.Features.Reservations.Commands.CancelReservation;
using ReservationMovieAPI.Application.Features.Reservations.Commands.CreateReservation;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize] // Tüm endpoint'ler giriş gerektirir
    public class ReservationsController : BaseController
    {
        /// <summary>Kullanıcının rezervasyonları.</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(new GetUserReservationsQuery
            {
                UserId = CurrentUserId,
                PageNumber = page,
                PageSize = pageSize
            }, ct);
            return Ok(result);
        }

        /// <summary>Rezervasyon detayı.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(new GetReservationByIdQuery
            {
                ReservationId = id,
                UserId = CurrentUserId,
                IsAdmin = IsAdmin
            }, ct);
            return Ok(result);
        }

        /// <summary>Yeni rezervasyon oluştur.</summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReservationRequest request,
            CancellationToken ct)
        {
            var command = new CreateReservationCommand
            {
                ScreeningId = request.ScreeningId,
                SeatIds = request.SeatIds,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(command, ct);
            return result.Success
                ? Created(string.Empty, result)
                : BadRequest(result);
        }

        /// <summary>Rezervasyon iptal et.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(
            Guid id,
            [FromBody] CancelReservationRequest? request,
            CancellationToken ct)
        {
            var command = new CancelReservationCommand
            {
                ReservationId = id,
                UserId = CurrentUserId,
                IsAdmin = IsAdmin,
                CancellationNote = request?.Note
            };

            var result = await Mediator.Send(command, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Tüm rezervasyonlar. [Admin]</summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(new GetAllReservationsQuery
            {
                PageNumber = page,
                PageSize = pageSize
            }, ct);
            return Ok(result);
        }
    }

    public record CreateReservationRequest(
        Guid ScreeningId,
        List<Guid> SeatIds);

    public record CancelReservationRequest(string? Note);
}
