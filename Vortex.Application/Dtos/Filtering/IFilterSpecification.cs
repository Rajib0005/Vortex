namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Generic contract for composing filter predicates on any entity type.
/// Each domain implements this to translate a filter query into IQueryable predicates.
/// </summary>
public interface IFilterSpecification<TEntity, in TFilter>
    where TFilter : BaseFilterQuery
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query, TFilter filter);
}
