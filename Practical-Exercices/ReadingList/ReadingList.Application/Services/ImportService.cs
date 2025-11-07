using ReadingList.Application.Abstractions.Csv;
using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.Application.Abstractions.Persistence;
using ReadingList.Application.Dtos;
using ReadingList.Application.Enums;
using ReadingList.Domain.Entities;
using System.Collections.Concurrent;

namespace ReadingList.Application.Services;

public sealed class ImportService
{
    private readonly ICsvBookParser _parser;
    private readonly IRepository<Book, int> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogger _systemLogger;
    private readonly IConflictResolver _resolver;

    public ImportService(ICsvBookParser parser, IRepository<Book, int> repository, IUnitOfWork unitOfWork, ISystemLogger systemLogger, IConflictResolver resolver)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    public async Task<ImportStatsDto> ImportAsync(
        IEnumerable<string> paths, 
        CancellationToken cancellationToken,
        int? maxDegreeOfParallelism = null)
    {
        if (paths is null || !paths.Any())
            throw new ArgumentException("At least one path must be provided.", nameof(paths));

        var importStats = new ImportStatsDto();
        var ImportConflictDtos = new ConcurrentBag<ImportConflictDto>();

        int maxSimultaneousFiles = maxDegreeOfParallelism ?? Math.Max(2, Environment.ProcessorCount / 2); 
        using var fileProcessingGate = new SemaphoreSlim(maxSimultaneousFiles); 

        var tasks = paths.Select(
            path => ProcessFileAsync(
                path, 
                importStats, 
                ImportConflictDtos, 
                fileProcessingGate, 
                cancellationToken)
            );
        await Task.WhenAll(tasks).ConfigureAwait(false);

        await ResolveConflictsAsync(ImportConflictDtos, importStats, cancellationToken).ConfigureAwait(false);

        var save = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (!save.IsSuccess)
            _systemLogger.Error(save.Error ?? "Unknown error when saving changes.");

        return importStats;
    }

    private async Task ProcessFileAsync(
           string path,
           ImportStatsDto stats,
           ConcurrentBag<ImportConflictDto> conflicts,
           SemaphoreSlim gate,
           CancellationToken ct)
    {
        await gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await foreach (var parsed in _parser.ParseFileAsync(path, ct).ConfigureAwait(false))
            {
                ct.ThrowIfCancellationRequested();
                HandleParsedRow(path, parsed, stats, conflicts);
            }
        }
        finally
        {
            gate.Release();
        }
    }

    private void HandleParsedRow(
            string path,
            ParsedBook parsed,
            ImportStatsDto stats,
            ConcurrentBag<ImportConflictDto> conflicts)
    {
        if (!parsed.Result.IsSuccess)
        {
            stats.IncrementMalformed();
            return;
        }

        var incoming = parsed.Result.Value!;
        if (_repository.Add(incoming))
        {
            stats.IncrementImported();
            return;
        }

        stats.IncrementDuplicatesEncountered();

        if (_repository.TryGet(incoming.Id, out var existing) && existing is not null)
        {
            conflicts.Add(new ImportConflictDto(
                Id: incoming.Id,
                Existing: existing,
                Incoming: incoming,
                SourcePath: path,
                LineNumber: parsed.LineNumber));
        }
        else
        {
            conflicts.Add(new ImportConflictDto(
                Id: incoming.Id,
                Existing: incoming,
                Incoming: incoming,
                SourcePath: path,
                LineNumber: parsed.LineNumber));
        }
    }

    private async Task ResolveConflictsAsync(
            IEnumerable<ImportConflictDto> conflicts,
            ImportStatsDto stats,
            CancellationToken ct)
    {
        foreach (var group in conflicts.GroupBy(c => c.Id))
        {
            var candidate = group.Last();
            var decision = await _resolver.DecideAsync(candidate, ct).ConfigureAwait(false);

            if (decision == DuplicateDecision.ReplaceWithIncoming)
            {
                _repository.Upsert(candidate.Incoming);
            }
            else
            {
                stats.AddSkippedId(candidate.Id);
            }
        }
    }
}
