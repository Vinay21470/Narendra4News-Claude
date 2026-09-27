using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> b)
    {
        b.Property(m => m.MovieName).HasMaxLength(300).IsRequired();
        b.Property(m => m.Slug).HasMaxLength(350).IsRequired();
        b.Property(m => m.Budget).HasColumnType("decimal(18,2)");
        b.HasIndex(m => m.Slug).IsUnique();
    }
}
