
namespace ReadingList.Infrastructure.Interfaces;

public sealed record CsvRow(int LineNumber, IReadOnlyList<string> Cells);

public interface ICsvReader
{
    IAsyncEnumerable<CsvRow> ReadAsync(string path, CancellationToken ct = default);
}
