using AirportManagement.Application.Abstractions;
using AirportManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AirportManagement.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AirportManagementDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AirportManagementDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(object Id)
    {
        return await _dbSet.FindAsync(Id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(object Id)
    {
        var entity = await _dbSet.FindAsync(Id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}
