using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> b)
    {
        b.Property(a => a.Title).HasMaxLength(300).IsRequired();
        b.Property(a => a.Slug).HasMaxLength(350).IsRequired();
        b.Property(a => a.ShortDescription).HasMaxLength(500);
        b.Property(a => a.SeoTitle).HasMaxLength(300);
        b.Property(a => a.SeoDescription).HasMaxLength(500);

        // Slug must be unique and is the primary lookup for /article/{slug}.
        b.HasIndex(a => a.Slug).IsUnique();
        b.HasIndex(a => a.PublishedDate);
        b.HasIndex(a => a.CategoryId);
        b.HasIndex(a => a.Status);
        b.HasIndex(a => new { a.Status, a.PublishedDate }); // homepage/list queries

        b.HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(a => a.Movie)
            .WithMany(m => m.Articles)
            .HasForeignKey(a => a.MovieId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
