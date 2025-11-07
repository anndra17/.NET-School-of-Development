namespace ReadingList.Application.Abstractions.Csv;

public sealed record CsvRow(int LineNumber, IReadOnlyList<string> Cells);

public interface ICsvReader
{
    IAsyncEnumerable<CsvRow> ReadAsync(string path, CancellationToken cancellationToken = default);
}
