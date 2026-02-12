using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationMovieAPI.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Settings (Options Pattern) ────────────────────────────────────────
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));
            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));
            services.Configure<AzureStorageSettings>(
                configuration.GetSection("AzureStorage"));

            // ── JWT Authentication ────────────────────────────────────────────────
            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero // Token tam süresinde sona ersin
                };
            });

            // ── Authorization Policies ────────────────────────────────────────────
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerOrAdmin",
                    policy => policy.RequireRole("Customer", "Admin"));
            });

            // ── Servisler ─────────────────────────────────────────────────────────
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
