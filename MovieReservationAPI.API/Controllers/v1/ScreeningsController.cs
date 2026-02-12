using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ScreeningsController : BaseController
    {
        /// <summary>Filme göre seanslar.</summary>
        [HttpGet("by-movie/{movieId:guid}")]
        public async Task<IActionResult> GetByMovie(
            Guid movieId,
            [FromQuery] DateTime? fromDate,
            CancellationToken ct)
        {
            var result = await Mediator.Send(new GetScreeningsByMovieQuery
            {
                MovieId = movieId,
                FromDate = fromDate ?? DateTime.UtcNow
            }, ct);
            return Ok(result);
        }

        /// <summary>Seanstaki müsait koltuklar.</summary>
        [HttpGet("{id:guid}/available-seats")]
        public async Task<IActionResult> GetAvailableSeats(
            Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new GetAvailableSeatsQuery { ScreeningId = id }, ct);
            return Ok(result);
        }

        /// <summary>Yeni seans oluştur. [Admin]</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            [FromBody] CreateScreeningCommand command,
            CancellationToken ct)
        {
            var result = await Mediator.Send(command, ct);
            return result.Success
                ? Created(string.Empty, result)
                : BadRequest(result);
        }

        /// <summary>Seans iptal et. [Admin]</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Cancel(
            Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new CancelScreeningCommand { ScreeningId = id }, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
