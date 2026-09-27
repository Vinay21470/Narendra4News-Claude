using Narendra4News.Domain.Enums;

namespace Narendra4News.Domain.Entities;

public class Comment : BaseEntity
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public string Content { get; set; } = string.Empty;
    public CommentStatus Status { get; set; } = CommentStatus.Pending;

    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
}
