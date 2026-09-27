namespace Narendra4News.Domain.Entities;

// One immutable row per reported collection day/milestone. Day 1, Day 2,
// Weekend etc. are all separate rows for the same MovieId so historical
// numbers are never overwritten - see spec section 33/34.
public class MovieCollection : BaseEntity
{
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    public DateTime CollectionDate { get; set; }
    public int? DayNumber { get; set; }
    public string? Label { get; set; } // e.g. "Weekend", "Week 1", "Lifetime"

    public decimal? IndiaNet { get; set; }
    public decimal? IndiaGross { get; set; }
    public decimal? Overseas { get; set; }
    public decimal? WorldwideGross { get; set; }
    public decimal? OpeningDay { get; set; }
    public decimal? WeekendCollection { get; set; }
    public decimal? TotalCollection { get; set; }

    public string? Notes { get; set; }
    public string Currency { get; set; } = "INR";
}
