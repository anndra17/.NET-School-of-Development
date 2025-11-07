using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IExitHandler
{
    Task HandleAsync(ExitCommand command, CancellationToken ct);
}
