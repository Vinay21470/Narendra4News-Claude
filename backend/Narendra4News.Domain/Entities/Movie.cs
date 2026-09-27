namespace Narendra4News.Domain.Entities;

public class Movie : BaseEntity
{
    public string MovieName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public string? PosterUrl { get; set; }
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Hero { get; set; }
    public string? Director { get; set; }
    public string? Producer { get; set; }
    public string? ProductionHouse { get; set; }
    public string? Genre { get; set; }
    public string? Language { get; set; }
    public decimal? Budget { get; set; }
    public int? RuntimeMinutes { get; set; }
    public string? Certification { get; set; }

    public ICollection<MovieCollection> Collections { get; set; } = new List<MovieCollection>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
