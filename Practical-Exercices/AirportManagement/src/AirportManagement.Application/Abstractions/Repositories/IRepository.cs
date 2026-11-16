namespace AirportManagement.Application.Abstractions.Repositories;

public interface IRepository<TDomain, TKey> where TDomain : class
{
    Task<TDomain?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<IEnumerable<TDomain>> GetAllAsync(CancellationToken ct = default);
    Task InsertAsync(TDomain entity, CancellationToken ct = default);
    Task UpdateAsync(TDomain entity, CancellationToken ct = default);
    Task DeleteAsync(TKey id, CancellationToken ct = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);
}

