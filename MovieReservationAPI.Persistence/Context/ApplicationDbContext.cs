using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ─── DbSet'ler ───────────────────────────────────────────────────────────
        // Her DbSet bir veritabanı tablosunu temsil eder.
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Screening> Screenings => Set<Screening>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity tablolarının konfigürasyonunu çalıştır
            base.OnModelCreating(modelBuilder);

            // Tüm entity konfigürasyon sınıflarını bu assembly'den otomatik uygula
            // Tek tek cfg.ApplyConfiguration(new MovieConfig()) yazmak yerine.
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);

            // Global Query Filter: Soft delete
            // IsDeleted = true olan kayıtlar varsayılan sorgularda görünmez.
            // Tüm entity'lere otomatik uygular (BaseEntity'den miras alanlar).
            modelBuilder.Entity<Movie>()
                .HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Genre>()
                .HasQueryFilter(g => !g.IsDeleted);
            modelBuilder.Entity<Cinema>()
                .HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Hall>()
                .HasQueryFilter(h => !h.IsDeleted);
            modelBuilder.Entity<Seat>()
                .HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Screening>()
                .HasQueryFilter(sc => !sc.IsDeleted);
            modelBuilder.Entity<Reservation>()
                .HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<ReservationSeat>()
                .HasQueryFilter(rs => !rs.IsDeleted);
            modelBuilder.Entity<Payment>()
                .HasQueryFilter(p => !p.IsDeleted);
        }

        /// <summary>
        /// SaveChanges'i override ediyoruz:
        /// 1. Soft delete bayrağını otomatik koy
        /// 2. CreatedAt / UpdatedAt audit alanlarını doldur
        /// 3. CreatedBy / UpdatedBy kullanıcısını doldur (ICurrentUserService ile)
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        // Gerçek silme yerine soft delete uygula
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}