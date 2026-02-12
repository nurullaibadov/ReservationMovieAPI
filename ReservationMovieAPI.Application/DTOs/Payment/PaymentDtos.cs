using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.DTOs.Payment
{

    public class ProcessPaymentDto
    {
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string PaymentMethod { get; set; } = "CreditCard";
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
    }

    public class PaymentResultDto
    {
        public bool IsSuccess { get; set; }
        public string? TransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public decimal Amount { get; set; }
    }

    public class RefundResultDto
    {
        public bool IsSuccess { get; set; }
        public string? RefundTransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime RefundedAt { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? Last4Digits { get; set; }
        public string? CardHolderName { get; set; }
    }
}
