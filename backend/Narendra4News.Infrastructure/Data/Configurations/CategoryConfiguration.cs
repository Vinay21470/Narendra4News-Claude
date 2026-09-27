using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.Property(c => c.Name).HasMaxLength(150).IsRequired();
        b.Property(c => c.Slug).HasMaxLength(150).IsRequired();
        b.HasIndex(c => c.Slug).IsUnique();
    }
}
