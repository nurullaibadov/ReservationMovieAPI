using FluentValidation;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace MovieReservationAPI.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(exception,
                "Unhandled exception. TraceId: {TraceId}, " +
                "Path: {Path}, Method: {Method}",
                traceId, context.Request.Path,
                context.Request.Method);

            var (statusCode, response) = exception switch
            {
                NotFoundException ex => (
                    HttpStatusCode.NotFound,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = new List<string> { ex.Message },
                        TraceId = traceId
                    }),

                BusinessRuleException ex => (
                    HttpStatusCode.UnprocessableEntity,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = new List<string> { ex.Message },
                        TraceId = traceId
                    }),

                ValidationException ex => (
                    HttpStatusCode.BadRequest,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation hatası.",
                        Errors = ex.Errors
                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                            .ToList(),
                        TraceId = traceId
                    }),

                UnauthorizedAccessException ex => (
                    HttpStatusCode.Unauthorized,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = new List<string> { ex.Message },
                        TraceId = traceId
                    }),

                _ => (
                    HttpStatusCode.InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Beklenmedik bir hata oluştu.",
                        Errors = new List<string>
                        {
                        // Production'da detay gösterme
                        "Lütfen daha sonra tekrar deneyin."
                        },
                        TraceId = traceId
                    })
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
