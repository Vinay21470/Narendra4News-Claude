namespace Narendra4News.Domain.Entities;

// Aggregated per-day view counter (ArticleId + ViewDate is unique) instead of
// one row per page load, so traffic spikes don't create millions of rows.
// See spec section 29 ("avoid one row per refresh").
public class ArticleView : BaseEntity
{
    public int ArticleId { get; set; }
    public Article? Article { get; set; }

    public DateOnly ViewDate { get; set; }
    public long ViewCount { get; set; }
}
