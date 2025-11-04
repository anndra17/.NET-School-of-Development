using ReadingList.Domain.Entities;

namespace ReadingList.Infrastructure.Interfaces
{
    public interface IExportStrategy
    {
        Task<Domain.Results.Result> ExportAsync(
            IEnumerable<Book> books,
            string path,
            bool overwriteAllowed,
            CancellationToken cancellationToken);
    }
}
