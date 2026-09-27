using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.DTOs.Admin;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _db;
    public AdminService(ApplicationDbContext db) => _db = db;

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var mostViewed = await _db.Articles.OrderByDescending(a => a.Views).FirstOrDefaultAsync(ct);

        return new DashboardStatsDto
        {
            TotalArticles = await _db.Articles.CountAsync(ct),
            PublishedArticles = await _db.Articles.CountAsync(a => a.Status == ArticleStatus.Published, ct),
            DraftArticles = await _db.Articles.CountAsync(a => a.Status == ArticleStatus.Draft, ct),
            TotalMovies = await _db.Movies.CountAsync(ct),
            TotalUsers = await _db.Users.CountAsync(ct),
            TotalViews = await _db.Articles.SumAsync(a => (long?)a.Views, ct) ?? 0,
            TodaysViews = await _db.ArticleViews.Where(v => v.ViewDate == today).SumAsync(v => (long?)v.ViewCount, ct) ?? 0,
            MostViewedArticleTitle = mostViewed?.Title,
            MostViewedArticleSlug = mostViewed?.Slug
        };
    }

    public async Task<List<ViewsOverTimeDto>> GetViewsOverTimeAsync(int days, CancellationToken ct = default)
    {
        var since = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        return await _db.ArticleViews
            .Where(v => v.ViewDate >= since)
            .GroupBy(v => v.ViewDate)
            .Select(g => new ViewsOverTimeDto { Date = g.Key, Views = g.Sum(x => x.ViewCount) })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);
    }
}
