using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IImportHandler
{
    Task HandleAsync(ImportCommand command, CancellationToken ct);
}
