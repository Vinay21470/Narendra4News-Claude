using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Search;
using Narendra4News.Application.Interfaces;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    public SearchController(ISearchService searchService) => _searchService = searchService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SearchResultDto>>>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 12, CancellationToken ct = default)
    {
        var result = await _searchService.SearchAsync(q, page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<SearchResultDto>>.Ok(result));
    }
}
