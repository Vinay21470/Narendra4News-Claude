using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Search;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly ApplicationDbContext _db;
    public SearchService(ApplicationDbContext db) => _db = db;

    // Simple LIKE-based search across title/content/movie name/category for
    // Phase 6. For larger catalogs, swap this for Azure AI Search or SQL
    // full-text indexes without changing the ISearchService contract.
    public async Task<PagedResult<SearchResultDto>> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new PagedResult<SearchResultDto> { Page = page, PageSize = pageSize };

        var like = $"%{query}%";

        var articleResults = _db.Articles.Include(a => a.Category)
            .Where(a => a.Status == ArticleStatus.Published &&
                (EF.Functions.Like(a.Title, like) || EF.Functions.Like(a.Content, like) || EF.Functions.Like(a.Category!.Name, like)))
            .Select(a => new SearchResultDto
            {
                Type = "Article", Title = a.Title, Slug = a.Slug, ImageUrl = a.FeaturedImageUrl,
                Excerpt = a.ShortDescription, Date = a.PublishedDate
            });

        var movieResults = _db.Movies
            .Where(m => EF.Functions.Like(m.MovieName, like))
            .Select(m => new SearchResultDto
            {
                Type = "Movie", Title = m.MovieName, Slug = m.Slug, ImageUrl = m.PosterUrl,
                Excerpt = m.Description, Date = m.ReleaseDate
            });

        var combined = articleResults.Concat(movieResults).OrderByDescending(r => r.Date);

        var total = await combined.CountAsync(ct);
        var items = await combined.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<SearchResultDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }
}
