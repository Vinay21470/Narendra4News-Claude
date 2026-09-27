using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class ArticleViewConfiguration : IEntityTypeConfiguration<ArticleView>
{
    public void Configure(EntityTypeBuilder<ArticleView> b)
    {
        b.HasIndex(v => new { v.ArticleId, v.ViewDate }).IsUnique();

        b.HasOne(v => v.Article)
            .WithMany(a => a.ViewRecords)
            .HasForeignKey(v => v.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
