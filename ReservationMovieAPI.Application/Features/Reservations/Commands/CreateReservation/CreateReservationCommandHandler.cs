using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Enums;
using ReservationMovieAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommandHandler
      : IRequestHandler<CreateReservationCommand, ApiResponse<ReservationDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateReservationCommandHandler> _logger;

        public CreateReservationCommandHandler(
            IUnitOfWork uow, IMapper mapper,
            ILogger<CreateReservationCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<ReservationDto>> Handle(
            CreateReservationCommand request, CancellationToken cancellationToken)
        {
            // ── Adım 1: Seansı getir ve kontrol et ──────────────────────────────
            var screening = await _uow.Screenings.GetScreeningWithDetailsAsync(
                request.ScreeningId, cancellationToken);

            if (screening == null)
                throw new NotFoundException(nameof(Screening), request.ScreeningId);

            if (!screening.IsActive)
                throw new BusinessRuleException("Bu seans aktif değil.");

            if (screening.StartTime <= DateTime.UtcNow)
                throw new BusinessRuleException("Geçmiş tarihli seanslar için rezervasyon yapılamaz.");

            if (screening.AvailableSeats < request.SeatIds.Count)
                throw new BusinessRuleException(
                    $"Yeterli koltuk yok. Müsait koltuk: {screening.AvailableSeats}");

            // ── Adım 2: Koltukların müsait olduğunu kontrol et (Race Condition) ─
            // Transaction + RowVersion ile eş zamanlı çakışma önlenir
            await _uow.BeginTransactionAsync(cancellationToken);

            try
            {
                var reservedSeatIds = await _uow.Reservations
                    .GetReservedSeatIdsAsync(request.ScreeningId, cancellationToken);

                var conflictingSeatIds = request.SeatIds
                    .Intersect(reservedSeatIds)
                    .ToList();

                if (conflictingSeatIds.Any())
                    throw new BusinessRuleException(
                        $"Seçilen koltukların bir kısmı zaten rezerve edilmiş.");

                // ── Adım 3: Koltukları getir ve fiyatları hesapla ───────────────
                var seats = await _uow.Seats.GetAsync(
                    predicate: s => request.SeatIds.Contains(s.Id),
                    cancellationToken: cancellationToken);

                if (seats.Count != request.SeatIds.Count)
                    throw new BusinessRuleException("Geçersiz koltuk seçimi.");

                decimal totalAmount = 0;
                var reservationSeats = new List<ReservationSeat>();

                foreach (var seat in seats)
                {
                    // Koltuk tipine göre fiyat belirle
                    var price = seat.SeatType switch
                    {
                        SeatType.VIP => screening.VipPrice,
                        SeatType.Premium => screening.PremiumPrice,
                        _ => screening.StandardPrice
                    };

                    totalAmount += price;

                    reservationSeats.Add(new ReservationSeat
                    {
                        SeatId = seat.Id,
                        Price = price,
                        Status = SeatStatus.Reserved
                    });
                }

                // ── Adım 4: Rezervasyon kodu oluştur ────────────────────────────
                var code = GenerateReservationCode();

                // ── Adım 5: Rezervasyonu kaydet ──────────────────────────────────
                var reservation = new Reservation
                {
                    UserId = request.UserId,
                    ScreeningId = request.ScreeningId,
                    ReservationCode = code,
                    Status = ReservationStatus.Pending,
                    TotalAmount = totalAmount,
                    DiscountAmount = 0,
                    NetAmount = totalAmount,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    ReservationSeats = reservationSeats
                };

                await _uow.Reservations.AddAsync(reservation, cancellationToken);

                // ── Adım 6: Müsait koltuk sayısını güncelle ──────────────────────
                screening.AvailableSeats -= request.SeatIds.Count;
                _uow.Screenings.Update(screening);

                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Reservation created: {Code} for user {UserId}",
                    code, request.UserId);

                var dto = _mapper.Map<ReservationDto>(reservation);
                return ApiResponse<ReservationDto>.SuccessResult(
                    dto, "Rezervasyon oluşturuldu. 10 dakika içinde ödeme yapınız.");
            }
            catch
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private static string GenerateReservationCode()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"MRS-{datePart}-{randomPart}";
        }
    }
}
