namespace Narendra4News.Application.DTOs.Admin;

public class DashboardStatsDto
{
    public int TotalArticles { get; set; }
    public int PublishedArticles { get; set; }
    public int DraftArticles { get; set; }
    public int TotalMovies { get; set; }
    public int TotalUsers { get; set; }
    public long TotalViews { get; set; }
    public long TodaysViews { get; set; }
    public string? MostViewedArticleTitle { get; set; }
    public string? MostViewedArticleSlug { get; set; }
}

public class ViewsOverTimeDto
{
    public DateOnly Date { get; set; }
    public long Views { get; set; }
}
