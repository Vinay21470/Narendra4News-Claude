using Azure.Storage.Blobs;
using Azure;
using Microsoft.Extensions.Configuration;
using Narendra4News.Application.Interfaces;

namespace Narendra4News.Infrastructure.Services;

// Uploads go straight to Azure Blob Storage; only the resulting URL is
// persisted in Azure SQL (spec section 10/11) - never raw image bytes.
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _publicApiUrl;

    public BlobStorageService(IConfiguration config)
    {
        // AZURE_STORAGE_CONNECTION_STRING comes from Azure App Service
        // configuration / Key Vault in production, User Secrets locally -
        // never committed to source (spec section 16/24).
        var connectionString = config["AZURE_STORAGE_CONNECTION_STRING"]
            ?? throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING is not configured.");
        var containerName = config["AZURE_STORAGE_CONTAINER"] ?? "media";

        _publicApiUrl = config["PUBLIC_API_URL"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("PUBLIC_API_URL must be configured, for example https://your-api.azurewebsites.net");
        var serviceClient = new BlobServiceClient(connectionString);
        _containerClient = serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task<(string blobUrl, string storedFileName)> UploadAsync(
        Stream content, string originalFileName, string contentType, CancellationToken ct = default)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var extension = contentType.ToLowerInvariant() switch { "image/png" => ".png", "image/webp" => ".webp", _ => ".jpg" };
        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        var blobClient = _containerClient.GetBlobClient(storedFileName);
        await blobClient.UploadAsync(content, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return ($"{_publicApiUrl}/api/media/files/{storedFileName}", storedFileName);
    }

    public async Task<(Stream content, string contentType)?> OpenAsync(string fileName, CancellationToken ct = default)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(fileName, @"^[a-f0-9]{32}\.(jpg|png|webp)$")) return null;
        try
        {
            var result = await _containerClient.GetBlobClient(fileName).DownloadStreamingAsync(cancellationToken: ct);
            return (result.Value.Content, result.Value.Details.ContentType);
        }
        catch (RequestFailedException ex) when (ex.Status == 404) { return null; }
    }

    public async Task DeleteAsync(string blobUrl, CancellationToken ct = default)
    {
        var fileName = blobUrl.Split('/').Last();
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }
}
