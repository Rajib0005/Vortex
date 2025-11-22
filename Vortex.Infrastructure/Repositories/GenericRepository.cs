using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vortex.Domain.Repositories;
using Vortex.Infrastructure.Data;

namespace Vortex.Infrastructure.Repositories;

public class GenericRepository<TEntity>(VortexDbContext _dbContext) : IGenericRepository<TEntity>
    where TEntity : class
{ 
    private DbSet<TEntity> _dbSet => _dbContext.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public IQueryable<TEntity> GetAllAsync()
    {
       return _dbContext.Set<TEntity>().AsNoTracking();
    }

    public IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression)
    {
        return  _dbContext.Set<TEntity>().Where(expression).AsNoTracking();
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