using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Repositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        /// <summary>Kullanıcının tüm rezervasyonları (sayfalı).</summary>
        Task<(IReadOnlyList<Reservation> Items, int TotalCount)> GetByUserIdAsync(
            string userId, int pageNumber, int pageSize,
            CancellationToken ct = default);

        /// <summary>Rezervasyon koduna göre getir.</summary>
        Task<Reservation?> GetByCodeAsync(
            string code, CancellationToken ct = default);

        /// <summary>Rezervasyon detayını tüm ilişkileriyle getir.</summary>
        Task<Reservation?> GetWithDetailsAsync(
            Guid id, CancellationToken ct = default);

        /// <summary>Belirli bir seansa ait rezervasyonlar.</summary>
        Task<IReadOnlyList<Reservation>> GetByScreeningIdAsync(
            Guid screeningId, CancellationToken ct = default);

        /// <summary>
        /// Süresi dolmuş (Pending ve ExpiresAt geçmiş) rezervasyonlar.
        /// Background job tarafından kullanılır.
        /// </summary>
        Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Bir seansta belirli koltukların rezerve olup olmadığını kontrol et.
        /// Eş zamanlı rezervasyon çakışmasını önler.
        /// </summary>
        Task<IReadOnlyList<Guid>> GetReservedSeatIdsAsync(
            Guid screeningId, CancellationToken ct = default);
    }
}
