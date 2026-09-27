namespace Narendra4News.Domain.Entities;

public class Like : BaseEntity
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
