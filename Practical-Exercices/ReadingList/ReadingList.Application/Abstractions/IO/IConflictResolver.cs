using ReadingList.Application.Dtos;
using ReadingList.Application.Enums;

namespace ReadingList.Application.Abstractions.IO;

public interface IConflictResolver
{
    Task<DuplicateDecision> DecideAsync(ImportConflictDto conflict, CancellationToken cancellationToken);
}
