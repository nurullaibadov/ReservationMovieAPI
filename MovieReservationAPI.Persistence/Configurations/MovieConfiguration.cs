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
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.LocalTitle)
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.LongDescription)
                .HasMaxLength(2000);

            builder.Property(m => m.DurationMinutes)
                .IsRequired();

            builder.Property(m => m.Director)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Language)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(m => m.PosterUrl)
                .HasMaxLength(500);

            builder.Property(m => m.TrailerUrl)
                .HasMaxLength(500);

            builder.Property(m => m.ImdbRating)
                .HasColumnType("decimal(3,1)");

            builder.Property(m => m.AverageRating)
                .HasColumnType("decimal(3,1)")
                .HasDefaultValue(0);

            builder.Property(m => m.CastJson)
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("[]");

            // Concurrency token
            builder.Property(m => m.RowVersion)
                .IsRowVersion();

            // Many-to-many: Movie ↔ Genre (join table: MovieGenres)
            builder.HasMany(m => m.Genres)
                .WithMany(g => g.Movies)
                .UsingEntity(j => j.ToTable("MovieGenres"));

            // Index: Film başlığına göre hızlı arama
            builder.HasIndex(m => m.Title)
                .HasDatabaseName("IX_Movies_Title");

            // Index: Vizyondaki filmler için
            builder.HasIndex(m => m.IsNowShowing)
                .HasDatabaseName("IX_Movies_IsNowShowing");

            // Index: Soft delete filter ile birlikte optimize
            builder.HasIndex(m => m.IsDeleted)
                .HasDatabaseName("IX_Movies_IsDeleted");
        }
    }
}