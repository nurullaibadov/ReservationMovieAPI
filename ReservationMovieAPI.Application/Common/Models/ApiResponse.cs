using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string message = "Operation successful")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResult(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };

        public static ApiResponse<T> FailureResult(string message, string error)
            => new() { Success = false, Message = message, Errors = new List<string> { error } };
    }

    /// <summary>
    /// Generic olmayan versiyon — sadece mesaj döndürmek için.
    /// Örnek: Delete işlemi sonucu.
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string message = "Operation successful")
            => new() { Success = true, Message = message };

        public new static ApiResponse FailureResult(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }
}
