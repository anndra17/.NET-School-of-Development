using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IExportHandler
{
    Task HandleAsync(ExportCommand command, CancellationToken ct);
}
