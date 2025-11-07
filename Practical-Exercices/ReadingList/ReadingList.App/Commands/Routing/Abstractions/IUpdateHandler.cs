using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IUpdateHandler
{
    Task HandleAsync(MarkFinishedCommand command, CancellationToken ct);
    Task HandleAsync(RateCommand command, CancellationToken ct);
}