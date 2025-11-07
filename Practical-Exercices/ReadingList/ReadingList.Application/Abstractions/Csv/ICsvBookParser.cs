using ReadingList.Domain.Results;
using ReadingList.Domain.Entities;

namespace ReadingList.Application.Abstractions.Csv;

public sealed record ParsedBook(int LineNumber, Result<Book> Result);

public interface ICsvBookParser
{
    IAsyncEnumerable<ParsedBook> ParseFileAsync(string path, CancellationToken cancellationToken = default);
}
