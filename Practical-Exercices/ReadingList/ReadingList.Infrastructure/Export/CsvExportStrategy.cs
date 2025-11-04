using ReadingList.Domain.Entities;
using ReadingList.Domain.Results;
using ReadingList.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Infrastructure.Export;


public sealed class CsvExportStrategy : IExportStrategy
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public CsvExportStrategy(IFileSystem fileSystem, ILogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.Error($"CSV export failed: {e.Message}");
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