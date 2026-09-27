using Narendra4News.Application.DTOs.Media;

namespace Narendra4News.Application.Interfaces;

public interface IMediaService
{
    Task<MediaDto> UploadAsync(Stream content, string fileName, string contentType, long sizeBytes, string userId, CancellationToken ct = default);
    Task<List<MediaDto>> GetAllAsync(CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
