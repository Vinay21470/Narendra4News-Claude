using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Search;

namespace Narendra4News.Application.Interfaces;

public interface ISearchService
{
    Task<PagedResult<SearchResultDto>> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default);
}
