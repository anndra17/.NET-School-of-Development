using ReadingList.Domain.Results;

namespace ReadingList.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
