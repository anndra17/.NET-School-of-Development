using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.Domain.Entities;
using ReadingList.Domain.Results;
using System.Globalization;
using System.Text;

namespace ReadingList.Infrastructure.Export;


public sealed class CsvExportStrategy : IExportStrategy
{
    private readonly IFileSystem _fileSystem;
    private readonly ISystemLogger _systemLogger;

    public CsvExportStrategy(IFileSystem fileSystem, ISystemLogger systemLogger)
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
            using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            await writer.WriteLineAsync("Id,Title,Author,Year,Pages,Genre,Finished,Rating");

            foreach (var b in books)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = string.Join(",",
                        EscapeCsv(b.Id.ToString(CultureInfo.InvariantCulture)),
                        EscapeCsv(b.Title),
                        EscapeCsv(b.Author),
                        EscapeCsv(b.Year.ToString(CultureInfo.InvariantCulture)),
                        EscapeCsv(b.Pages.ToString(CultureInfo.InvariantCulture)),
                        EscapeCsv(b.Genre ?? string.Empty),
                        EscapeCsv(b.Finished ? "yes" : "no"),
                        EscapeCsv(b.Rating.ToString(CultureInfo.InvariantCulture))
                    );

                await writer.WriteLineAsync(line);
            }

            await writer.FlushAsync();
            return Result.Ok();
        }
        catch (IOException e)
        {
            _systemLogger.Error($"CSV export failed: {e.Message}");
            return Result.Fail($"CSV export failed: {e.Message}");

        }
    }

    private static string EscapeCsv(string? value)
    {
        if (value is null) return string.Empty;

        var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
        var text = value.Replace("\"", "\"\"");

        return needsQuotes ? $"\"{text}\"" : text;
    }
}