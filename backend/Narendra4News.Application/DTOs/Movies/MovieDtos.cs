namespace Narendra4News.Application.DTOs.Movies;

public class MovieListItemDto
{
    public int Id { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Genre { get; set; }
}

public class MovieDetailDto : MovieListItemDto
{
    public string? HeroImageUrl { get; set; }
    public string? Description { get; set; }
    public string? Hero { get; set; }
    public string? Director { get; set; }
    public string? Producer { get; set; }
    public string? ProductionHouse { get; set; }
    public string? Language { get; set; }
    public decimal? Budget { get; set; }
    public int? RuntimeMinutes { get; set; }
    public string? Certification { get; set; }
    public List<MovieCollectionDto> Collections { get; set; } = new();
}

public class MovieCollectionDto
{
    public int Id { get; set; }
    public DateTime CollectionDate { get; set; }
    public int? DayNumber { get; set; }
    public string? Label { get; set; }
    public decimal? IndiaNet { get; set; }
    public decimal? IndiaGross { get; set; }
    public decimal? Overseas { get; set; }
    public decimal? WorldwideGross { get; set; }
    public decimal? OpeningDay { get; set; }
    public decimal? WeekendCollection { get; set; }
    public decimal? TotalCollection { get; set; }
    public string? Notes { get; set; }
}

public class UpsertMovieRequest
{
    public string MovieName { get; set; } = string.Empty;
    public string? Slug { get; set; }
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
}

// Each collection row is created independently and never overwrites a
// previous day's numbers (spec section 33/34) - update only targets its own row.
public class UpsertMovieCollectionRequest
{
    public DateTime CollectionDate { get; set; }
    public int? DayNumber { get; set; }
    public string? Label { get; set; }
    public decimal? IndiaNet { get; set; }
    public decimal? IndiaGross { get; set; }
    public decimal? Overseas { get; set; }
    public decimal? WorldwideGross { get; set; }
    public decimal? OpeningDay { get; set; }
    public decimal? WeekendCollection { get; set; }
    public decimal? TotalCollection { get; set; }
    public string? Notes { get; set; }
}
