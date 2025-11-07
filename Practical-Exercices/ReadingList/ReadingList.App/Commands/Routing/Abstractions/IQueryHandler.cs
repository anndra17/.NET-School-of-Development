using ReadingList.CLI.Commands.Models;

namespace ReadingList.CLI.Commands.Routing.Abstractions;

public interface IQueryHandler
{
    Task HandleAsync(ListAllCommand command, CancellationToken ct);
    Task HandleAsync(FilterFinishedCommand command, CancellationToken ct);
    Task HandleAsync(ByAuthorCommand command, CancellationToken ct);
    Task HandleAsync(TopRatedCommand command, CancellationToken ct);
    Task HandleAsync(StatsCommand command, CancellationToken ct);
}
