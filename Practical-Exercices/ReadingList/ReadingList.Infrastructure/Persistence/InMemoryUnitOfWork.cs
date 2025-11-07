using ReadingList.Application.Abstractions.Persistence;
using ReadingList.Domain.Results;

namespace ReadingList.Infrastructure.Persistence;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok());
    }
}