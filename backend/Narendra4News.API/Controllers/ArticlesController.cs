using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Articles;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;
    public ArticlesController(IArticleService articleService) => _articleService = articleService;

    // GET /api/articles?categorySlug=box-office&isFeatured=true&page=1&pageSize=12
    // Public, paginated, published-only. Never returns drafts (spec #5/#15).
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleListItemDto>>>> GetPublished([FromQuery] ArticleQueryParams query, CancellationToken ct)
    {
        var result = await _articleService.GetPublishedAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ArticleListItemDto>>.Ok(result));
    }

    [HttpGet("trending")]
    public async Task<ActionResult<ApiResponse<List<ArticleListItemDto>>>> GetTrending([FromQuery] int count = 6, CancellationToken ct = default)
    {
        var result = await _articleService.GetTrendingAsync(count, ct);
        return Ok(ApiResponse<List<ArticleListItemDto>>.Ok(result));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<List<ArticleListItemDto>>>> GetFeatured([FromQuery] int count = 5, CancellationToken ct = default)
    {
        var result = await _articleService.GetFeaturedAsync(count, ct);
        return Ok(ApiResponse<List<ArticleListItemDto>>.Ok(result));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<ArticleDetailDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var article = await _articleService.GetBySlugAsync(slug, ct);
        if (article is null) return NotFound(ApiResponse<ArticleDetailDto>.Fail("Article not found"));
        return Ok(ApiResponse<ArticleDetailDto>.Ok(article));
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleListItemDto>>>> GetAllForAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _articleService.GetAllForAdminAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ArticleListItemDto>>.Ok(result));
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ArticleDetailDto>>> GetForAdmin(int id, CancellationToken ct)
    {
        var article = await _articleService.GetForAdminAsync(id, ct);
        return article is null ? NotFound() : Ok(ApiResponse<ArticleDetailDto>.Ok(article));
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ArticleDetailDto>>> Create(CreateArticleRequest request, CancellationToken ct)
    {
        var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var article = await _articleService.CreateAsync(request, authorId, ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = article.Slug }, ApiResponse<ArticleDetailDto>.Ok(article, "Article created successfully"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ArticleDetailDto>>> Update(int id, UpdateArticleRequest request, CancellationToken ct)
    {
        var article = await _articleService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<ArticleDetailDto>.Ok(article, "Article updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _articleService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Article deleted successfully"));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> SetStatus(int id, [FromBody] ArticleStatus status, CancellationToken ct)
    {
        await _articleService.SetStatusAsync(id, status, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Article status updated"));
    }
}
