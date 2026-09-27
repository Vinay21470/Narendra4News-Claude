using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Comments;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentsController(ICommentService commentService) => _commentService = commentService;

    [HttpGet("articles/{articleId:int}/comments")]
    public async Task<ActionResult<ApiResponse<List<CommentDto>>>> GetForArticle(int articleId, CancellationToken ct)
    {
        var result = await _commentService.GetApprovedForArticleAsync(articleId, ct);
        return Ok(ApiResponse<List<CommentDto>>.Ok(result));
    }

    [HttpPost("articles/{articleId:int}/comments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CommentDto>>> Create(int articleId, CreateCommentRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var comment = await _commentService.CreateAsync(articleId, userId, request, ct);
        return Ok(ApiResponse<CommentDto>.Ok(comment, "Comment submitted for moderation"));
    }

    [HttpDelete("comments/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _commentService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Comment deleted"));
    }

    [HttpPatch("comments/{id:int}/moderate")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Moderate(int id, [FromBody] CommentStatus status, CancellationToken ct)
    {
        await _commentService.ModerateAsync(id, status, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Comment moderation status updated"));
    }
}
