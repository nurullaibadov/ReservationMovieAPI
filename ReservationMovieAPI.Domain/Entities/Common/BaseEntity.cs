using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities.Common
{
  public abstract class BaseEntity
    {
        /// <summary>
        /// Birincil anahtar. GUID kullanıyoruz çünkü:
        /// - Dağıtık sistemlerde çakışma riski yok
        /// - URL'de expose edildiğinde sequence tahmin edilemez
        /// - Merge/import işlemlerinde sorun yok
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Kaydın veritabanına eklendiği tarih/saat (UTC).
        /// UTC kullanıyoruz — farklı timezone'larda tutarlı çalışır.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Kaydı oluşturan kullanıcının Id'si.
        /// Nullable çünkü sistem tarafından oluşturulan kayıtlar olabilir.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Son güncelleme tarihi. Null ise hiç güncellenmemiş demek.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Son güncelleyen kullanıcının Id'si.
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Soft delete bayrağı.
        /// True ise kayıt "silinmiş" kabul edilir ama veritabanından kaldırılmaz.
        /// Neden? Rezervasyon geçmişi, ödeme kaydı gibi kritik verileri
        /// gerçekten silmek istemeyiz — GDPR ve audit trail için.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Soft delete tarih/saati.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Silen kullanıcının Id'si.
        /// </summary>
        public string? DeletedBy { get; set; }

        /// <summary>
        /// Optimistic concurrency control için.
        /// EF Core bu field'ı row version olarak kullanır.
        /// Aynı anda iki kişi aynı kaydı güncellemeye çalışırsa
        /// ikincisi ConcurrencyException alır.
        /// Sinema rezervasyonlarında kritik!
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
