using ReadingList.Domain.Entities;

namespace ReadingList.Application.Abstractions.IO;

public interface IExportStrategy
{
    Task<Domain.Results.Result> ExportAsync(
        IEnumerable<Book> books,
        string path,
        bool overwriteAllowed,
        CancellationToken cancellationToken);
}
