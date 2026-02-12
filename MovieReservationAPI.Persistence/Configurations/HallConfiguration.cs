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
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.ToTable("Halls");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.HallType)
                .IsRequired()
                .HasMaxLength(50);

            // Hall → Cinema ilişkisi (Many-to-One)
            // Bir sinema silinirse salonları da silinsin (Cascade)
            builder.HasOne(h => h.Cinema)
                .WithMany(c => c.Halls)
                .HasForeignKey(h => h.CinemaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => h.CinemaId)
                .HasDatabaseName("IX_Halls_CinemaId");
        }
    }
}
