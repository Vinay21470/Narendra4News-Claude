using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Media;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;
using System.Security.Claims;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize(Roles = UserRoles.Admin)]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    public MediaController(IMediaService mediaService) => _mediaService = mediaService;

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<MediaDto>>> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<MediaDto>.Fail("No file provided"));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await using var stream = file.OpenReadStream();

        try
        {
            var media = await _mediaService.UploadAsync(stream, file.FileName, file.ContentType, file.Length, userId, ct);
            return Ok(ApiResponse<MediaDto>.Ok(media, "File uploaded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MediaDto>.Fail(ex.Message));
        }
    }

    [HttpGet("files/{fileName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFile(string fileName, [FromServices] IBlobStorageService storage, CancellationToken ct)
    {
        var image = await storage.OpenAsync(fileName, ct);
        if (image is null) return NotFound();
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers.CacheControl = "public,max-age=3600";
        return File(image.Value.content, image.Value.contentType);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MediaDto>>>> GetAll(CancellationToken ct)
    {
        var result = await _mediaService.GetAllAsync(ct);
        return Ok(ApiResponse<List<MediaDto>>.Ok(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _mediaService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "File deleted successfully"));
    }
}
