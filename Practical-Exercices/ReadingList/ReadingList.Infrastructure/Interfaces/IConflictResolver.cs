using ReadingList.Infrastructure.Csv;
using ReadingList.Infrastructure.Enums;

namespace ReadingList.Infrastructure.Interfaces;

public interface IConflictResolver
{
    Task<DuplicateDecision> DecideAsync(ImportConflict conflict, CancellationToken cancellationToken);
}
