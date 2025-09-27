namespace Vortex.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void UpdateAsync(TEntity entity);
    void DeleteAsync(TEntity entity);
    void DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task<int> SaveChangesAsync();
}