using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.Application.Enums;
using ReadingList.Domain.Entities;

namespace ReadingList.Application.Services;

public sealed class ExportService
{
    private readonly IExportStrategy _csvStrategy;
    private readonly IExportStrategy _jsonStrategy;
    private readonly IFileSystem _fileSystem;
    private readonly ISystemLogger _systemLogger;
    private readonly IOverwritePolicy _overwritePolicy;

    public ExportService(
        IExportStrategy csvStrategy,
        IExportStrategy jsonStrategy,
        IFileSystem fileSystem,
        ISystemLogger systemLogger,
        IOverwritePolicy overwritePolicy)
    {
        _csvStrategy = csvStrategy ?? throw new ArgumentNullException(nameof(csvStrategy));
        _jsonStrategy = jsonStrategy ?? throw new ArgumentNullException(nameof(jsonStrategy));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
        _overwritePolicy = overwritePolicy ?? throw new ArgumentNullException(nameof(overwritePolicy));
    }

    public async Task<Domain.Results.Result> ExportAsync(
            IEnumerable<Book> books,
            string path,
            ExportFormat format,
            CancellationToken cancellationToken)
    {
        if (books is null) throw new ArgumentNullException(nameof(books));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path is required.", nameof(path));

        var strategy = SelectStrategy(path, format);
        var overwriteAllowed = await EnsureOverwritePolicyAsync(path, cancellationToken);
        if (!overwriteAllowed)
            return Domain.Results.Result.Fail("Export cancelled by user.");

        return await strategy.ExportAsync(books, path, overwriteAllowed, cancellationToken);
    }

    private IExportStrategy SelectStrategy(string path, ExportFormat format)
    {
        if (format == ExportFormat.Auto)
        {
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext switch
            {
                ".csv" => _csvStrategy,
                ".json" => _jsonStrategy,
                _ => throw new NotSupportedException("Unknown file extension. Use .csv or .json, or specify format.")
            };
        }

        return format switch
        {
            ExportFormat.Csv => _csvStrategy,
            ExportFormat.Json => _jsonStrategy,
            _ => throw new NotSupportedException($"Unsupported export format: {format}")
        };
    }

    private async Task<bool> EnsureOverwritePolicyAsync(string path, CancellationToken cancellationToken)
    {
        if (_fileSystem.FileExists(path))
        {
            _systemLogger.Warn($"File '{path}' already exists.");
            var allow = await _overwritePolicy.ConfirmOverwriteAsync(path, cancellationToken);
            return allow;
        }

        return true;
    }
}
