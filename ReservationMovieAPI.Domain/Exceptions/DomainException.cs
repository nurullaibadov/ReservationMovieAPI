using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Exceptions
{
    /// </summary>
    public class DomainException : Exception
    {
        public string ErrorCode { get; }

        public DomainException(string message, string errorCode = "DOMAIN_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DomainException(string message, Exception innerException,
            string errorCode = "DOMAIN_ERROR")
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
