using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Movies;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ApplicationDbContext _db;
    public MovieService(ApplicationDbContext db) => _db = db;

    private static MovieListItemDto ToListItem(Movie m) => new()
    {
        Id = m.Id, MovieName = m.MovieName, Slug = m.Slug, PosterUrl = m.PosterUrl,
        ReleaseDate = m.ReleaseDate, Genre = m.Genre
    };

    private static MovieCollectionDto ToCollectionDto(MovieCollection c) => new()
    {
        Id = c.Id, CollectionDate = c.CollectionDate, DayNumber = c.DayNumber, Label = c.Label,
        IndiaNet = c.IndiaNet, IndiaGross = c.IndiaGross, Overseas = c.Overseas,
        WorldwideGross = c.WorldwideGross, OpeningDay = c.OpeningDay,
        WeekendCollection = c.WeekendCollection, TotalCollection = c.TotalCollection, Notes = c.Notes
    };

    public async Task<PagedResult<MovieListItemDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var q = _db.Movies.OrderByDescending(m => m.ReleaseDate);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<MovieListItemDto> { Items = items.Select(ToListItem).ToList(), Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<MovieDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var movie = await _db.Movies
            .Include(m => m.Collections.OrderBy(c => c.CollectionDate))
            .FirstOrDefaultAsync(m => m.Slug == slug, ct);
        if (movie is null) return null;

        return new MovieDetailDto
        {
            Id = movie.Id, MovieName = movie.MovieName, Slug = movie.Slug, PosterUrl = movie.PosterUrl,
            HeroImageUrl = movie.HeroImageUrl, Description = movie.Description, ReleaseDate = movie.ReleaseDate,
            Hero = movie.Hero, Director = movie.Director, Producer = movie.Producer,
            ProductionHouse = movie.ProductionHouse, Genre = movie.Genre, Language = movie.Language,
            Budget = movie.Budget, RuntimeMinutes = movie.RuntimeMinutes, Certification = movie.Certification,
            Collections = movie.Collections.Select(ToCollectionDto).ToList()
        };
    }

    public async Task<MovieDetailDto> CreateAsync(UpsertMovieRequest request, CancellationToken ct = default)
    {
        var slug = await EnsureUniqueSlugAsync(string.IsNullOrWhiteSpace(request.Slug) ? request.MovieName : request.Slug, ct);
        var movie = new Movie
        {
            MovieName = request.MovieName, Slug = slug, HeroImageUrl = request.HeroImageUrl,
            PosterUrl = request.PosterUrl, Description = request.Description, ReleaseDate = request.ReleaseDate,
            Hero = request.Hero, Director = request.Director, Producer = request.Producer,
            ProductionHouse = request.ProductionHouse, Genre = request.Genre, Language = request.Language,
            Budget = request.Budget, RuntimeMinutes = request.RuntimeMinutes, Certification = request.Certification
        };
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync(ct);
        return (await GetBySlugAsync(movie.Slug, ct))!;
    }

    public async Task<MovieDetailDto> UpdateAsync(int id, UpsertMovieRequest request, CancellationToken ct = default)
    {
        var movie = await _db.Movies.FindAsync([id], ct) ?? throw new KeyNotFoundException("Movie not found");
        movie.MovieName = request.MovieName;
        movie.HeroImageUrl = request.HeroImageUrl;
        movie.PosterUrl = request.PosterUrl;
        movie.Description = request.Description;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Hero = request.Hero;
        movie.Director = request.Director;
        movie.Producer = request.Producer;
        movie.ProductionHouse = request.ProductionHouse;
        movie.Genre = request.Genre;
        movie.Language = request.Language;
        movie.Budget = request.Budget;
        movie.RuntimeMinutes = request.RuntimeMinutes;
        movie.Certification = request.Certification;
        movie.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return (await GetBySlugAsync(movie.Slug, ct))!;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var movie = await _db.Movies.FindAsync([id], ct);
        if (movie is not null) { _db.Movies.Remove(movie); await _db.SaveChangesAsync(ct); }
    }

    public async Task<List<MovieCollectionDto>> GetCollectionsAsync(int movieId, CancellationToken ct = default) =>
        await _db.MovieCollections.Where(c => c.MovieId == movieId)
            .OrderBy(c => c.CollectionDate)
            .Select(c => c)
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.Select(ToCollectionDto).ToList(), ct);

    public async Task<MovieCollectionDto> AddCollectionAsync(int movieId, UpsertMovieCollectionRequest request, CancellationToken ct = default)
    {
        if (!await _db.Movies.AnyAsync(m => m.Id == movieId, ct))
            throw new KeyNotFoundException("Movie not found");

        // Always inserts a NEW row - historical Day 1/2/3/... collection
        // records are never overwritten (spec section 33/34).
        var collection = new MovieCollection
        {
            MovieId = movieId, CollectionDate = request.CollectionDate, DayNumber = request.DayNumber,
            Label = request.Label, IndiaNet = request.IndiaNet, IndiaGross = request.IndiaGross,
            Overseas = request.Overseas, WorldwideGross = request.WorldwideGross, OpeningDay = request.OpeningDay,
            WeekendCollection = request.WeekendCollection, TotalCollection = request.TotalCollection, Notes = request.Notes
        };
        _db.MovieCollections.Add(collection);
        await _db.SaveChangesAsync(ct);
        return ToCollectionDto(collection);
    }

    public async Task<MovieCollectionDto> UpdateCollectionAsync(int collectionId, UpsertMovieCollectionRequest request, CancellationToken ct = default)
    {
        var collection = await _db.MovieCollections.FindAsync([collectionId], ct) ?? throw new KeyNotFoundException("Collection record not found");
        collection.CollectionDate = request.CollectionDate;
        collection.DayNumber = request.DayNumber;
        collection.Label = request.Label;
        collection.IndiaNet = request.IndiaNet;
        collection.IndiaGross = request.IndiaGross;
        collection.Overseas = request.Overseas;
        collection.WorldwideGross = request.WorldwideGross;
        collection.OpeningDay = request.OpeningDay;
        collection.WeekendCollection = request.WeekendCollection;
        collection.TotalCollection = request.TotalCollection;
        collection.Notes = request.Notes;
        collection.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ToCollectionDto(collection);
    }

    public async Task DeleteCollectionAsync(int collectionId, CancellationToken ct = default)
    {
        var collection = await _db.MovieCollections.FindAsync([collectionId], ct);
        if (collection is not null) { _db.MovieCollections.Remove(collection); await _db.SaveChangesAsync(ct); }
    }

    private async Task<string> EnsureUniqueSlugAsync(string source, CancellationToken ct)
    {
        var baseSlug = DbInitializer.Slugify(source);
        var slug = baseSlug;
        var i = 1;
        while (await _db.Movies.AnyAsync(m => m.Slug == slug, ct))
            slug = $"{baseSlug}-{++i}";
        return slug;
    }
}
