using ReservationMovieAPI.Domain.Entities.Common;
using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        /// <summary>Rezervasyonu yapan kullanıcı.</summary>
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        /// <summary>Hangi seans için rezervasyon yapıldı.</summary>
        public Guid ScreeningId { get; set; }
        public virtual Screening Screening { get; set; } = null!;

        /// <summary>
        /// Rezervasyon kodu — kullanıcıya gösterilen benzersiz referans.
        /// Örnek: MRS-20241215-ABC12
        /// </summary>
        public string ReservationCode { get; set; } = string.Empty;

        /// <summary>Rezervasyonun mevcut durumu.</summary>
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        /// <summary>
        /// Toplam tutar — seçilen koltukların fiyatlarının toplamı.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Uygulanan indirim tutarı.</summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>
        /// Ödenecek net tutar.
        /// NetAmount = TotalAmount - DiscountAmount
        /// </summary>
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Rezervasyonun sona erme süresi.
        /// Bu süre içinde ödeme yapılmazsa Expired'a alınır.
        /// Genellikle oluşturma zamanından +10 dakika.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>İptal notu (kullanıcı veya admin tarafından).</summary>
        public string? CancellationNote { get; set; }

        /// <summary>İptal tarihi.</summary>
        public DateTime? CancelledAt { get; set; }

        /// <summary>Rezervasyondaki koltuklar (many-to-many ara tablosu).</summary>
        public virtual ICollection<ReservationSeat> ReservationSeats { get; set; }
            = new HashSet<ReservationSeat>();

        /// <summary>Bu rezervasyona ait ödeme.</summary>
        public virtual Payment? Payment { get; set; }
    }
}
