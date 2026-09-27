using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Narendra4News.Domain.Entities;
using Narendra4News.Domain.Enums;

namespace Narendra4News.Infrastructure.Data;

// Runs on startup (see Program.cs). Seeds roles, an initial Admin account
// from configuration (never hardcoded), default categories, and - only in
// Development - sample movies/articles clearly flagged as sample content
// (spec sections 27 & 28).
public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config,
        ILogger logger,
        bool isDevelopment)
    {
        // Schema is created by the committed EF migrations before seeding.

        foreach (var role in new[] { UserRoles.Admin, UserRoles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = config["ADMIN_EMAIL"] ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var adminPassword = config["ADMIN_PASSWORD"] ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserRoles.Admin);
                    logger.LogInformation("Seeded initial Admin account for {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to seed Admin account: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        else
        {
            logger.LogWarning(
                "ADMIN_EMAIL / ADMIN_PASSWORD not configured - skipping admin seeding. " +
                "Set them via environment variables, User Secrets (dev) or Azure App Service configuration / Key Vault (prod).");
        }

        if (!db.Categories.Any())
        {
            var categories = new[]
            {
                "Telugu Updates", "Trending", "Box Office", "Gossips", "Reviews",
                "Special", "TRP", "Records", "Movies", "OTT", "Trailers", "Events"
            }.Select((name, i) => new Category
            {
                Name = name,
                Slug = Slugify(name),
                DisplayOrder = i,
                IsActive = true
            });

            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded default categories.");
        }

        if (isDevelopment && !db.Movies.Any())
        {
            var movie = new Movie
            {
                MovieName = "[SAMPLE] The Paradise",
                Slug = "sample-the-paradise",
                Description = "Sample development data - remove before production (spec section 28).",
                ReleaseDate = DateTime.UtcNow.AddDays(-3),
                Genre = "Action",
                Language = "Telugu"
            };
            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            db.MovieCollections.AddRange(
                new MovieCollection { MovieId = movie.Id, CollectionDate = movie.ReleaseDate!.Value, DayNumber = 1, Label = "Day 1", IndiaNet = 1850, WorldwideGross = 3200 },
                new MovieCollection { MovieId = movie.Id, CollectionDate = movie.ReleaseDate!.Value.AddDays(1), DayNumber = 2, Label = "Day 2", IndiaNet = 1420, WorldwideGross = 2600 }
            );

            var boxOfficeCategory = db.Categories.First(c => c.Slug == "box-office");
            var article = new Article
            {
                Title = "[SAMPLE] The Paradise Day 2 Total Worldwide Collections",
                Slug = "sample-the-paradise-day-2-total-worldwide-collections",
                ShortDescription = "Sample article - remove before production.",
                Content = "<p>This is sample development content.</p>",
                CategoryId = boxOfficeCategory.Id,
                MovieId = movie.Id,
                Status = ArticleStatus.Published,
                PublishedDate = DateTime.UtcNow,
                IsFeatured = true,
                SeoTitle = "The Paradise Day 2 Collections",
                SeoDescription = "Sample SEO description."
            };

            // Sample article needs an AuthorId (FK). Reuse the seeded admin if present.
            var admin = adminEmail is not null ? await userManager.FindByEmailAsync(adminEmail) : null;
            if (admin is not null)
            {
                article.AuthorId = admin.Id;
                db.Articles.Add(article);
                await db.SaveChangesAsync();
            }

            logger.LogInformation("Seeded sample development movie/article data.");
        }
    }

    public static string Slugify(string input) =>
        input.ToLowerInvariant().Trim().Replace(" ", "-").Replace("'", "");
}
