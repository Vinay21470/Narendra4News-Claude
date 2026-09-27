using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.DTOs.Media;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _db;
    private readonly IBlobStorageService _blobStorage;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private const long MaxSizeBytes = 8 * 1024 * 1024; // 8 MB

    public MediaService(ApplicationDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }

    public async Task<MediaDto> UploadAsync(Stream content, string fileName, string contentType, long sizeBytes, string userId, CancellationToken ct = default)
    {
        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException("Only JPG, JPEG, PNG and WEBP images are allowed.");
        if (sizeBytes > MaxSizeBytes)
            throw new InvalidOperationException("File exceeds the 8 MB upload limit.");

        if (sizeBytes <= 0) throw new InvalidOperationException("Image is empty.");
        // Bound memory use and check the file signature rather than trusting the browser MIME type.
        using var validated = new MemoryStream();
        var buffer = new byte[81920];
        int read;
        while ((read = await content.ReadAsync(buffer, ct)) > 0)
        {
            if (validated.Length + read > MaxSizeBytes) throw new InvalidOperationException("File exceeds the 8 MB upload limit.");
            await validated.WriteAsync(buffer.AsMemory(0, read), ct);
        }
        var bytes = validated.ToArray();
        var detected = bytes.Length >= 8 && bytes.AsSpan(0, 8).SequenceEqual(new byte[] {137,80,78,71,13,10,26,10}) ? "image/png"
            : bytes.Length >= 3 && bytes[0] == 255 && bytes[1] == 216 && bytes[2] == 255 ? "image/jpeg"
            : bytes.Length >= 12 && System.Text.Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP" ? "image/webp" : null;
        if (detected is null || (contentType.ToLowerInvariant().Replace("image/jpg", "image/jpeg") != detected))
            throw new InvalidOperationException("File contents must match a JPG, PNG or WEBP image.");
        contentType = detected;
        sizeBytes = validated.Length;
        validated.Position = 0;
        var (blobUrl, storedFileName) = await _blobStorage.UploadAsync(validated, fileName, contentType, ct);

        var media = new Media
        {
            FileName = storedFileName,
            OriginalFileName = fileName,
            BlobUrl = blobUrl,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            UploadedByUserId = userId
        };
        _db.MediaItems.Add(media);
        await _db.SaveChangesAsync(ct);

        return new MediaDto
        {
            Id = media.Id, FileName = media.FileName, OriginalFileName = media.OriginalFileName,
            BlobUrl = media.BlobUrl, ContentType = media.ContentType, SizeBytes = media.SizeBytes,
            CreatedDate = media.CreatedDate
        };
    }

    public async Task<List<MediaDto>> GetAllAsync(CancellationToken ct = default) =>
        await _db.MediaItems.OrderByDescending(m => m.CreatedDate)
            .Select(m => new MediaDto
            {
                Id = m.Id, FileName = m.FileName, OriginalFileName = m.OriginalFileName, BlobUrl = m.BlobUrl,
                ContentType = m.ContentType, SizeBytes = m.SizeBytes, CreatedDate = m.CreatedDate
            }).ToListAsync(ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var media = await _db.MediaItems.FindAsync([id], ct);
        if (media is null) return;
        await _blobStorage.DeleteAsync(media.BlobUrl, ct);
        _db.MediaItems.Remove(media);
        await _db.SaveChangesAsync(ct);
    }
}
