using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Generic implementation of the filter → count → paginate → project pipeline.
/// Inject this in any domain service and call GetFilteredAsync with the appropriate spec.
/// </summary>
public sealed class FilteringService(IMapper mapper) : IFilteringService
{
    private readonly IMapper _mapper = mapper;

    public async Task<PagedResult<TDto>> GetFilteredAsync<TEntity, TFilter, TDto>(
        IQueryable<TEntity> source,
        TFilter filter,
        IFilterSpecification<TEntity, TFilter> specification,
        CancellationToken cancellationToken = default)
        where TFilter : BaseFilterQuery
        where TEntity : class
    {
        var filtered = specification.Apply(source, filter);

        var totalCount = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDto>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = filter.Page,
            PageSize   = filter.PageSize
        };
    }
}
