using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> b)
    {
        b.Property(m => m.BlobUrl).HasMaxLength(1000).IsRequired();
        b.Property(m => m.FileName).HasMaxLength(300).IsRequired();
    }
}
