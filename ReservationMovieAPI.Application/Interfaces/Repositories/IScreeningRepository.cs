using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Repositories
{
    public interface IScreeningRepository : IGenericRepository<Screening>
    {
        /// <summary>Belirli bir film için aktif seansları getir.</summary>
        Task<IReadOnlyList<Screening>> GetByMovieIdAsync(
            Guid movieId, DateTime? fromDate = null,
            CancellationToken ct = default);

        /// <summary>Belirli bir salon için belirli tarihte seanslar.</summary>
        Task<IReadOnlyList<Screening>> GetByHallAndDateAsync(
            Guid hallId, DateTime date, CancellationToken ct = default);

        /// <summary>
        /// Salon çakışma kontrolü.
        /// Aynı salonda aynı saatte başka seans var mı?
        /// </summary>
        Task<bool> HasOverlapAsync(
            Guid hallId, DateTime startTime, DateTime endTime,
            Guid? excludeScreeningId = null,
            CancellationToken ct = default);

        /// <summary>Seans detayını (film + salon + koltuklar) getir.</summary>
        Task<Screening?> GetScreeningWithDetailsAsync(
            Guid screeningId, CancellationToken ct = default);

        /// <summary>Belirli tarih aralığında seanslar.</summary>
        Task<IReadOnlyList<Screening>> GetByDateRangeAsync(
            DateTime from, DateTime to, CancellationToken ct = default);
    }
}
