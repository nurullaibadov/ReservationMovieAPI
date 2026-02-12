using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentDto request);
        Task<RefundResultDto> ProcessRefundAsync(
            Guid transactionId, decimal amount, string reason);
    }
}
