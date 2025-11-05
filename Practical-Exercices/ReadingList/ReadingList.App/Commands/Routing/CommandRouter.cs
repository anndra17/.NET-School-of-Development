using ReadingList.App.Commands.Abstractions;
using ReadingList.App.Commands.Models;
using ReadingList.App.Commands.Routing.Abstractions;
using ReadingList.App.UI.Abstractions;

namespace ReadingList.App.Commands.Routing;

public sealed class CommandRouter
{
    private readonly IImportHandler _import;
    private readonly IQueryHandler _query;
    private readonly IUpdateHandler _update;
    private readonly IExportHandler _export;
    private readonly IHelpHandler _help;
    private readonly IExitHandler _exit;
    private readonly IAppLogger _logger;

    public CommandRouter(
            IImportHandler import,
            IQueryHandler query,
            IUpdateHandler update,
            IExportHandler export,
            IHelpHandler help,
            IExitHandler exit,
            IAppLogger logger)
    {
        _import = import ?? throw new ArgumentNullException(nameof(import));
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _update = update ?? throw new ArgumentNullException(nameof(update));
        _export = export ?? throw new ArgumentNullException(nameof(export));
        _help = help ?? throw new ArgumentNullException(nameof(help));
        _exit = exit ?? throw new ArgumentNullException(nameof(exit));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RouteAsync(ICommand command, CancellationToken ct)
    {
        switch (command)
        {
            case ImportCommand c: await _import.HandleAsync(c, ct); return;

            case ListAllCommand c: await _query.HandleAsync(c, ct); return;
            case FilterFinishedCommand c: await _query.HandleAsync(c, ct); return;
            case ByAuthorCommand c: await _query.HandleAsync(c, ct); return;
            case TopRatedCommand c: await _query.HandleAsync(c, ct); return;
            case StatsCommand c: await _query.HandleAsync(c, ct); return;

            case MarkFinishedCommand c: await _update.HandleAsync(c, ct); return;
            case RateCommand c: await _update.HandleAsync(c, ct); return;

            case ExportCommand c: await _export.HandleAsync(c, ct); return;

            case HelpCommand c: await _help.HandleAsync(c, ct); return;
            case ExitCommand c: await _exit.HandleAsync(c, ct); return;

            default:
                _logger.Error("Unknown command. Try 'help'.");
                return;
        }
    }
}
