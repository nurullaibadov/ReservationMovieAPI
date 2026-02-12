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
    public class MovieRepository : GenericRepository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Movie>> SearchByTitleAsync(
            string searchTerm, int pageNumber, int pageSize,
            CancellationToken ct = default)
        {
            var term = searchTerm.ToLower();

            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Where(m => m.Title.ToLower().Contains(term) ||
                            m.Director.ToLower().Contains(term) ||
                            (m.LocalTitle != null &&
                             m.LocalTitle.ToLower().Contains(term)))
                .OrderBy(m => m.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Movie>> GetByGenreAsync(
            Guid genreId, int pageNumber, int pageSize,
            CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Where(m => m.Genres.Any(g => g.Id == genreId))
                .OrderBy(m => m.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Movie>> GetNowShowingAsync(
            CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Where(m => m.IsNowShowing)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Movie>> GetComingSoonAsync(
            CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Where(m => m.IsComingSoon && m.ReleaseDate > DateTime.UtcNow)
                .OrderBy(m => m.ReleaseDate)
                .ToListAsync(ct);
        }

        public async Task<Movie?> GetMovieWithDetailsAsync(
            Guid movieId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Include(m => m.Screenings.Where(s =>
                    s.IsActive && s.StartTime > DateTime.UtcNow))
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(m => m.Id == movieId, ct);
        }

        public async Task<IReadOnlyList<Movie>> GetMoviesByCityAsync(
            string city, DateTime date, CancellationToken ct = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Genres)
                .Where(m => m.Screenings.Any(s =>
                    s.IsActive &&
                    s.StartTime >= startOfDay &&
                    s.StartTime < endOfDay &&
                    s.Hall.Cinema.City == city))
                .OrderBy(m => m.Title)
                .ToListAsync(ct);
        }
    }
}
