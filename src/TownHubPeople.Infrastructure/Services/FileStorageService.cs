using Microsoft.Extensions.Configuration;
using TownHubPeople.Domain.Interfaces;

namespace TownHubPeople.Infrastructure.Services;

/// <summary>
/// Local-disk file storage. Files are stored under a configurable base path
/// (defaults to <c>wwwroot/uploads</c>). <see cref="StoredFile.StoredPath"/> is a
/// relative path so it can be persisted and later resolved regardless of the base path.
/// Designed so it can be swapped for blob storage later without changing callers.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration configuration)
    {
        var configured = configuration["FileStorage:BasePath"];
        _basePath = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")
            : configured;
    }

    public async Task<StoredFile> SaveAsync(string folder, string fileName, string? contentType, Stream content, CancellationToken ct = default)
    {
        var safeFolder = SanitizeFolder(folder);
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(safeFolder, uniqueName);
        var absolutePath = Path.Combine(_basePath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var fileStream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream, ct);
        }

        var fileInfo = new FileInfo(absolutePath);

        return new StoredFile
        {
            StoredPath = relativePath.Replace('\\', '/'),
            FileName = Path.GetFileName(fileName),
            ContentType = contentType,
            FileSize = fileInfo.Length
        };
    }

    public Task<Stream?> OpenReadAsync(string storedPath, CancellationToken ct = default)
    {
        var absolutePath = Path.Combine(_basePath, NormalizeRelative(storedPath));
        if (!File.Exists(absolutePath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string storedPath, CancellationToken ct = default)
    {
        var absolutePath = Path.Combine(_basePath, NormalizeRelative(storedPath));
        if (File.Exists(absolutePath))
            File.Delete(absolutePath);
        return Task.CompletedTask;
    }

    private static string SanitizeFolder(string folder)
    {
        var parts = folder.Split('/', '\\', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => string.Concat(p.Where(c => !Path.GetInvalidFileNameChars().Contains(c))));
        return Path.Combine(parts.ToArray());
    }

    private static string NormalizeRelative(string storedPath)
        => storedPath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
}
