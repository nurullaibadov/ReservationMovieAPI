using ReservationMovieAPI.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        IGenericRepository<Domain.Entities.Genre> Genres { get; }
        IGenericRepository<Domain.Entities.Cinema> Cinemas { get; }
        IGenericRepository<Domain.Entities.Hall> Halls { get; }
        IGenericRepository<Domain.Entities.Seat> Seats { get; }
        IScreeningRepository Screenings { get; }
        IReservationRepository Reservations { get; }
        IGenericRepository<Domain.Entities.Payment> Payments { get; }

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydeder.
        /// Başarısız olursa tüm değişiklikler geri alınır.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>Explicit transaction başlatır.</summary>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>Transaction'ı commit eder.</summary>
        Task CommitTransactionAsync(CancellationToken ct = default);

        /// <summary>Transaction'ı geri alır.</summary>
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
