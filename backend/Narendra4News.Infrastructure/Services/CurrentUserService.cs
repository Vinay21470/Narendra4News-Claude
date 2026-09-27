using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;
    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public string? UserId => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => _accessor.HttpContext?.User?.IsInRole(UserRoles.Admin) ?? false;
}
