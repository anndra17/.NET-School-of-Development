namespace AirportManagement.Application.Abstractions.Repositories;

public interface IRepository<TDomain> where TDomain : class
{
    Task<TDomain?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TDomain>> GetAllAsync(CancellationToken ct = default);
    Task InsertAsync(TDomain entity, CancellationToken ct = default);
    Task UpdateAsync(TDomain entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
