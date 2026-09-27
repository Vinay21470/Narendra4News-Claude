using Narendra4News.Application.DTOs.Admin;

namespace Narendra4News.Application.Interfaces;

public interface IAdminService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<List<ViewsOverTimeDto>> GetViewsOverTimeAsync(int days, CancellationToken ct = default);
}
