using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationMovieAPI.Application.Features.Payments.Commands.ProcessPayment;

namespace MovieReservationAPI.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class PaymentsController : BaseController
    {
        /// <summary>Ödeme işlemi yap.</summary>
        [HttpPost("process")]
        public async Task<IActionResult> Process(
            [FromBody] ProcessPaymentRequest request,
            CancellationToken ct)
        {
            var command = new ProcessPaymentCommand
            {
                ReservationId = request.ReservationId,
                PaymentMethod = request.PaymentMethod,
                CardNumber = request.CardNumber,
                CardHolderName = request.CardHolderName,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                Cvv = request.Cvv,
                UserId = CurrentUserId
            };

            var result = await Mediator.Send(command, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Ödeme geçmişi.</summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(new GetPaymentHistoryQuery
            {
                UserId = CurrentUserId,
                PageNumber = page,
                PageSize = pageSize
            }, ct);
            return Ok(result);
        }
    }

    public record ProcessPaymentRequest(
        Guid ReservationId,
        string PaymentMethod,
        string CardNumber,
        string CardHolderName,
        string ExpiryMonth,
        string ExpiryYear,
        string Cvv);
}
