using System.Collections.Concurrent;

namespace ReadingList.Infrastructure.Csv;

public sealed class ImportStats
{
    private int _imported;
    private int _duplicatesEncountered;
    private int _malformed;
    internal readonly ConcurrentDictionary<int, byte> _skippedIds = new();

    public int Imported => _imported;
    public int DuplicatesEncountered => _duplicatesEncountered;
    public int Malformed => _malformed;
    public IReadOnlyCollection<int> SkippedIds => _skippedIds.Keys.ToArray();

    public void IncrementImported() => Interlocked.Increment(ref _imported);

    public void IncrementDuplicatesEncountered() => Interlocked.Increment(ref _duplicatesEncountered);

    public void IncrementMalformed() => Interlocked.Increment(ref _malformed);

    public void AddSkippedId(int id) => _skippedIds.TryAdd(id, 1);
}
