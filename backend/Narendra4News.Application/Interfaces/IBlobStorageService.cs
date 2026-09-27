namespace Narendra4News.Application.Interfaces;

public interface IBlobStorageService
{
    /// Uploads a file stream to Azure Blob Storage and returns its application image URL.
    /// The file is renamed to a unique name server-side; the caller's original
    /// name is preserved separately for display purposes only (spec section 11).
    Task<(string blobUrl, string storedFileName)> UploadAsync(
        Stream content, string originalFileName, string contentType, CancellationToken ct = default);

    Task<(Stream content, string contentType)?> OpenAsync(string fileName, CancellationToken ct = default);

    Task DeleteAsync(string blobUrl, CancellationToken ct = default);
}
