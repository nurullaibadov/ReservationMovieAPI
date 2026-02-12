using ReservationMovieAPI.Domain.Entities.Common;
using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class ReservationSeat : BaseEntity
    {
        /// <summary>Hangi rezervasyona ait.</summary>
        public Guid ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; } = null!;

        /// <summary>Hangi koltuk.</summary>
        public Guid SeatId { get; set; }
        public virtual Seat Seat { get; set; } = null!;

        /// <summary>
        /// Bu koltuk için ödenen fiyat.
        /// Seans fiyatından farklı olabilir (promosyon, indirim).
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>Koltuğun bu rezervasyondaki durumu.</summary>
        public SeatStatus Status { get; set; } = SeatStatus.Reserved;
    }
}
