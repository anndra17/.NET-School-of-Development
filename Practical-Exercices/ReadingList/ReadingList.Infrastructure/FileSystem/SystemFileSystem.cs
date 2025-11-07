using ReadingList.Application.Abstractions.IO;

namespace ReadingList.Infrastructure.FileSystem;

public sealed class SystemFileSystem: IFileSystem
{
    public bool FileExists(string path)
        => File.Exists(path);

    public bool DirectoryExists(string path)
        => Directory.Exists(path);

    public void CreateDirectoryIfMissing(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }

    public async Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct = default)
    {
        return await File.ReadAllLinesAsync(path, ct);
    }

    public Task<Stream> OpenReadAsync(string path, CancellationToken ct = default)
    {
        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        return Task.FromResult(stream);
    }

    public Task<Stream> OpenWriteAsync(string path, bool overwrite = false, CancellationToken ct = default)
    {
        var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
        Stream stream = new FileStream(path, mode, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        return Task.FromResult(stream);
    }
}
