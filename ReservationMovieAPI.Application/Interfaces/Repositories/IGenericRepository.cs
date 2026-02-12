using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // ─── Okuma Operasyonları ──────────────────────────────────────────────

        /// <summary>ID ile tek kayıt getirir. Bulamazsa null döner.</summary>
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// ID ile tek kayıt getirir, ilişkili tablolarla birlikte (Include).
        /// includes parametresi: m => m.Include(x => x.Genre).ThenInclude(...)
        /// </summary>
        Task<T?> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default);

        /// <summary>Tüm kayıtları listeler (soft delete filtreli).</summary>
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Koşula göre filtreli liste.
        /// Örnek: GetAsync(m => m.IsNowShowing)
        /// </summary>
        Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            bool disableTracking = true,
            CancellationToken ct = default);

        /// <summary>
        /// Sayfalanmış liste.
        /// </summary>
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default);

        /// <summary>Koşula uyan ilk kayıt.</summary>
        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default);

        /// <summary>Koşula uyan kayıt sayısı.</summary>
        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default);

        /// <summary>Koşula uyan kayıt var mı?</summary>
        Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);

        // ─── Yazma Operasyonları ──────────────────────────────────────────────

        /// <summary>Yeni kayıt ekler (SaveChanges dahil değil).</summary>
        Task AddAsync(T entity, CancellationToken ct = default);

        /// <summary>Birden fazla kayıt ekler.</summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>Kayıt günceller.</summary>
        void Update(T entity);

        /// <summary>Birden fazla kayıt günceller.</summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>Soft delete uygular.</summary>
        void Delete(T entity);

        /// <summary>Birden fazla kayıt soft delete.</summary>
        void DeleteRange(IEnumerable<T> entities);
    }
}
