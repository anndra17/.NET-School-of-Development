using ReadingList.Application.Dtos;
using ReadingList.Application.Services;
using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;
using System.Globalization;

namespace ReadingList.CLI.Commands.Routing;

public sealed class ImportHandler : IImportHandler
{
    private readonly ImportService _importService;
    private readonly IConsoleNotifier _consoleNotifier;
    private readonly IConsole _console;

    public ImportHandler(ImportService importService, IConsoleNotifier consoleNotifier, IConsole console)
    {
        _importService = importService ?? throw new ArgumentNullException(nameof(importService));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public async Task HandleAsync(ImportCommand command, CancellationToken cancellationToken)
    {
        if (command.Paths is null || command.Paths.Count == 0)
        {
            _consoleNotifier.Error("Usage: import <file1.csv> [file2.csv ...]");
            return;
        }

        var paths = command.Paths
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _consoleNotifier.Info($"Importing {paths.Count} file(s)...");

        var stats = await _importService.ImportAsync(paths, cancellationToken);

        _consoleNotifier.Success("Import completed.");
        _console.WriteLine();

        PrintStats(stats);
    }

    private void PrintStats(ImportStatsDto stats)
    {
        _consoleNotifier.Info($"Imported:   {stats.Imported.ToString(CultureInfo.InvariantCulture)}");
        _consoleNotifier.Info($"Duplicates: {stats.DuplicatesEncountered.ToString(CultureInfo.InvariantCulture)}");
        _consoleNotifier.Info($"Malformed:  {stats.Malformed.ToString(CultureInfo.InvariantCulture)}");

        if (stats.SkippedIds.Count > 0)
        {
            _consoleNotifier.Warn($"Skipped Ids: {string.Join(", ", stats.SkippedIds.OrderBy(x => x))}");
        }

        _console.WriteLine();
    }
}
