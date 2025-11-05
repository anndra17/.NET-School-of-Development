using ReadingList.App.Commands.Models;

namespace ReadingList.App.Commands.Routing.Abstractions;

public interface IImportHandler
{
    Task HandleAsync(ImportCommand command, CancellationToken ct);
}
