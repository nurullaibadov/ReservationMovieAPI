using ReservationMovieAPI.Domain.Entities.Common;
using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Payment : BaseEntity
    {
        /// <summary>Ödemenin ait olduğu rezervasyon.</summary>
        public Guid ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; } = null!;

        /// <summary>Ödeme tutarı.</summary>
        public decimal Amount { get; set; }

        /// <summary>Para birimi (ISO 4217: "TRY", "USD", "EUR").</summary>
        public string Currency { get; set; } = "TRY";

        /// <summary>Ödeme yöntemi (CreditCard, DebitCard, Online).</summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>Ödeme durumu.</summary>
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// Ödeme gateway'inden dönen transaction ID.
        /// Gerçek entegrasyonda iPayTR, Stripe vb. kullanılır.
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>Gateway'den dönen ham cevap (JSON).</summary>
        public string? GatewayResponse { get; set; }

        /// <summary>Ödeme tarihi.</summary>
        public DateTime? PaidAt { get; set; }

        /// <summary>İade tutarı (kısmi iade için).</summary>
        public decimal? RefundAmount { get; set; }

        /// <summary>İade tarihi.</summary>
        public DateTime? RefundedAt { get; set; }

        /// <summary>İade nedeni.</summary>
        public string? RefundReason { get; set; }

        // Kart bilgileri (maskelenmiş — güvenlik için)
        /// <summary>Kart sahibi adı.</summary>
        public string? CardHolderName { get; set; }

        /// <summary>Maskelenmiş kart numarasının son 4 hanesi.</summary>
        public string? Last4Digits { get; set; }
    }
}
