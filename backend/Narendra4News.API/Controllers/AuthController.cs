using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Auth;
using Narendra4News.Domain.Entities;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, DisplayName = request.DisplayName, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<AuthResponse>.Fail(string.Join(" ", result.Errors.Select(e => e.Description))));

        // Registered visitors get the plain "User" role - only seeded admins
        // (or an admin promoting someone via /admin/users) get "Admin".
        await _userManager.AddToRoleAsync(user, UserRoles.User);

        var token = await GenerateTokenAsync(user);
        return Ok(ApiResponse<AuthResponse>.Ok(token, "Registered successfully"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid email or password"));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid email or password"));

        var token = await GenerateTokenAsync(user);
        return Ok(ApiResponse<AuthResponse>.Ok(token, "Login successful"));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CurrentUserDto>>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(ApiResponse<CurrentUserDto>.Fail("User not found"));
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<CurrentUserDto>.Ok(new CurrentUserDto
        {
            UserId = user.Id, Email = user.Email!, DisplayName = user.DisplayName, Roles = roles.ToList()
        }));
    }

    private async Task<AuthResponse> GenerateTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var jwtSecret = _config["JWT_SECRET"]!;
        var issuer = _config["JWT_ISSUER"] ?? "Narendra4News";
        var audience = _config["JWT_AUDIENCE"] ?? "Narendra4NewsClient";
        var expires = DateTime.UtcNow.AddHours(12);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.DisplayName)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expires,
            UserId = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Roles = roles.ToList()
        };
    }
}
