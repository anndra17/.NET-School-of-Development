using ReadingList.App.Commands.Models;

namespace ReadingList.App.Commands.Routing.Abstractions;

public interface IHelpHandler
{
    Task HandleAsync(HelpCommand command, CancellationToken ct);
}