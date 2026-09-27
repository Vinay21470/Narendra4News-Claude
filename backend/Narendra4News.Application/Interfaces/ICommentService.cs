using Narendra4News.Application.DTOs.Comments;
using Narendra4News.Domain.Enums;

namespace Narendra4News.Application.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetApprovedForArticleAsync(int articleId, CancellationToken ct = default);
    Task<CommentDto> CreateAsync(int articleId, string userId, CreateCommentRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task ModerateAsync(int id, CommentStatus status, CancellationToken ct = default);
}
