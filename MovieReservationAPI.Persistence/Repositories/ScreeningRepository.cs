using Microsoft.EntityFrameworkCore;
using MovieReservationAPI.Persistence.Context;
using ReservationMovieAPI.Application.Interfaces.Repositories;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.Repositories
{
    public class ScreeningRepository
    : GenericRepository<Screening>, IScreeningRepository
    {
        public ScreeningRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Screening>> GetByMovieIdAsync(
            Guid movieId, DateTime? fromDate = null,
            CancellationToken ct = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(s => s.Hall)
                    .ThenInclude(h => h.Cinema)
                .Where(s => s.MovieId == movieId && s.IsActive);

            if (fromDate.HasValue)
                query = query.Where(s => s.StartTime >= fromDate.Value);

            return await query
                .OrderBy(s => s.StartTime)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Screening>> GetByHallAndDateAsync(
            Guid hallId, DateTime date, CancellationToken ct = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Movie)
                .Where(s => s.HallId == hallId &&
                            s.StartTime >= startOfDay &&
                            s.StartTime < endOfDay &&
                            s.IsActive)
                .OrderBy(s => s.StartTime)
                .ToListAsync(ct);
        }

        public async Task<bool> HasOverlapAsync(
            Guid hallId, DateTime startTime, DateTime endTime,
            Guid? excludeScreeningId = null,
            CancellationToken ct = default)
        {
            var query = _dbSet.Where(s =>
                s.HallId == hallId &&
                s.IsActive &&
                s.StartTime < endTime &&
                s.EndTime > startTime);

            if (excludeScreeningId.HasValue)
                query = query.Where(s => s.Id != excludeScreeningId.Value);

            return await query.AnyAsync(ct);
        }

        public async Task<Screening?> GetScreeningWithDetailsAsync(
            Guid screeningId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                    .ThenInclude(h => h.Seats)
                .Include(s => s.Hall)
                    .ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(s => s.Id == screeningId, ct);
        }

        public async Task<IReadOnlyList<Screening>> GetByDateRangeAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                    .ThenInclude(h => h.Cinema)
                .Where(s => s.StartTime >= from &&
                            s.StartTime <= to &&
                            s.IsActive)
                .OrderBy(s => s.StartTime)
                .ToListAsync(ct);
        }
    }
}
