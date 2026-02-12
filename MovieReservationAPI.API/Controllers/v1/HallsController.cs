using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Domain.Entities;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HallsController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public HallsController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        /// <summary>Sinemaya ait salonlar.</summary>
        [HttpGet("by-cinema/{cinemaId:guid}")]
        public async Task<IActionResult> GetByCinema(
            Guid cinemaId, CancellationToken ct)
        {
            var halls = await _uow.Halls.GetAsync(
                h => h.CinemaId == cinemaId && h.IsActive,
                ct: ct);

            var dtos = _mapper.Map<List<HallDto>>(halls);
            return Ok(ApiResponse<List<HallDto>>.SuccessResult(dtos));
        }

        /// <summary>Yeni salon oluştur. [Admin]</summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(
            [FromBody] CreateHallDto dto,
            CancellationToken ct)
        {
            var hall = _mapper.Map<Hall>(dto);
            await _uow.Halls.AddAsync(hall, ct);

            // Koltukları otomatik oluştur
            var seats = new List<Seat>();
            for (int row = 0; row < dto.Rows; row++)
            {
                for (int seatNum = 1; seatNum <= dto.SeatsPerRow; seatNum++)
                {
                    seats.Add(new Seat
                    {
                        HallId = hall.Id,
                        RowLabel = ((char)('A' + row)).ToString(),
                        SeatNumber = seatNum,
                        SeatType = Domain.Enums.SeatType.Standard,
                        IsActive = true
                    });
                }
            }

            await _uow.Seats.AddRangeAsync(seats, ct);
            hall.TotalSeats = seats.Count;
            _uow.Halls.Update(hall);

            await _uow.SaveChangesAsync(ct);

            var result = _mapper.Map<HallDto>(hall);
            return Ok(ApiResponse<HallDto>.SuccessResult(
                result, $"Salon oluşturuldu ve {seats.Count} koltuk eklendi."));
        }
    }
}
