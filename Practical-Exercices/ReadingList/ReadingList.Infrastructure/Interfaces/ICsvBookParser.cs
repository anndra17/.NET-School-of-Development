using ReadingList.Domain.Results;
using ReadingList.Domain.Entities;

namespace ReadingList.Infrastructure.Interfaces;
public sealed record ParsedBook(int LineNumber, Result<Book> Result);

public interface ICsvBookParser
{
    IAsyncEnumerable<ParsedBook> ParseFileAsync(string path, CancellationToken ct = default);
}
