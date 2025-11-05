using ReadingList.App.Commands.Models;

namespace ReadingList.App.Commands.Routing.Abstractions;

public interface IExitHandler
{
    Task HandleAsync(ExitCommand command, CancellationToken ct);
}
