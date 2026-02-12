using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MovieReservationAPI.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        private ISender? _mediator;

        /// <summary>
        /// MediatR sender — constructor injection yerine
        /// property injection: alt sınıflar constructor'a
        /// parametre eklemek zorunda kalmaz.
        /// </summary>
        protected ISender Mediator =>
            _mediator ??= HttpContext.RequestServices
                .GetRequiredService<ISender>();

        /// <summary>JWT token'dan kullanıcı ID'si.</summary>
        protected string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>JWT token'dan kullanıcı email'i.</summary>
        protected string CurrentUserEmail =>
            User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        /// <summary>Kullanıcı admin mi?</summary>
        protected bool IsAdmin =>
            User.IsInRole("Admin");

        /// <summary>Standart 200 OK cevabı.</summary>
        protected IActionResult OkResult<T>(T data, string message = "Success")
            => Ok(new
            {
                success = true,
                message,
                data,
                timestamp = DateTime.UtcNow
            });
    }
}
