using ReadingList.Domain.Entities;
using ReadingList.Domain.Results;
using ReadingList.Infrastructure.Interfaces;
using ReadingList.Infrastructure.Utils;
using System.Runtime.CompilerServices;

namespace ReadingList.Infrastructure.Import;

public sealed class CsvBookParser : ICsvBookParser
{
    private readonly ICsvReader _reader;
    private readonly ILogger _logger;

    public CsvBookParser(ICsvReader reader, ILogger logger)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async IAsyncEnumerable<ParsedBook> ParseFileAsync(
            string path,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var row in _reader.ReadAsync(path, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var res = TryMapRowToBook(row.Cells, out var book, out var error);

            if (!res)
            {
                _logger.Warn($"[{path}] Line {row.LineNumber}: {error}");
                yield return new ParsedBook(row.LineNumber, Result<Book>.Fail(error));
                continue;
            }

            yield return new ParsedBook(row.LineNumber, Result<Book>.Ok(book!));
        }
    }

    private static bool TryMapRowToBook(
            IReadOnlyList<string> cells,
            out Book? book,
            out string error)
    {
        book = null;
        error = string.Empty;

        if (cells.Count < CsvValidationUtils.ExpectedColumnNumber)
        {
            error = $"Insufficient number of colums (expected {CsvValidationUtils.ExpectedColumnNumber}).";
            return false;
        }

        if (!CsvValidationUtils.TryParseId(cells[0], out var id, out error)) return false;
        if (!CsvValidationUtils.TryParseNonEmpty(cells[1], "Title", out var title, out error)) return false;
        if (!CsvValidationUtils.TryParseNonEmpty(cells[2], "Author", out var author, out error)) return false;
        if (!CsvValidationUtils.TryParseYear(cells[3], out var year, out error)) return false;
        if (!CsvValidationUtils.TryParsePages(cells[4], out var pages, out error)) return false;
        var genre = CsvValidationUtils.Safe(cells[5]);
        if (!CsvValidationUtils.TryParseFinished(cells[6], out var finished))
        {
            error = $"Invalid value for finished field: '{cells[6]}' (expected: yes/no/y/n)";
            return false;
        }
        if (!CsvValidationUtils.TryParseRating(cells[7], out var rating, out error)) return false;

        var created = Book.Create(id, title, author, year, pages, genre, rating, finished);
        if (!created.IsSuccess)
        {
            error = created.Error ?? "Unknown error at Book creation.";
            return false;
        }

        book = created.Value!;
        return true;
    }
}
