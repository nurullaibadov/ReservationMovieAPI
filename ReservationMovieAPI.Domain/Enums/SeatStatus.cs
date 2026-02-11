using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Enums
{
    public enum SeatStatus
    {
        /// <summary>Koltuk müsait, rezerve edilebilir.</summary>
        Available = 1,

        /// <summary>
        /// Koltuk geçici olarak kilitli.
        /// Birisi ödeme ekranındayken diğerleri alamasın diye.
        /// 10 dakika sonra otomatik açılır.
        /// </summary>
        Held = 2,

        /// <summary>Koltuk rezerve edilmiş, ödeme bekleniyor.</summary>
        Reserved = 3,

        /// <summary>Ödeme tamamlanmış, koltuk satılmış.</summary>
        Occupied = 4,

        /// <summary>Koltuk devre dışı (bakım, arıza vb.)</summary>
        Disabled = 5
    }
}
