using ReadingList.CLI.UI.Abstractions;
using ReadingList.CLI.Commands.Parsing;
using ReadingList.CLI.Commands.Routing;
using ReadingList.CLI.Commands.Abstractions;

namespace ReadingList.CLI.App;

public sealed class CliApplication
{
    private readonly IConsole _console;
    private readonly IConsoleNotifier _notifier;
    private readonly CommandParser _parser;
    private readonly CommandRouter _router;

    private CancellationTokenSource? _currentCommandCts;

    public CliApplication(IConsole console, IConsoleNotifier notifier, CommandParser parser, CommandRouter router)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _router = router ?? throw new ArgumentNullException(nameof(router));
    }

    public void CancelCurrentOperation()
    {
        try { _currentCommandCts?.Cancel(); }
        catch { }
    }

    public async Task RunAsync(CancellationToken shutdownToken)
    {
        _notifier.Info("Type 'help' for commands. Press Ctrl+C to cancel the current operation.");

        while (!shutdownToken.IsCancellationRequested)
        {
            _console.Write("> ");
            var line = await _console.ReadLineAsync(shutdownToken);
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parsed = _parser.Parse(line);
            if (!parsed.IsSuccess)
            {
                _notifier.Error(parsed.Error!);
                continue;
            }

            using var opCts = CancellationTokenSource.CreateLinkedTokenSource(shutdownToken);
            _currentCommandCts = opCts;

            try
            {
                if (parsed.Command is ICommand cmd)
                {
                    await _router.RouteAsync(cmd, opCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                _notifier.Warn("Operation canceled.");
            }
            finally
            {
                _currentCommandCts = null; 
            }
        }
    }
}
