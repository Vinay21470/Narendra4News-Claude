using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Narendra4News.API.Extensions;
using Narendra4News.API.Middleware;
using Narendra4News.Domain.Entities;
using Narendra4News.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault: in Production, secrets (JWT_SECRET, AZURE_SQL_CONNECTION_STRING,
// AZURE_STORAGE_CONNECTION_STRING, ADMIN_PASSWORD) are pulled from Key Vault via
// managed identity. Locally, the same keys come from User Secrets / env vars, so
// application code never branches on environment to find configuration (spec #16/#24).
var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];
if (!string.IsNullOrWhiteSpace(keyVaultUri) && builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new Azure.Identity.DefaultAzureCredential());
}

builder.Services.AddNarendra4NewsServices(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Narendra4News API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter a valid JWT token"
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
});

if (!string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
{
    builder.Services.AddApplicationInsightsTelemetry();
}

var app = builder.Build();

// Apply pending EF Core migrations and seed roles/admin/categories on startup.
// Safe to run every deploy: migrations are idempotent, seeding checks for
// existing data first (spec sections 26/27/28).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (db.Database.IsSqlServer())
        await db.Database.MigrateAsync();

    await DbInitializer.SeedAsync(db, userManager, roleManager, app.Configuration, logger, app.Environment.IsDevelopment());
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Narendra4NewsCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", async (ApplicationDbContext db, CancellationToken ct) =>
    await db.Database.CanConnectAsync(ct) ? Results.Ok(new { status = "healthy" }) : Results.StatusCode(503));

app.Run();
