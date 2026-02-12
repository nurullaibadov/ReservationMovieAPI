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
    public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
    {
        public void Configure(EntityTypeBuilder<ReservationSeat> builder)
        {
            builder.ToTable("ReservationSeats");

            builder.HasKey(rs => rs.Id);

            builder.Property(rs => rs.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(rs => rs.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationSeats)
                .HasForeignKey(rs => rs.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Seat)
                .WithMany(s => s.ReservationSeats)
                .HasForeignKey(rs => rs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aynı koltuk aynı rezervasyona 2 kez eklenemez
            builder.HasIndex(rs => new { rs.ReservationId, rs.SeatId })
                .IsUnique()
                .HasDatabaseName("IX_ReservationSeats_Unique");
        }
    }
}