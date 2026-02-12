using Microsoft.EntityFrameworkCore;
using MovieReservationAPI.Persistence.Context;
using ReservationMovieAPI.Application.Interfaces.Repositories;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.Repositories
{
    public class ReservationRepository
        : GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<(IReadOnlyList<Reservation> Items, int TotalCount)>
            GetByUserIdAsync(
                string userId, int pageNumber, int pageSize,
                CancellationToken ct = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.Cinema)
                .Include(r => r.ReservationSeats)
                    .ThenInclude(rs => rs.Seat)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<Reservation?> GetByCodeAsync(
            string code, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationCode == code, ct);
        }

        public async Task<Reservation?> GetWithDetailsAsync(
            Guid id, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.Cinema)
                .Include(r => r.ReservationSeats)
                    .ThenInclude(rs => rs.Seat)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<IReadOnlyList<Reservation>> GetByScreeningIdAsync(
            Guid screeningId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.ReservationSeats)
                .Where(r => r.ScreeningId == screeningId &&
                            r.Status != ReservationStatus.Cancelled &&
                            r.Status != ReservationStatus.Expired)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.Status == ReservationStatus.Pending &&
                            r.ExpiresAt < DateTime.UtcNow)
                .Include(r => r.ReservationSeats)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Guid>> GetReservedSeatIdsAsync(
            Guid screeningId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.ScreeningId == screeningId &&
                            r.Status != ReservationStatus.Cancelled &&
                            r.Status != ReservationStatus.Expired)
                .SelectMany(r => r.ReservationSeats
                    .Select(rs => rs.SeatId))
                .Distinct()
                .ToListAsync(ct);
        }
    }
}
