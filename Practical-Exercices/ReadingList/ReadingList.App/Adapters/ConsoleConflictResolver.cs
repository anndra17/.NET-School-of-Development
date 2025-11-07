using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Dtos;
using ReadingList.Application.Enums;
using ReadingList.CLI.UI.Abstractions;

namespace ReadingList.CLI.Adapters;


public sealed class ConsoleConflictResolver : IConflictResolver
{
    private readonly IConsole _console;
    private readonly IConsoleNotifier _consoleNotifier;

    public ConsoleConflictResolver(IConsole console, IConsoleNotifier consoleNotifier)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public async Task<DuplicateDecision> DecideAsync(ImportConflictDto conflict, CancellationToken cancellationToken)
    {
        _consoleNotifier.Warn($"Duplicate Id {conflict.Id} at {conflict.SourcePath}:{conflict.LineNumber}");

        _console.WriteLine($"Existing : \"{conflict.Existing.Title}\" by {conflict.Existing.Author} (Rating {conflict.Existing.Rating})");
        _console.WriteLine($"Incoming : \"{conflict.Incoming.Title}\" by {conflict.Incoming.Author} (Rating {conflict.Incoming.Rating})");
        _console.WriteLine();

        while (true)
        {
            var yes = await _console.PromptYesNoAsync(
                "Replace existing with incoming?",
                defaultYes: false,
                cancellationToken: cancellationToken);

            return yes ? DuplicateDecision.ReplaceWithIncoming
                       : DuplicateDecision.KeepExisting;
        }
    }
}
