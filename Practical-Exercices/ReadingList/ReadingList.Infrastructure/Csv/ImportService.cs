using ReadingList.Domain.Entities;
using ReadingList.Domain.Interfaces;
using ReadingList.Infrastructure.Enums;
using ReadingList.Infrastructure.Interfaces;
using System.Collections.Concurrent;

namespace ReadingList.Infrastructure.Csv;

public sealed class ImportService
{
    private readonly ICsvBookParser _parser;
    private readonly IRepository<Book, int> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IConflictResolver _resolver;

    public ImportService(ICsvBookParser parser, IRepository<Book, int> repo, IUnitOfWork uow, ILogger log, IConflictResolver resolver)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _repository = repo ?? throw new ArgumentNullException(nameof(repo));
        _unitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
        _logger = log ?? throw new ArgumentNullException(nameof(log));
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    public async Task<ImportStats> ImportAsync(
        IEnumerable<string> paths, 
        CancellationToken cancellationToken,
        int? maxDegreeOfParallelism = null)
    {
        if (paths is null || !paths.Any())
            throw new ArgumentException("At least one path must be provided.", nameof(paths));

        var importStats = new ImportStats();
        var importConflicts = new ConcurrentBag<ImportConflict>();

        int maxSimultaneousFiles = maxDegreeOfParallelism ?? Math.Max(2, Environment.ProcessorCount / 2); 
        using var fileProcessingGate = new SemaphoreSlim(maxSimultaneousFiles); 

        var tasks = paths.Select(path => ProcessFileAsync(path, importStats, importConflicts, fileProcessingGate, cancellationToken));
        await Task.WhenAll(tasks).ConfigureAwait(false);

        await ResolveConflictsAsync(importConflicts, importStats, cancellationToken).ConfigureAwait(false);

        var save = await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (!save.IsSuccess)
            _logger.Error(save.Error ?? "Unknown error when saving changes.");

        return importStats;
    }

    private async Task ProcessFileAsync(
           string path,
           ImportStats stats,
           ConcurrentBag<ImportConflict> conflicts,
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
            ImportStats stats,
            ConcurrentBag<ImportConflict> conflicts)
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
            conflicts.Add(new ImportConflict(
                Id: incoming.Id,
                Existing: existing,
                Incoming: incoming,
                SourcePath: path,
                LineNumber: parsed.LineNumber));
        }
        else
        {
            conflicts.Add(new ImportConflict(
                Id: incoming.Id,
                Existing: incoming,
                Incoming: incoming,
                SourcePath: path,
                LineNumber: parsed.LineNumber));
        }
    }

    private async Task ResolveConflictsAsync(
            IEnumerable<ImportConflict> conflicts,
            ImportStats stats,
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
