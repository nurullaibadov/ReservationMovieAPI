using MediatR;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Payments.Commands.ProcessPayment
{
    public record ProcessPaymentCommand : IRequest<ApiResponse<PaymentDto>>
    {
        public Guid ReservationId { get; init; }
        public string PaymentMethod { get; init; } = "CreditCard";
        public string CardNumber { get; init; } = string.Empty;
        public string CardHolderName { get; init; } = string.Empty;
        public string ExpiryMonth { get; init; } = string.Empty;
        public string ExpiryYear { get; init; } = string.Empty;
        public string Cvv { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
    }
}
