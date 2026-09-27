using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Admin;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = UserRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> Dashboard(CancellationToken ct)
    {
        var stats = await _adminService.GetDashboardStatsAsync(ct);
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<List<ViewsOverTimeDto>>>> Statistics([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var stats = await _adminService.GetViewsOverTimeAsync(days, ct);
        return Ok(ApiResponse<List<ViewsOverTimeDto>>.Ok(stats));
    }
}
