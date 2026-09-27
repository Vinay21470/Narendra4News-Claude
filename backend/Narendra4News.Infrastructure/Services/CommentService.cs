using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.DTOs.Comments;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Domain.Enums;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;
    public CommentService(ApplicationDbContext db) => _db = db;

    public async Task<List<CommentDto>> GetApprovedForArticleAsync(int articleId, CancellationToken ct = default) =>
        await _db.Comments.Include(c => c.User)
            .Where(c => c.ArticleId == articleId && c.Status == CommentStatus.Approved)
            .OrderByDescending(c => c.CreatedDate)
            .Select(c => new CommentDto
            {
                Id = c.Id, Content = c.Content, UserDisplayName = c.User!.DisplayName,
                CreatedDate = c.CreatedDate, ParentCommentId = c.ParentCommentId
            }).ToListAsync(ct);

    public async Task<CommentDto> CreateAsync(int articleId, string userId, CreateCommentRequest request, CancellationToken ct = default)
    {
        var comment = new Comment
        {
            ArticleId = articleId, UserId = userId, Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            // Moderation-first: comments never show publicly until approved (spec section 30).
            Status = CommentStatus.Pending
        };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(ct);
        var user = await _db.Users.FindAsync([userId], ct);
        return new CommentDto { Id = comment.Id, Content = comment.Content, UserDisplayName = user?.DisplayName ?? "", CreatedDate = comment.CreatedDate, ParentCommentId = comment.ParentCommentId };
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var comment = await _db.Comments.FindAsync([id], ct);
        if (comment is not null) { _db.Comments.Remove(comment); await _db.SaveChangesAsync(ct); }
    }

    public async Task ModerateAsync(int id, CommentStatus status, CancellationToken ct = default)
    {
        var comment = await _db.Comments.FindAsync([id], ct) ?? throw new KeyNotFoundException("Comment not found");
        comment.Status = status;
        comment.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}
