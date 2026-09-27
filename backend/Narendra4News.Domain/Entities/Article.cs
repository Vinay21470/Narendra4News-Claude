using Narendra4News.Domain.Enums;

namespace Narendra4News.Domain.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }

    // Nullable: an article about a specific movie (e.g. box office collection
    // posts) links back to the Movie; general news articles leave this null.
    public int? MovieId { get; set; }
    public Movie? Movie { get; set; }

    public DateTime? PublishedDate { get; set; }
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public bool IsFeatured { get; set; }
    public bool IsTrending { get; set; }

    // Denormalized fast counter, incremented on read. Detailed per-view
    // analytics rows live in ArticleView and are aggregated separately so
    // this column stays cheap to read on every card/list render.
    public long Views { get; set; }
    public long Likes { get; set; }

    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ArticleView> ViewRecords { get; set; } = new List<ArticleView>();
    public ICollection<Like> LikeRecords { get; set; } = new List<Like>();
}
