using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Repositories
{
    public interface IMovieRepository : IGenericRepository<Movie>
    {
        /// <summary>Film adına göre tam metin arama.</summary>
        Task<IReadOnlyList<Movie>> SearchByTitleAsync(
            string searchTerm, int pageNumber, int pageSize,
            CancellationToken ct = default);

        /// <summary>Türe göre filmleri getir.</summary>
        Task<IReadOnlyList<Movie>> GetByGenreAsync(
            Guid genreId, int pageNumber, int pageSize,
            CancellationToken ct = default);

        /// <summary>Şu an vizyondaki filmler.</summary>
        Task<IReadOnlyList<Movie>> GetNowShowingAsync(
            CancellationToken ct = default);

        /// <summary>Yakında gelecek filmler.</summary>
        Task<IReadOnlyList<Movie>> GetComingSoonAsync(
            CancellationToken ct = default);

        /// <summary>Film detayını tüm ilişkileriyle getir.</summary>
        Task<Movie?> GetMovieWithDetailsAsync(
            Guid movieId, CancellationToken ct = default);

        /// <summary>Belirli bir şehirde gösterimde olan filmler.</summary>
        Task<IReadOnlyList<Movie>> GetMoviesByCityAsync(
            string city, DateTime date, CancellationToken ct = default);
    }
}
