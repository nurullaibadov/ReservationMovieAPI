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
    public class ScreeningConfiguration : IEntityTypeConfiguration<Screening>
    {
        public void Configure(EntityTypeBuilder<Screening> builder)
        {
            builder.ToTable("Screenings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.StandardPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(s => s.PremiumPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(s => s.VipPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(s => s.RowVersion)
                .IsRowVersion();

            builder.HasOne(sc => sc.Movie)
                .WithMany(m => m.Screenings)
                .HasForeignKey(sc => sc.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Hall silinirse seans da silinsin, ama dikkatli ol
            builder.HasOne(sc => sc.Hall)
                .WithMany(h => h.Screenings)
                .HasForeignKey(sc => sc.HallId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict: Hall varken silemezsin

            // Aynı salonda çakışan seans olmasın — DB level kontrol
            builder.HasIndex(sc => new { sc.HallId, sc.StartTime })
                .HasDatabaseName("IX_Screenings_Hall_StartTime");

            builder.HasIndex(sc => sc.MovieId)
                .HasDatabaseName("IX_Screenings_MovieId");

            builder.HasIndex(sc => sc.StartTime)
                .HasDatabaseName("IX_Screenings_StartTime");
        }
    }

}