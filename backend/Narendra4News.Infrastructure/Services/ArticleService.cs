using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Articles;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Domain.Enums;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _db;
    public ArticleService(ApplicationDbContext db) => _db = db;

    private static ArticleListItemDto ToListItem(Article a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Slug = a.Slug,
        ShortDescription = a.ShortDescription,
        FeaturedImageUrl = a.FeaturedImageUrl,
        CategoryName = a.Category?.Name ?? string.Empty,
        CategorySlug = a.Category?.Slug ?? string.Empty,
        PublishedDate = a.PublishedDate,
        Views = a.Views,
        IsFeatured = a.IsFeatured,
        IsTrending = a.IsTrending
    };

    public async Task<PagedResult<ArticleListItemDto>> GetPublishedAsync(ArticleQueryParams query, CancellationToken ct = default)
    {
        var q = _db.Articles.Include(a => a.Category)
            .Where(a => a.Status == ArticleStatus.Published && a.PublishedDate <= DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(query.CategorySlug))
            q = q.Where(a => a.Category!.Slug == query.CategorySlug);
        if (query.IsFeatured.HasValue)
            q = q.Where(a => a.IsFeatured == query.IsFeatured.Value);
        if (query.IsTrending.HasValue)
            q = q.Where(a => a.IsTrending == query.IsTrending.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(a => a.PublishedDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => a)
            .ToListAsync(ct);

        return new PagedResult<ArticleListItemDto>
        {
            Items = items.Select(ToListItem).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<ArticleDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var article = await _db.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.Movie)
            .FirstOrDefaultAsync(a => a.Slug == slug, ct);

        if (article is null || article.Status != ArticleStatus.Published || article.PublishedDate > DateTime.UtcNow || article.PublishedDate is null) return null;

        // Increment the aggregated per-day view counter (upsert), plus the
        // fast denormalized counter on the article itself. See ArticleView
        // entity comment re: avoiding one row per page load.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var viewRow = await _db.ArticleViews.FirstOrDefaultAsync(v => v.ArticleId == article.Id && v.ViewDate == today, ct);
        if (viewRow is null)
            _db.ArticleViews.Add(new ArticleView { ArticleId = article.Id, ViewDate = today, ViewCount = 1 });
        else
            viewRow.ViewCount++;
        article.Views++;
        await _db.SaveChangesAsync(ct);

        var related = await _db.Articles.Include(a => a.Category)
            .Where(a => a.CategoryId == article.CategoryId && a.Id != article.Id && a.Status == ArticleStatus.Published && a.PublishedDate <= DateTime.UtcNow)
            .OrderByDescending(a => a.PublishedDate)
            .Take(4)
            .ToListAsync(ct);

        return new ArticleDetailDto
        {
            CategoryId = article.CategoryId, MovieId = article.MovieId, Status = article.Status,
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            ShortDescription = article.ShortDescription,
            Content = article.Content,
            FeaturedImageUrl = article.FeaturedImageUrl,
            CategoryName = article.Category?.Name ?? string.Empty,
            CategorySlug = article.Category?.Slug ?? string.Empty,
            PublishedDate = article.PublishedDate,
            UpdatedDate = article.UpdatedDate,
            Views = article.Views,
            Likes = article.Likes,
            IsFeatured = article.IsFeatured,
            IsTrending = article.IsTrending,
            AuthorName = article.Author?.DisplayName ?? "Narendra4News Staff",
            SeoTitle = article.SeoTitle,
            SeoDescription = article.SeoDescription,
            SeoKeywords = article.SeoKeywords,
            MovieSlug = article.Movie?.Slug,
            RelatedArticles = related.Select(ToListItem).ToList()
        };
    }

    public async Task<List<ArticleListItemDto>> GetTrendingAsync(int count, CancellationToken ct = default)
    {
        var items = await _db.Articles.Include(a => a.Category)
            .Where(a => a.Status == ArticleStatus.Published && a.PublishedDate <= DateTime.UtcNow)
            .OrderByDescending(a => a.IsTrending)
            .ThenByDescending(a => a.Views)
            .ThenByDescending(a => a.PublishedDate)
            .Take(count)
            .ToListAsync(ct);
        return items.Select(ToListItem).ToList();
    }

    public async Task<List<ArticleListItemDto>> GetFeaturedAsync(int count, CancellationToken ct = default)
    {
        var items = await _db.Articles.Include(a => a.Category)
            .Where(a => a.Status == ArticleStatus.Published && a.IsFeatured && a.PublishedDate <= DateTime.UtcNow)
            .OrderByDescending(a => a.PublishedDate)
            .Take(count)
            .ToListAsync(ct);
        return items.Select(ToListItem).ToList();
    }

    public async Task<PagedResult<ArticleListItemDto>> GetAllForAdminAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var q = _db.Articles.Include(a => a.Category).OrderByDescending(a => a.CreatedDate);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<ArticleListItemDto>
        {
            Items = items.Select(ToListItem).ToList(),
            Page = page, PageSize = pageSize, TotalCount = total
        };
    }

    public async Task<ArticleDetailDto> CreateAsync(CreateArticleRequest request, string authorId, CancellationToken ct = default)
    {
        var slug = await EnsureUniqueSlugAsync(string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug, ct);

        var article = new Article
        {
            Title = request.Title,
            Slug = slug,
            ShortDescription = request.ShortDescription,
            Content = request.Content,
            FeaturedImageUrl = request.FeaturedImageUrl,
            CategoryId = request.CategoryId,
            MovieId = request.MovieId,
            AuthorId = authorId,
            Status = request.Status,
            PublishedDate = request.Status == ArticleStatus.Published
                ? (request.PublishedDate ?? DateTime.UtcNow)
                : request.PublishedDate,
            IsFeatured = request.IsFeatured,
            IsTrending = request.IsTrending,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            SeoKeywords = request.SeoKeywords
        };

        _db.Articles.Add(article);
        await _db.SaveChangesAsync(ct);
        return (await GetBySlugInternalNoViewIncrementAsync(article.Id, ct))!;
    }

    public async Task<ArticleDetailDto> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken ct = default)
    {
        var article = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id, ct)
            ?? throw new KeyNotFoundException("Article not found");

        // Editing never touches the slug once set, so previously-shared/
        // indexed URLs keep working permanently (spec section 33).
        article.Title = request.Title;
        article.ShortDescription = request.ShortDescription;
        article.Content = request.Content;
        article.FeaturedImageUrl = request.FeaturedImageUrl;
        article.CategoryId = request.CategoryId;
        article.MovieId = request.MovieId;
        article.IsFeatured = request.IsFeatured;
        article.IsTrending = request.IsTrending;
        article.SeoTitle = request.SeoTitle;
        article.SeoDescription = request.SeoDescription;
        article.SeoKeywords = request.SeoKeywords;
        article.UpdatedDate = DateTime.UtcNow;

        if (article.Status != ArticleStatus.Published && request.Status == ArticleStatus.Published)
            article.PublishedDate = request.PublishedDate ?? DateTime.UtcNow;
        article.Status = request.Status;

        await _db.SaveChangesAsync(ct);
        return (await GetBySlugInternalNoViewIncrementAsync(article.Id, ct))!;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var article = await _db.Articles.FindAsync([id], ct);
        if (article is not null)
        {
            _db.Articles.Remove(article);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task SetStatusAsync(int id, ArticleStatus status, CancellationToken ct = default)
    {
        var article = await _db.Articles.FindAsync([id], ct) ?? throw new KeyNotFoundException("Article not found");
        article.Status = status;
        if (status == ArticleStatus.Published && article.PublishedDate is null)
            article.PublishedDate = DateTime.UtcNow;
        article.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public Task<ArticleDetailDto?> GetForAdminAsync(int id, CancellationToken ct = default) => GetBySlugInternalNoViewIncrementAsync(id, ct);

    private async Task<ArticleDetailDto?> GetBySlugInternalNoViewIncrementAsync(int id, CancellationToken ct)
    {
        var article = await _db.Articles.Include(a => a.Category).Include(a => a.Author).Include(a => a.Movie)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article is null) return null;
        return new ArticleDetailDto
        {
            CategoryId = article.CategoryId, MovieId = article.MovieId, Status = article.Status,
            Id = article.Id, Title = article.Title, Slug = article.Slug, ShortDescription = article.ShortDescription,
            Content = article.Content, FeaturedImageUrl = article.FeaturedImageUrl,
            CategoryName = article.Category?.Name ?? "", CategorySlug = article.Category?.Slug ?? "",
            PublishedDate = article.PublishedDate, UpdatedDate = article.UpdatedDate, Views = article.Views,
            Likes = article.Likes, IsFeatured = article.IsFeatured, IsTrending = article.IsTrending,
            AuthorName = article.Author?.DisplayName ?? "Narendra4News Staff",
            SeoTitle = article.SeoTitle, SeoDescription = article.SeoDescription, SeoKeywords = article.SeoKeywords,
            MovieSlug = article.Movie?.Slug
        };
    }

    private async Task<string> EnsureUniqueSlugAsync(string source, CancellationToken ct)
    {
        var baseSlug = DbInitializer.Slugify(source);
        var slug = baseSlug;
        var i = 1;
        while (await _db.Articles.AnyAsync(a => a.Slug == slug, ct))
            slug = $"{baseSlug}-{++i}";
        return slug;
    }
}
