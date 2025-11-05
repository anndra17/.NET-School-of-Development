using ReadingList.Domain.Interfaces;
using System.Collections.Concurrent;

namespace ReadingList.Infrastructure.Repositories;

public class InMemoryRepository<T, TKey> : IRepository<T, TKey>
    where TKey : notnull
    where T : class
{
    private readonly Func<T, TKey> _keySelector;
    private readonly ConcurrentDictionary<TKey, T> _store;

    public InMemoryRepository(Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        _store = new ConcurrentDictionary<TKey, T>(comparer ?? EqualityComparer<TKey>.Default);
    }

    public bool Add(T entity)
    {
        if (entity is null) 
            throw new ArgumentNullException(nameof(entity));

        var key = _keySelector(entity);
        var added = _store.TryAdd(key, entity);

        return added;         
    }

    public bool Upsert(T entity)
    {
        if (entity is null) 
            throw new ArgumentNullException(nameof(entity));

        var key = _keySelector(entity);
        var inserted = _store.TryAdd(key, entity);
        if (!inserted)
            _store[key] = entity;

        return inserted;
    }

    public bool Remove(TKey id) 
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        var removed = _store.TryRemove(id, out _);

        return removed;
    }

    public bool TryGet(TKey id, out T?  entity)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));

        var ok = _store.TryGetValue(id, out var value);
        entity = value;

        return ok;
    }

    public IReadOnlyCollection<T> GetAll() => _store.Values.ToArray();

    public int Count => _store.Count;
}
