using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Articles;

namespace Narendra4News.Application.Interfaces;

public interface IArticleService
{
    Task<PagedResult<ArticleListItemDto>> GetPublishedAsync(ArticleQueryParams query, CancellationToken ct = default);
    Task<ArticleDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<ArticleListItemDto>> GetTrendingAsync(int count, CancellationToken ct = default);
    Task<List<ArticleListItemDto>> GetFeaturedAsync(int count, CancellationToken ct = default);

    Task<ArticleDetailDto?> GetForAdminAsync(int id, CancellationToken ct = default);

    // Admin-only
    Task<PagedResult<ArticleListItemDto>> GetAllForAdminAsync(int page, int pageSize, CancellationToken ct = default);
    Task<ArticleDetailDto> CreateAsync(CreateArticleRequest request, string authorId, CancellationToken ct = default);
    Task<ArticleDetailDto> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task SetStatusAsync(int id, Domain.Enums.ArticleStatus status, CancellationToken ct = default);
}
