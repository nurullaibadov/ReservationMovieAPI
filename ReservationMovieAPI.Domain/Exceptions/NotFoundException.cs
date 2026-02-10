using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        /// <param name="entityName">Bulunamayan entity'nin adı (örn: "Movie")</param>
        /// <param name="key">Aranan anahtar değer (örn: Guid)</param>
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.", "NOT_FOUND")
        {
        }
    }
}
