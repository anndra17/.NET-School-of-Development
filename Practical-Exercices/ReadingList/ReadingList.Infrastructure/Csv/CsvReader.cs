using ReadingList.Infrastructure.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReadingList.Infrastructure.Csv;

public sealed class CsvReader : ICsvReader
{
    private static readonly string[] CanonicalHeader =
        { "Id","Title","Author","Year","Pages","Genre","Finished","Rating" };

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public CsvReader(IFileSystem fs, ILogger log)
    {
        _fileSystem = fs ?? throw new ArgumentNullException(nameof(fs));
        _logger = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async IAsyncEnumerable<CsvRow> ReadAsync(
            string path,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_fileSystem.FileExists(path))
        {
            _logger.Warn($"Missing CSV: {path}");
            yield break;
        }

        await using var stream = await _fileSystem.OpenReadAsync(path, cancellationToken);
        using var reader = new StreamReader(
            stream, 
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 
            detectEncodingFromByteOrderMarks: true);

        string? line;
        var lineNumber = 0;
        var headerProcessed = false;

        while ((line = await reader.ReadLineAsync()) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cells;
            try
            {
                cells = CsvTokenizer.Split(line, delimiter: ',', quoteChar: '"', trimUnquotedFields: true);
            }
            catch (FormatException ex)
            {
                _logger.Warn($"[{path}] Line {lineNumber}: {ex.Message}");
                continue;
            }

            if (!headerProcessed)
            {
                if (LooksLikeHeader(cells))
                {
                    headerProcessed = true;
                    if (!HeaderMatchesCanonical(cells))
                    {
                        _logger.Warn($"[{path}] Different header at line {lineNumber} from the canonical one. Continues with positions in the order found.");
                    }
                    continue;
                }
                else
                {
                    _logger.Warn($"[{path}] Missing header; The first line of data is {lineNumber}.");
                    headerProcessed = true;
                }
            }

            yield return new CsvRow(lineNumber, cells);
        }
    }

    private static bool LooksLikeHeader(IReadOnlyList<string> cells)
    {
        if (cells.Count < CanonicalHeader.Length) return false;

        for (int i = 0; i < CanonicalHeader.Length; i++)
        {
            if (double.TryParse(cells[i], out _)) return false;
        }

        return true;
    }

    private static bool HeaderMatchesCanonical(IReadOnlyList<string> cells)
    {
        if (cells.Count < CanonicalHeader.Length) return false;

        for (int i = 0; i < CanonicalHeader.Length; i++)
        {
            if (!StringEqualsLoose(cells[i], CanonicalHeader[i])) return false;
        }

        return true;
    }

    private static bool StringEqualsLoose(string? a, string? b)
        => string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);
}
