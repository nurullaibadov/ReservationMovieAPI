using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // AutoMapper — bu assembly'deki tüm mapping profile'ları bul ve kaydet
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation — bu assembly'deki tüm validator'ları bul ve kaydet
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // MediatR — CQRS handler'larını bu assembly'de ara
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                // Pipeline Behaviors — her MediatR request'i için sırayla çalışır:
                // 1. LoggingBehavior     → İsteği logla
                // 2. ValidationBehavior → FluentValidation ile doğrula
                // 3. PerformanceBehavior → Yavaş sorguları logla
                cfg.AddBehavior(typeof(IPipelineBehavior<,>),
                    typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>),
                    typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>),
                    typeof(PerformanceBehavior<,>));
            });

            return services;
        }
    }
}
