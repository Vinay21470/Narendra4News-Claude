using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class MovieCollectionConfiguration : IEntityTypeConfiguration<MovieCollection>
{
    public void Configure(EntityTypeBuilder<MovieCollection> b)
    {
        foreach (var prop in new[] { nameof(MovieCollection.IndiaNet), nameof(MovieCollection.IndiaGross),
                 nameof(MovieCollection.Overseas), nameof(MovieCollection.WorldwideGross),
                 nameof(MovieCollection.OpeningDay), nameof(MovieCollection.WeekendCollection),
                 nameof(MovieCollection.TotalCollection) })
        {
            b.Property(prop).HasColumnType("decimal(18,2)");
        }

        b.HasIndex(mc => mc.MovieId);
        b.HasIndex(mc => mc.CollectionDate);
        // A movie can have at most one row per (day label + date) - prevents
        // accidental duplicate "Day 2" entries while still allowing Day 1..N,
        // Weekend, Week 1, Lifetime etc. to coexist independently (spec #33/#34).
        b.HasIndex(mc => new { mc.MovieId, mc.DayNumber, mc.Label, mc.CollectionDate }).IsUnique();

        b.HasOne(mc => mc.Movie)
            .WithMany(m => m.Collections)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
