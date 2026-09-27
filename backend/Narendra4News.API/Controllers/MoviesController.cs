using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Movies;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    public MoviesController(IMovieService movieService) => _movieService = movieService;

    [HttpGet("movies")]
    public async Task<ActionResult<ApiResponse<PagedResult<MovieListItemDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 12, CancellationToken ct = default)
    {
        var result = await _movieService.GetAllAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<MovieListItemDto>>.Ok(result));
    }

    [HttpGet("movies/{slug}")]
    public async Task<ActionResult<ApiResponse<MovieDetailDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var movie = await _movieService.GetBySlugAsync(slug, ct);
        if (movie is null) return NotFound(ApiResponse<MovieDetailDto>.Fail("Movie not found"));
        return Ok(ApiResponse<MovieDetailDto>.Ok(movie));
    }

    [HttpPost("movies")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<MovieDetailDto>>> Create(UpsertMovieRequest request, CancellationToken ct)
    {
        var movie = await _movieService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = movie.Slug }, ApiResponse<MovieDetailDto>.Ok(movie, "Movie created successfully"));
    }

    [HttpPut("movies/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<MovieDetailDto>>> Update(int id, UpsertMovieRequest request, CancellationToken ct)
    {
        var movie = await _movieService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<MovieDetailDto>.Ok(movie, "Movie updated successfully"));
    }

    [HttpDelete("movies/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _movieService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Movie deleted successfully"));
    }

    // --- Box office collections ---

    [HttpGet("movies/{movieId:int}/collections")]
    public async Task<ActionResult<ApiResponse<List<MovieCollectionDto>>>> GetCollections(int movieId, CancellationToken ct)
    {
        var result = await _movieService.GetCollectionsAsync(movieId, ct);
        return Ok(ApiResponse<List<MovieCollectionDto>>.Ok(result));
    }

    [HttpPost("movies/{movieId:int}/collections")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<MovieCollectionDto>>> AddCollection(int movieId, UpsertMovieCollectionRequest request, CancellationToken ct)
    {
        var result = await _movieService.AddCollectionAsync(movieId, request, ct);
        return Ok(ApiResponse<MovieCollectionDto>.Ok(result, "Collection record added"));
    }

    [HttpPut("collections/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<MovieCollectionDto>>> UpdateCollection(int id, UpsertMovieCollectionRequest request, CancellationToken ct)
    {
        var result = await _movieService.UpdateCollectionAsync(id, request, ct);
        return Ok(ApiResponse<MovieCollectionDto>.Ok(result, "Collection record updated"));
    }

    [HttpDelete("collections/{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCollection(int id, CancellationToken ct)
    {
        await _movieService.DeleteCollectionAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Collection record deleted"));
    }
}
