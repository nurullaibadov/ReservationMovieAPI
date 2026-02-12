using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Persistence.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.RowLabel)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(s => s.SeatType)
                .HasConversion<string>()  // Enum'u string olarak sakla (okunabilir)
                .HasMaxLength(20);

            // Hall → Seat (Many-to-One)
            builder.HasOne(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            // Benzersiz kısıt: Aynı salonda aynı koltuk olamaz
            builder.HasIndex(s => new { s.HallId, s.RowLabel, s.SeatNumber })
                .IsUnique()
                .HasDatabaseName("IX_Seats_Hall_Row_Number");
        }
    }
}
