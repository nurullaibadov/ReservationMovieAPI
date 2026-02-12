using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CinemasController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CinemasController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        /// <summary>Tüm sinemalar.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? city,
            CancellationToken ct)
        {
            var cinemas = string.IsNullOrEmpty(city)
                ? await _uow.Cinemas.GetAllAsync(ct)
                : await _uow.Cinemas.GetAsync(
                    c => c.City == city &&
                         c.IsActive, ct: ct);

            var dtos = _mapper.Map<List<CinemaDto>>(cinemas);
            return Ok(ApiResponse<List<CinemaDto>>.SuccessResult(dtos));
        }

        /// <summary>ID ile sinema detayı (salonlar dahil).</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id, CancellationToken ct)
        {
            var cinema = await _uow.Cinemas.GetByIdAsync(
                id,
                q => q.Include(c => c.Halls),
                ct);

            if (cinema == null)
                return NotFound(ApiResponse.FailureResult(
                    "Sinema bulunamadı."));

            var dto = _mapper.Map<CinemaDetailDto>(cinema);
            return Ok(ApiResponse<CinemaDetailDto>.SuccessResult(dto));
        }

        /// <summary>Yeni sinema oluştur. [Admin]</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCinemaDto dto,
            CancellationToken ct)
        {
            var cinema = _mapper.Map<Domain.Entities.Cinema>(dto);
            await _uow.Cinemas.AddAsync(cinema, ct);
            await _uow.SaveChangesAsync(ct);

            var result = _mapper.Map<CinemaDto>(cinema);
            return CreatedAtAction(nameof(GetById),
                new { id = cinema.Id },
                ApiResponse<CinemaDto>.SuccessResult(result));
        }
    }

}
