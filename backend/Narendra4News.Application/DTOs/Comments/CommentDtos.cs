namespace Narendra4News.Application.DTOs.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int? ParentCommentId { get; set; }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
}
