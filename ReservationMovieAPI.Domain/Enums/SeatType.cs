using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Enums
{
    public enum SeatType
    {
        /// <summary>Normal koltuk.</summary>
        Standard = 1,

        /// <summary>Daha iyi konum/konfor, biraz daha pahalı.</summary>
        Premium = 2,

        /// <summary>En iyi konum, en pahalı.</summary>
        VIP = 3,

        /// <summary>Engelli erişimine uygun koltuk.</summary>
        Accessible = 4
    }
}
