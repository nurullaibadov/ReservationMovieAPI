using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace MovieReservationAPI.Persistence
{

    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── EF Core SQL Server ────────────────────────────────────────────────
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        // Migration assembly: Persistence katmanında
                        sqlOptions.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.FullName);

                        // Geçici bağlantı hatalarında otomatik yeniden dene
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });

                // Development'ta SQL sorgularını logla
                options.EnableSensitiveDataLogging(false);
            });

            // ── ASP.NET Core Identity ─────────────────────────────────────────────
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Şifre kuralları
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // Hesap kilitleme (brute force koruması)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Email benzersiz olsun
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // ── Repository Kayıtları ──────────────────────────────────────────────
            services.AddScoped(typeof(IGenericRepository<>),
                typeof(GenericRepository<>));
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IScreeningRepository, ScreeningRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();

            // ── Unit of Work ──────────────────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
