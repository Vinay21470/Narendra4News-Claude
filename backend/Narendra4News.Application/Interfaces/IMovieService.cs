using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Movies;

namespace Narendra4News.Application.Interfaces;

public interface IMovieService
{
    Task<PagedResult<MovieListItemDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<MovieDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<MovieDetailDto> CreateAsync(UpsertMovieRequest request, CancellationToken ct = default);
    Task<MovieDetailDto> UpdateAsync(int id, UpsertMovieRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<List<MovieCollectionDto>> GetCollectionsAsync(int movieId, CancellationToken ct = default);
    Task<MovieCollectionDto> AddCollectionAsync(int movieId, UpsertMovieCollectionRequest request, CancellationToken ct = default);
    Task<MovieCollectionDto> UpdateCollectionAsync(int collectionId, UpsertMovieCollectionRequest request, CancellationToken ct = default);
    Task DeleteCollectionAsync(int collectionId, CancellationToken ct = default);
}
