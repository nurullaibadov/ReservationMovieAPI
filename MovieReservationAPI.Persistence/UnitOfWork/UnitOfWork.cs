using Microsoft.EntityFrameworkCore.Storage;
using MovieReservationAPI.Persistence.Context;
using MovieReservationAPI.Persistence.Repositories;
using ReservationMovieAPI.Application.Interfaces;
using ReservationMovieAPI.Application.Interfaces.Repositories;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization — sadece kullanıldığında oluşturulur
        private IMovieRepository? _movies;
        private IGenericRepository<Genre>? _genres;
        private IGenericRepository<Cinema>? _cinemas;
        private IGenericRepository<Hall>? _halls;
        private IGenericRepository<Seat>? _seats;
        private IScreeningRepository? _screenings;
        private IReservationRepository? _reservations;
        private IGenericRepository<Payment>? _payments;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IMovieRepository Movies =>
            _movies ??= new MovieRepository(_context);

        public IGenericRepository<Genre> Genres =>
            _genres ??= new GenericRepository<Genre>(_context);

        public IGenericRepository<Cinema> Cinemas =>
            _cinemas ??= new GenericRepository<Cinema>(_context);

        public IGenericRepository<Hall> Halls =>
            _halls ??= new GenericRepository<Hall>(_context);

        public IGenericRepository<Seat> Seats =>
            _seats ??= new GenericRepository<Seat>(_context);

        public IScreeningRepository Screenings =>
            _screenings ??= new ScreeningRepository(_context);

        public IReservationRepository Reservations =>
            _reservations ??= new ReservationRepository(_context);

        public IGenericRepository<Payment> Payments =>
            _payments ??= new GenericRepository<Payment>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
                await _transaction.CommitAsync(ct);
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(ct);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
