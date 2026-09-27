using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Narendra4News.Infrastructure.Data;
using Narendra4News.Infrastructure.Services;
var container = "smoke" + Guid.NewGuid().ToString("N");
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> {
    ["AZURE_STORAGE_CONNECTION_STRING"] = "UseDevelopmentStorage=true",
    ["AZURE_STORAGE_CONTAINER"] = container,
    ["PUBLIC_API_URL"] = "https://example.test"
}).Build();
var blobs = new BlobStorageService(config);
var client = new BlobServiceClient("UseDevelopmentStorage=true").GetBlobContainerClient(container);
var png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII=");
try {
    var (url, name) = await blobs.UploadAsync(new MemoryStream(png), "../../bad-name.png", "image/png");
    if (url != $"https://example.test/api/media/files/{name}" || name.Contains('/')) throw new Exception("Incorrect image URL");
    var props = await client.GetPropertiesAsync();
    if (props.Value.PublicAccess != Azure.Storage.Blobs.Models.PublicAccessType.None) throw new Exception("Container is public");
    var image = await blobs.OpenAsync(name) ?? throw new Exception("Image not found");
    using var copy = new MemoryStream();
    await image.content.CopyToAsync(copy); await image.content.DisposeAsync();
    if (!copy.ToArray().SequenceEqual(png) || image.contentType != "image/png") throw new Exception("Image changed");
    if (await blobs.OpenAsync("../private") is not null) throw new Exception("Invalid path accepted");
    await blobs.DeleteAsync(url);
    if (await blobs.OpenAsync(name) is not null) throw new Exception("Image not deleted");
    using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Server=localhost;Database=unused;Integrated Security=True").Options);
    var media = new MediaService(db, blobs);
    foreach (var bytes in new[] { Array.Empty<byte>(), new byte[] { 1, 2, 3 }, new byte[8 * 1024 * 1024 + 1] }) {
        try { await media.UploadAsync(new MemoryStream(bytes), "fake.png", "image/png", bytes.Length, "test"); throw new Exception("Invalid upload accepted"); }
        catch (InvalidOperationException) { }
    }
    Console.WriteLine("PASS: private Blob upload, stable image URL, byte-preserving download, deletion, invalid path and upload rejection.");
} finally { await client.DeleteIfExistsAsync(); }
