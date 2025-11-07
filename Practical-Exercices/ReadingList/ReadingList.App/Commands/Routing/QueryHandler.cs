using ReadingList.Application.Services;
using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;
using ReadingList.CLI.UI.Rendering;

namespace ReadingList.CLI.Commands.Routing;

public sealed class QueryHandler : IQueryHandler
{
    private readonly BookQueryService _queries;
    private readonly BookTableRenderer _booksRenderer;
    private readonly BookStatsRenderer _statsRenderer;
    private readonly IConsoleNotifier _consoleNotifier;

    public QueryHandler(
        BookQueryService queries, 
        BookTableRenderer renderer,
        BookStatsRenderer statsRenderer,
        IConsoleNotifier consoleNotifier)
    {
        _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        _booksRenderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _statsRenderer = statsRenderer ?? throw new ArgumentNullException(nameof(statsRenderer));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public Task HandleAsync(ListAllCommand command, CancellationToken ct)
    {
        var list = _queries.ListAll();
        if (list.Count == 0) { _consoleNotifier.Info("No books."); _consoleNotifier.Info(""); return Task.CompletedTask; }

        _booksRenderer.Render(list);
        return Task.CompletedTask;
    }

    public Task HandleAsync(FilterFinishedCommand command, CancellationToken ct)
    {
        var list = _queries.FilterFinished(command.Finished);
        if (list.Count == 0)
        {
            _consoleNotifier.Info(command.Finished ? "No finished books." : "No unfinished books.");
            _consoleNotifier.Info("");
            return Task.CompletedTask;
        }

        _booksRenderer.Render(list);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ByAuthorCommand command, CancellationToken ct)
    {
        var list = _queries.ByAuthorContains(command.Text, ignoreCase: true);
        if (list.Count == 0)
        {
            _consoleNotifier.Info($"No books found for author containing \"{command.Text}\".");
            _consoleNotifier.Info("");
            return Task.CompletedTask;
        }

        _booksRenderer.Render(list);
        return Task.CompletedTask;
    }

    public Task HandleAsync(TopRatedCommand command, CancellationToken ct)
    {
        var list = _queries.TopRated(command.Count);
        if (list.Count == 0) { _consoleNotifier.Info("No books."); _consoleNotifier.Info(""); return Task.CompletedTask; }

        _booksRenderer.Render(list);
        return Task.CompletedTask;
    }

    public Task HandleAsync(StatsCommand command, CancellationToken ct)
    {
        var stats = _queries.Stats();   
        _statsRenderer.Render(stats);  
        return Task.CompletedTask;
    }

}
