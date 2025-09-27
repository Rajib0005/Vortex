using Microsoft.EntityFrameworkCore;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Data;

namespace Vortex.Infrastructure.Repositories;

public class GenericRepository<TEntity>(VortexDbContext dbContext, DbSet<TEntity> dbSet) : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly VortexDbContext  _dbContext = dbContext;
    private readonly DbSet<TEntity> _dbSet = dbSet;

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
       return  await _dbSet.ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void  UpdateAsync(TEntity entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }
    
    public void DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await  _dbContext.SaveChangesAsync();
    }
}