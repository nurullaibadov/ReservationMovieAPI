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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(p => p.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("TRY");

            builder.Property(p => p.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.GatewayResponse)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.RefundAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.CardHolderName)
                .HasMaxLength(100);

            builder.Property(p => p.Last4Digits)
                .HasMaxLength(4);

            // Her rezervasyonun tek bir ödemesi var (One-to-One)
            builder.HasOne(p => p.Reservation)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.TransactionId)
                .HasDatabaseName("IX_Payments_TransactionId");
        }
    }
}
