using ReadingList.App.Commands.Models;

namespace ReadingList.App.Commands.Routing.Abstractions;

public interface IUpdateHandler
{
    Task HandleAsync(MarkFinishedCommand command, CancellationToken ct);
    Task HandleAsync(RateCommand command, CancellationToken ct);
}