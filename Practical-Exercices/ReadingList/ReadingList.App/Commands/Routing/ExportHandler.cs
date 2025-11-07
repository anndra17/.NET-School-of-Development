using ReadingList.Application.Abstractions.Persistence;
using ReadingList.Application.Enums;
using ReadingList.Application.Services;
using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;
using ReadingList.Domain.Entities;
using System.Globalization;
using InfraFormat = ReadingList.Application.Enums.ExportFormat;

namespace ReadingList.CLI.Commands.Routing;

public sealed class ExportHandler : IExportHandler
{
    private readonly IRepository<Book, int> _repository;
    private readonly ExportService _exportService;
    private readonly IConsoleNotifier _consoleNotifier;

    public ExportHandler(
        IRepository<Book, int> repository,
        ExportService exportService,
        IConsoleNotifier consoleNotifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public async Task HandleAsync(ExportCommand command, CancellationToken cancellationToken)
    {
        var books = _repository.GetAll().ToList();
        if (books.Count == 0)
        {
            _consoleNotifier.Warn("No books to export.");
            return;
        }

        _consoleNotifier.Info($"Exporting {books.Count.ToString(CultureInfo.InvariantCulture)} books to \"{command.Path}\" as {command.Format}...");

        var infraFormat = MapFormat(command.Format);

        var result = await _exportService.ExportAsync(books, command.Path, infraFormat, cancellationToken);

        if (result.IsSuccess)
        {
            _consoleNotifier.Success($"Export saved to \"{command.Path}\".");
        }
        else
        {
            _consoleNotifier.Error($"Export failed: {result.Error}");
        }
    }

    private static InfraFormat MapFormat(ExportFormat format) => format switch
    {
        ExportFormat.Json => InfraFormat.Json,
        ExportFormat.Csv => InfraFormat.Csv,
        ExportFormat.Auto => InfraFormat.Auto,
        _ => InfraFormat.Auto
    };
}
