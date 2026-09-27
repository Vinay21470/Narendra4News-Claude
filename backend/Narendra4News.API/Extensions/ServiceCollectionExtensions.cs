using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Infrastructure.Data;
using Narendra4News.Infrastructure.Services;
using System.Text;

namespace Narendra4News.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNarendra4NewsServices(this IServiceCollection services, IConfiguration config)
    {
        // Connection string: AZURE_SQL_CONNECTION_STRING (Azure App Service config /
        // Key Vault in prod) falls back to the standard EF "DefaultConnection" key
        // for local development via appsettings.Development.json / User Secrets.
        var connectionString = config["AZURE_SQL_CONNECTION_STRING"] ?? config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No SQL connection string configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        var jwtSecret = config["JWT_SECRET"] ?? throw new InvalidOperationException(
            "JWT_SECRET is not configured. Set it via environment variable, User Secrets, or Azure Key Vault - never in source.");
        var jwtIssuer = config["JWT_ISSUER"] ?? "Narendra4News";
        var jwtAudience = config["JWT_AUDIENCE"] ?? "Narendra4NewsClient";

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });

        services.AddAuthorization();

        services.AddCors(options =>
        {
            options.AddPolicy("Narendra4NewsCors", policy =>
            {
                var allowedOrigins = config["CORS_ALLOWED_ORIGINS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    ?? new[] { "https://narendra4news.net", "https://www.narendra4news.net", "http://localhost:5173" };
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
