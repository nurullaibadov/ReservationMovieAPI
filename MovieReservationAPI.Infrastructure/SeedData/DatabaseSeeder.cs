using Microsoft.AspNetCore.Identity;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.SeedData
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Migration'ları otomatik uygula
            await context.Database.MigrateAsync();

            // ── Roller ────────────────────────────────────────────────────────────
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── Admin kullanıcı ───────────────────────────────────────────────────
            var adminEmail = "admin@moviereservation.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123456!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── Türler (Genres) ───────────────────────────────────────────────────
            if (!await context.Genres.AnyAsync())
            {
                var genres = new[]
                {
                new Genre { Name = "Action", Description = "Aksiyon" },
                new Genre { Name = "Comedy", Description = "Komedi" },
                new Genre { Name = "Drama", Description = "Dram" },
                new Genre { Name = "Horror", Description = "Korku" },
                new Genre { Name = "Sci-Fi", Description = "Bilim Kurgu" },
                new Genre { Name = "Romance", Description = "Romantik" },
                new Genre { Name = "Thriller", Description = "Gerilim" },
                new Genre { Name = "Animation", Description = "Animasyon" }
            };

                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            // ── Sinema & Salonlar ─────────────────────────────────────────────────
            if (!await context.Cinemas.AnyAsync())
            {
                var cinema = new Cinema
                {
                    Name = "CineStar İstanbul",
                    Address = "Bağcılar Merkez Mah.",
                    City = "İstanbul",
                    OpeningTime = TimeSpan.FromHours(10),
                    ClosingTime = TimeSpan.FromHours(24),
                    IsActive = true
                };

                var hall1 = new Hall
                {
                    Name = "Salon 1",
                    HallType = "Standard",
                    Rows = 10,
                    SeatsPerRow = 15,
                    TotalSeats = 150,
                    IsActive = true
                };

                // Koltukları oluştur
                var seats = new List<Seat>();
                for (int row = 0; row < 10; row++)
                {
                    for (int num = 1; num <= 15; num++)
                    {
                        seats.Add(new Seat
                        {
                            HallId = hall1.Id,
                            RowLabel = ((char)('A' + row)).ToString(),
                            SeatNumber = num,
                            SeatType = row >= 8
                                ? Domain.Enums.SeatType.VIP
                                : row >= 5
                                    ? Domain.Enums.SeatType.Premium
                                    : Domain.Enums.SeatType.Standard,
                            IsActive = true
                        });
                    }
                }

                hall1.Seats = seats;
                cinema.Halls = new List<Hall> { hall1 };

                await context.Cinemas.AddAsync(cinema);
                await context.SaveChangesAsync();
            }
        }
    }
}
