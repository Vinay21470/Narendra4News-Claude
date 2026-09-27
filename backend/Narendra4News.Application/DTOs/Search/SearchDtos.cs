namespace Narendra4News.Application.DTOs.Search;

public class SearchResultDto
{
    public string Type { get; set; } = string.Empty; // "Article" | "Movie"
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Excerpt { get; set; }
    public DateTime? Date { get; set; }
}
