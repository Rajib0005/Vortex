using System.Linq.Expressions;

namespace Vortex.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    IQueryable<TEntity> GetAllAsync();
     IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void UpdateAsync(TEntity entity);
    void DeleteAsync(TEntity entity);
    void DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task<int> SaveChangesAsync();
}

