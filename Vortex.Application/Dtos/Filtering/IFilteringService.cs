namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Generic filtering pipeline interface.
/// Any service can inject this to get filtered, paginated results for any entity.
/// </summary>
public interface IFilteringService
{
    Task<PagedResult<TDto>> GetFilteredAsync<TEntity, TFilter, TDto>(
        IQueryable<TEntity> source,
        TFilter filter,
        IFilterSpecification<TEntity, TFilter> specification,
        CancellationToken cancellationToken = default)
        where TFilter : BaseFilterQuery
        where TEntity : class;
}
