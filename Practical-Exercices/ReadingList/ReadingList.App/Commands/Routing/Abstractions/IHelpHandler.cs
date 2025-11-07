using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IHelpHandler
{
    Task HandleAsync(HelpCommand command, CancellationToken ct);
}