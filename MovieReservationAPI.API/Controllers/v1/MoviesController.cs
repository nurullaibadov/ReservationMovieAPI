using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.Features.Movies.Commands.CreateMovie;
using ReservationMovieAPI.Application.Features.Movies.Queries.GetAllMovies;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MoviesController : BaseController
    {
        /// <summary>
        /// Tüm filmleri sayfalı olarak listeler.
        /// Filtreleme: searchTerm, genreId, isNowShowing, isComingSoon
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetAllMoviesQuery query,
            CancellationToken ct)
        {
            var result = await Mediator.Send(query, ct);
            return Ok(result);
        }

        /// <summary>ID ile film detayını getirir.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new GetMovieByIdQuery { MovieId = id }, ct);
            return Ok(result);
        }

        /// <summary>Film arama.</summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(new SearchMoviesQuery
            {
                SearchTerm = term,
                PageNumber = page,
                PageSize = pageSize
            }, ct);
            return Ok(result);
        }

        /// <summary>Vizyondaki filmler.</summary>
        [HttpGet("now-showing")]
        public async Task<IActionResult> NowShowing(CancellationToken ct)
        {
            var result = await Mediator.Send(
                new GetAllMoviesQuery { IsNowShowing = true }, ct);
            return Ok(result);
        }

        /// <summary>Yakında gelecek filmler.</summary>
        [HttpGet("coming-soon")]
        public async Task<IActionResult> ComingSoon(CancellationToken ct)
        {
            var result = await Mediator.Send(
                new GetAllMoviesQuery { IsComingSoon = true }, ct);
            return Ok(result);
        }

        /// <summary>Yeni film oluştur. [Admin]</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieCommand command,
            CancellationToken ct)
        {
            var result = await Mediator.Send(command, ct);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result);
        }

        /// <summary>Film güncelle. [Admin]</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateMovieCommand command,
            CancellationToken ct)
        {
            var updatedCommand = command with { MovieId = id };
            var result = await Mediator.Send(updatedCommand, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Film sil (soft delete). [Admin]</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(
            Guid id, CancellationToken ct)
        {
            var result = await Mediator.Send(
                new DeleteMovieCommand { MovieId = id }, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Film posteri yükle. [Admin]</summary>
        [HttpPost("{id:guid}/poster")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadPoster(
            Guid id,
            IFormFile file,
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya boş olamaz.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Sadece JPEG, PNG ve WebP desteklenir.");

            if (file.Length > 5 * 1024 * 1024) // 5MB
                return BadRequest("Dosya boyutu 5MB'ı geçemez.");

            using var stream = file.OpenReadStream();
            var result = await Mediator.Send(new UploadMoviePosterCommand
            {
                MovieId = id,
                FileStream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType
            }, ct);

            return Ok(result);
        }
    }
}
