using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResultDto> ProcessPaymentAsync(
            ProcessPaymentDto request)
        {
            _logger.LogInformation(
                "Processing payment for reservation {ReservationId}, " +
                "Amount: {Amount}",
                request.ReservationId, request.Amount);

            // Gerçek ödeme gateway entegrasyonu burada olur.
            // Simülasyon: Test kart numaraları
            await Task.Delay(500); // Gateway gecikme simülasyonu

            // Test kartları:
            // 4242424242424242 → Başarılı
            // 4000000000000002 → Başarısız
            var isSuccess = !request.CardNumber.EndsWith("0002");

            if (isSuccess)
            {
                _logger.LogInformation(
                    "Payment successful for reservation {ReservationId}",
                    request.ReservationId);

                return new PaymentResultDto
                {
                    IsSuccess = true,
                    TransactionId = $"TXN-{Guid.NewGuid():N}"[..20].ToUpper(),
                    Message = "Ödeme başarıyla tamamlandı.",
                    ProcessedAt = DateTime.UtcNow,
                    Amount = request.Amount
                };
            }

            _logger.LogWarning(
                "Payment failed for reservation {ReservationId}",
                request.ReservationId);

            return new PaymentResultDto
            {
                IsSuccess = false,
                TransactionId = null,
                Message = "Ödeme işlemi başarısız. Kart bilgilerinizi kontrol edin.",
                ProcessedAt = DateTime.UtcNow,
                Amount = request.Amount
            };
        }

        public async Task<RefundResultDto> ProcessRefundAsync(
            Guid transactionId, decimal amount, string reason)
        {
            _logger.LogInformation(
                "Processing refund for transaction {TransactionId}", transactionId);

            await Task.Delay(300);

            return new RefundResultDto
            {
                IsSuccess = true,
                RefundTransactionId = $"REF-{Guid.NewGuid():N}"[..15].ToUpper(),
                Message = "İade başarıyla işlendi.",
                RefundedAt = DateTime.UtcNow,
                Amount = amount
            };
        }
    }
}
