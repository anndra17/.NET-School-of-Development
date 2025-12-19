namespace AirportManagement.Application.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(object Id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(object Id);
}
