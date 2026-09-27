using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> b)
    {
        // One like per user per article.
        b.HasIndex(l => new { l.ArticleId, l.UserId }).IsUnique();

        b.HasOne(l => l.Article)
            .WithMany(a => a.LikeRecords)
            .HasForeignKey(l => l.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
