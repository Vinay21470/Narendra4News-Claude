using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Narendra4News.Domain.Entities;

namespace Narendra4News.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieCollection> MovieCollections => Set<MovieCollection>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Media> MediaItems => Set<Media>();
    public DbSet<ArticleView> ArticleViews => Set<ArticleView>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
