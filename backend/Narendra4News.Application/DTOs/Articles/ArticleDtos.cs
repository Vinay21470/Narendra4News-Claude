using Narendra4News.Domain.Enums;

namespace Narendra4News.Application.DTOs.Articles;

public class ArticleListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public DateTime? PublishedDate { get; set; }
    public long Views { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsTrending { get; set; }
}

public class ArticleDetailDto : ArticleListItemDto
{
    public int CategoryId { get; set; }
    public int? MovieId { get; set; }
    public ArticleStatus Status { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime? UpdatedDate { get; set; }
    public long Likes { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? MovieSlug { get; set; }
    public List<ArticleListItemDto> RelatedArticles { get; set; } = new();
}

public class CreateArticleRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; } // optional - auto-generated from Title if omitted
    public string ShortDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int? MovieId { get; set; }
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    public DateTime? PublishedDate { get; set; } // used for "Schedule"
    public bool IsFeatured { get; set; }
    public bool IsTrending { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
}

public class UpdateArticleRequest : CreateArticleRequest { }

public class ArticleQueryParams
{
    public string? CategorySlug { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsTrending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
