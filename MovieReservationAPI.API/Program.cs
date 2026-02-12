using Asp.Versioning;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using MovieReservationAPI.API.Extensions;
using MovieReservationAPI.API.Middleware;
using MovieReservationAPI.Infrastructure;
using MovieReservationAPI.Infrastructure.SeedData;  
using MovieReservationAPI.Persistence;
using MovieReservationAPI.Persistence.Context;
using ReservationMovieAPI.Application;
using ReservationMovieAPI.Domain.Entities;
using Serilog;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("🎬 Movie Reservation System API başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithEnvironmentName());

    // ── Katman Servis Kayıtları ────────────────────────────────────────────
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddPersistenceServices(builder.Configuration);

    // ── Controller & JSON ──────────────────────────────────────────────────
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            // Enum'ları string olarak serialize et
            opts.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());

            // camelCase
            opts.JsonSerializerOptions.PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase;

            // Döngüsel referansları yoksay
            opts.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

            // Null değerleri yazma
            opts.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    // ── API Versioning ─────────────────────────────────────────────────────
    builder.Services.AddApiVersioning(opts =>
    {
        opts.DefaultApiVersion = new ApiVersion(1, 0);
        opts.AssumeDefaultVersionWhenUnspecified = true;
        opts.ReportApiVersions = true;
        opts.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"),
            new QueryStringApiVersionReader("api-version"));
    })
    .AddApiExplorer(opts =>
    {
        opts.GroupNameFormat = "'v'VVV";
        opts.SubstituteApiVersionInUrl = true;
    });

    // ── Swagger ────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerDocumentation();

    // ── Rate Limiting ──────────────────────────────────────────────────────
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(
        builder.Configuration.GetSection("RateLimiting"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration,
        RateLimitConfiguration>();

    // ── Health Checks ──────────────────────────────────────────────────────
    builder.Services.AddHealthChecks(builder.Configuration);

    // ── CORS ───────────────────────────────────────────────────────────────
    builder.Services.AddCors(opts =>
    {
        opts.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });

        opts.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(
                "https://yourdomain.com",
                "https://www.yourdomain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // ── Response Compression ───────────────────────────────────────────────
    builder.Services.AddResponseCompression(opts =>
    {
        opts.EnableForHttps = true;
    });

    // ═══════════════════════════════════════════════════════════════════════
    var app = builder.Build();
    // ═══════════════════════════════════════════════════════════════════════

    // ── Database Migration & Seed ──────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        await DatabaseSeeder.SeedAsync(
            dbContext, userManager, roleManager);

        Log.Information("Database migration ve seed tamamlandı.");
    }

    // ═══════════════════════════════════════════════════════════════════════
    // MİDDLEWARE PIPELINE — Sıralama kritik!
    // ═══════════════════════════════════════════════════════════════════════

    // 1. Exception Handler — en dışta olmalı ki her hatayı yakalar
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // 2. HTTPS yönlendirme
    app.UseHttpsRedirection();

    // 3. Response Compression
    app.UseResponseCompression();

    // 4. Serilog Request Logging
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} " +
            "responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // 5. Swagger (sadece Development veya açıkça izin verilmişse)
    if (app.Environment.IsDevelopment() ||
        builder.Configuration.GetValue<bool>("ApiSettings:EnableSwagger"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/swagger/v1/swagger.json",
                "Movie Reservation API v1");
            opts.RoutePrefix = "swagger";
            opts.DocExpansion(
                Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            opts.DisplayRequestDuration();
        });
    }

    // 6. Rate Limiting
    app.UseIpRateLimiting();

    // 7. CORS
    app.UseCors(app.Environment.IsDevelopment()
        ? "AllowAll"
        : "Production");

    // 8. Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // 9. Controllers
    app.MapControllers();

    // 10. Health Check Endpoint
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var result = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds
                })
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(result,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));
        }
    });

    // Minimal API test endpoint
    app.MapGet("/", () => new
    {
        service = "Movie Reservation System API",
        version = "1.0",
        status = "Running",
        timestamp = DateTime.UtcNow,
        swagger = "/swagger"
    });

    Log.Information(
        "API hazır. Swagger: https://localhost:{Port}/swagger",
        builder.Configuration["ASPNETCORE_URLS"]?.Split(";")
            .FirstOrDefault(u => u.Contains("https"))
            ?.Split(":").Last() ?? "7001");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ API başlatılamadı!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;