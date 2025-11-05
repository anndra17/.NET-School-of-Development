
namespace ReadingList.Infrastructure.Interfaces;

public interface IFileSystem
{
    bool FileExists(string path);
    bool DirectoryExists(string path);
    void CreateDirectoryIfMissing(string directoryPath);

    Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);
    Task<Stream> OpenWriteAsync(string path, bool overwrite = false, CancellationToken cancellationToken = default);
}
