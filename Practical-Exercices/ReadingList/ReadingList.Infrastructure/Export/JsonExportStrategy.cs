using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.Domain.Entities;
using ReadingList.Domain.Results;
using System.Text.Json;

namespace ReadingList.Infrastructure.Export;

public sealed class JsonExportStrategy : IExportStrategy
{
    private readonly IFileSystem _fileSystem;
    private readonly ISystemLogger _systemLogger;
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonExportStrategy(IFileSystem fileSystem, ISystemLogger systemLogger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
    }

    public async Task<Result> ExportAsync(
        IEnumerable<Book> books,
        string path,
        bool overwriteAllowed,
        CancellationToken cancellationToken)
    {
        if (books is null) throw new ArgumentNullException(nameof(books));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path is required.", nameof(path));

        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                _fileSystem.CreateDirectoryIfMissing(directory);

            await using var stream = await _fileSystem.OpenWriteAsync(path, overwriteAllowed, cancellationToken);

            var payload = books.Select(b => new
            {
                id = b.Id,
                title = b.Title,
                author = b.Author,
                year = b.Year,
                pages = b.Pages,
                genre = b.Genre,
                finished = b.Finished,
                rating = b.Rating
            });

            await JsonSerializer.SerializeAsync(stream, payload, Options, cancellationToken);
            return Domain.Results.Result.Ok();
        }
        catch (IOException e)
        {
            _systemLogger.Error($"JSON export failed: {e.Message}");
            return Result.Fail($"JSON export failed: {e.Message}");
        }
    }
}
