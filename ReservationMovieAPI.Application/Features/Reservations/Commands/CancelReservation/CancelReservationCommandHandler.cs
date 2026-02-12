using MediatR;
using Microsoft.Extensions.Logging;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Application.Interfaces.Services;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Enums;
using ReservationMovieAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Reservations.Commands.CancelReservation
{
    public class CancelReservationCommandHandler
      : IRequestHandler<CancelReservationCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;
        private readonly ILogger<CancelReservationCommandHandler> _logger;

        public CancelReservationCommandHandler(
            IUnitOfWork uow, IEmailService emailService,
            ILogger<CancelReservationCommandHandler> logger)
        {
            _uow = uow;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(
            CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _uow.Reservations.GetWithDetailsAsync(
                request.ReservationId, cancellationToken);

            if (reservation == null)
                throw new NotFoundException(
                    nameof(Reservation), request.ReservationId);

            // Admin değilse kendi rezervasyonunu iptal edebilir
            if (!request.IsAdmin && reservation.UserId != request.UserId)
                throw new UnauthorizedAccessException(
                    "Bu rezervasyonu iptal etme yetkiniz yok.");

            // İptal edilebilir durumlar
            if (reservation.Status == ReservationStatus.Cancelled)
                throw new BusinessRuleException("Rezervasyon zaten iptal edilmiş.");

            if (reservation.Status == ReservationStatus.Completed)
                throw new BusinessRuleException(
                    "Tamamlanmış rezervasyonlar iptal edilemez.");

            // Seans başlamışsa iptal edilemez
            if (reservation.Screening.StartTime <= DateTime.UtcNow)
                throw new BusinessRuleException(
                    "Seans başladıktan sonra iptal yapılamaz.");

            await _uow.BeginTransactionAsync(cancellationToken);
            try
            {
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancellationNote = request.CancellationNote;
                reservation.CancelledAt = DateTime.UtcNow;

                // Koltukları serbest bırak
                foreach (var rs in reservation.ReservationSeats)
                    rs.Status = SeatStatus.Available;

                // Müsait koltuk sayısını artır
                var screening = await _uow.Screenings.GetByIdAsync(
                    reservation.ScreeningId, cancellationToken);
                if (screening != null)
                {
                    screening.AvailableSeats +=
                        reservation.ReservationSeats.Count;
                    _uow.Screenings.Update(screening);
                }

                _uow.Reservations.Update(reservation);
                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);

                // İade işlemi (ödeme yapılmışsa)
                if (reservation.Payment?.Status == PaymentStatus.Completed &&
                    !string.IsNullOrEmpty(reservation.Payment.TransactionId))
                {
                    _logger.LogInformation(
                        "Refund needed for reservation {Code}",
                        reservation.ReservationCode);
                }

                // İptal emaili
                _ = Task.Run(async () =>
                    await _emailService.SendReservationCancellationAsync(
                        reservation.User.Email!,
                        reservation.User.FullName,
                        reservation.ReservationCode), cancellationToken);

                _logger.LogInformation(
                    "Reservation cancelled: {Code}", reservation.ReservationCode);

                return ApiResponse<bool>.SuccessResult(
                    true, "Rezervasyon başarıyla iptal edildi.");
            }
            catch
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
