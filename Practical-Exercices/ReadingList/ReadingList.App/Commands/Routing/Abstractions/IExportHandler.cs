using ReadingList.App.Commands.Models;

namespace ReadingList.App.Commands.Routing.Abstractions;

public interface IExportHandler
{
    Task HandleAsync(ExportCommand command, CancellationToken ct);
}
