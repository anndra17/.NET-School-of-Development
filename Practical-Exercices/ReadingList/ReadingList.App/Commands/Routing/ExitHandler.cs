using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;

namespace ReadingList.CLI.Commands.Routing;

public sealed class ExitHandler : IExitHandler
{
    private readonly IConsoleNotifier _consoleNotifier;

    public ExitHandler(IConsoleNotifier consoleNotifier)
    {
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public Task HandleAsync(ExitCommand command, CancellationToken cancellationToken)
    {
        _consoleNotifier.Info("Goodbye!");
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
