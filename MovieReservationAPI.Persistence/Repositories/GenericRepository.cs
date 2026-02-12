using Microsoft.EntityFrameworkCore;
using MovieReservationAPI.Persistence.Context;
using ReservationMovieAPI.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.FindAsync(new object[] { id }, ct);

        public async Task<T?> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null)
                query = includes(query);

            // EF Core'un parametrelenmiş filtresi
            var keyName = _context.Model
                .FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties
                .Select(x => x.Name)
                .First();

            return await query.FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, keyName) == id, ct);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
            => await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            bool disableTracking = true,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync(ct);

            return await query.ToListAsync(ct);
        }

        public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync(ct);

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            CancellationToken ct = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null)
                query = includes(query);

            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default)
        {
            if (predicate == null)
                return await _dbSet.CountAsync(ct);

            return await _dbSet.CountAsync(predicate, ct);
        }

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
            => await _dbSet.AnyAsync(predicate, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken ct = default)
            => await _dbSet.AddRangeAsync(entities, ct);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void UpdateRange(IEnumerable<T> entities)
            => _dbSet.UpdateRange(entities);

        public void Delete(T entity)
            => _dbSet.Remove(entity); // DbContext.SaveChanges'te soft delete'e dönüşür

        public void DeleteRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);
    }
}
