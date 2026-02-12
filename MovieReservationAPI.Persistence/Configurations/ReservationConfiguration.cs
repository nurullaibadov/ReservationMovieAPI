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
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReservationCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(r => r.TotalAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(r => r.DiscountAmount)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(r => r.NetAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(r => r.CancellationNote)
                .HasMaxLength(500);

            builder.Property(r => r.RowVersion)
                .IsRowVersion();

            // Kullanıcı → Rezervasyon (Many-to-One)
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seans → Rezervasyon
            builder.HasOne(r => r.Screening)
                .WithMany(sc => sc.Reservations)
                .HasForeignKey(r => r.ScreeningId)
                .OnDelete(DeleteBehavior.Restrict);

            // Benzersiz rezervasyon kodu
            builder.HasIndex(r => r.ReservationCode)
                .IsUnique()
                .HasDatabaseName("IX_Reservations_Code");

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_Reservations_UserId");

            builder.HasIndex(r => r.Status)
                .HasDatabaseName("IX_Reservations_Status");
        }
    }
}