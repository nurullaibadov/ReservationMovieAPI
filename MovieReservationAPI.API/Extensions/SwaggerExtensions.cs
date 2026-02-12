using Microsoft.OpenApi.Models;

namespace MovieReservationAPI.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Movie Reservation System API",
                    Version = "v1",
                    Description = @"
## 🎬 Movie Reservation System API

ASP.NET Core 8 + Onion Architecture ile geliştirilmiş 
film rezervasyon sistemi.

### Özellikler
- JWT tabanlı kimlik doğrulama
- Film, sinema, salon ve seans yönetimi
- Gerçek zamanlı koltuk rezervasyonu
- Ödeme işleme

### Kullanım
1. `/auth/register` ile kayıt olun
2. `/auth/login` ile token alın
3. Swagger UI'daki **Authorize** butonuna token'ı yapıştırın
4. Tüm endpoint'lere erişin
                ",
                    Contact = new OpenApiContact
                    {
                        Name = "Movie Reservation Team",
                        Email = "dev@moviereservation.com"
                    }
                });

                // JWT Bearer şeması tanımla
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token'ınızı girin.\n\n" +
                                  "Örnek: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                // Tüm endpoint'lere Bearer şemasını uygula
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

                // XML dokümantasyon dosyasını ekle
                var xmlFile = $"{System.Reflection.Assembly
                    .GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);

                // Enum'ları string olarak göster
                options.UseInlineDefinitionsForEnums();
            });

            return services;
        }
    }
}
