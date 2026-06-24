namespace TownHubPeople.Domain.Interfaces;

public class StoredFile
{
    public string StoredPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
}

public interface IFileStorageService
{
    /// <summary>Persists a file under the given logical folder and returns its storage metadata.</summary>
    Task<StoredFile> SaveAsync(string folder, string fileName, string? contentType, Stream content, CancellationToken ct = default);

    /// <summary>Opens a previously stored file for reading. Returns null if it no longer exists.</summary>
    Task<Stream?> OpenReadAsync(string storedPath, CancellationToken ct = default);

    /// <summary>Deletes a stored file. No-op if it does not exist.</summary>
    Task DeleteAsync(string storedPath, CancellationToken ct = default);
}
