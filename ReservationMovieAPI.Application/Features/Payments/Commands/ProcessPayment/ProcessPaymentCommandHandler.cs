using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.DTOs.Payment;
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

namespace ReservationMovieAPI.Application.Features.Payments.Commands.ProcessPayment
{
    public class ProcessPaymentCommandHandler
        : IRequestHandler<ProcessPaymentCommand, ApiResponse<PaymentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;

        public ProcessPaymentCommandHandler(
            IUnitOfWork uow, IPaymentService paymentService,
            IEmailService emailService, IMapper mapper,
            ILogger<ProcessPaymentCommandHandler> logger)
        {
            _uow = uow;
            _paymentService = paymentService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<PaymentDto>> Handle(
            ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // 1. Rezervasyonu getir
            var reservation = await _uow.Reservations.GetWithDetailsAsync(
                request.ReservationId, cancellationToken);

            if (reservation == null)
                throw new NotFoundException(
                    nameof(Reservation), request.ReservationId);

            // 2. Yetki kontrolü
            if (reservation.UserId != request.UserId)
                throw new UnauthorizedAccessException(
                    "Bu rezervasyona erişim yetkiniz yok.");

            // 3. Durum kontrolü
            if (reservation.Status != ReservationStatus.Pending)
                throw new BusinessRuleException(
                    $"Ödeme yapılamaz. Rezervasyon durumu: {reservation.Status}");

            // 4. Süre dolmuş mu?
            if (reservation.ExpiresAt < DateTime.UtcNow)
            {
                reservation.Status = ReservationStatus.Expired;
                _uow.Reservations.Update(reservation);
                await _uow.SaveChangesAsync(cancellationToken);
                throw new BusinessRuleException(
                    "Rezervasyon süresi dolmuş. Lütfen yeni rezervasyon yapın.");
            }

            // 5. Ödeme işlemi
            await _uow.BeginTransactionAsync(cancellationToken);

            try
            {
                var paymentRequest = new ProcessPaymentDto
                {
                    ReservationId = reservation.Id,
                    Amount = reservation.NetAmount,
                    Currency = "TRY",
                    PaymentMethod = request.PaymentMethod,
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName,
                    ExpiryMonth = request.ExpiryMonth,
                    ExpiryYear = request.ExpiryYear,
                    Cvv = request.Cvv
                };

                var paymentResult = await _paymentService
                    .ProcessPaymentAsync(paymentRequest);

                var payment = new Payment
                {
                    ReservationId = reservation.Id,
                    Amount = reservation.NetAmount,
                    Currency = "TRY",
                    PaymentMethod = request.PaymentMethod,
                    Status = paymentResult.IsSuccess
                        ? PaymentStatus.Completed
                        : PaymentStatus.Failed,
                    TransactionId = paymentResult.TransactionId,
                    PaidAt = paymentResult.IsSuccess
                        ? DateTime.UtcNow : null,
                    CardHolderName = request.CardHolderName,
                    Last4Digits = request.CardNumber.Length >= 4
                        ? request.CardNumber[^4..] : null,
                    GatewayResponse = paymentResult.Message
                };

                await _uow.Payments.AddAsync(payment, cancellationToken);

                if (paymentResult.IsSuccess)
                {
                    // 6. Rezervasyon durumunu güncelle
                    reservation.Status = ReservationStatus.Confirmed;
                    _uow.Reservations.Update(reservation);

                    // 7. Koltuk durumlarını güncelle
                    foreach (var rs in reservation.ReservationSeats)
                    {
                        rs.Status = SeatStatus.Occupied;
                    }

                    await _uow.SaveChangesAsync(cancellationToken);
                    await _uow.CommitTransactionAsync(cancellationToken);

                    // 8. Onay emaili gönder (async, hata olursa sistemi durdurma)
                    _ = Task.Run(async () =>
                    {
                        var seatLabels = reservation.ReservationSeats
                            .Select(rs =>
                                $"{rs.Seat.RowLabel}{rs.Seat.SeatNumber}")
                            .ToList();

                        await _emailService.SendReservationConfirmationAsync(
                            reservation.User.Email!,
                            reservation.User.FullName,
                            reservation.ReservationCode,
                            reservation.Screening.Movie.Title,
                            reservation.Screening.StartTime,
                            reservation.Screening.Hall.Name,
                            seatLabels,
                            reservation.NetAmount);
                    }, cancellationToken);

                    var dto = _mapper.Map<PaymentDto>(payment);
                    return ApiResponse<PaymentDto>.SuccessResult(
                        dto, "Ödeme başarılı. Rezervasyonunuz onaylandı!");
                }
                else
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    await _uow.CommitTransactionAsync(cancellationToken);

                    return ApiResponse<PaymentDto>.FailureResult(
                        paymentResult.Message);
                }
            }
            catch
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
