namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Generic paged response wrapper returned by the filtering pipeline.
/// </summary>
public class PagedResult<T>
{
    public List<T> Items      { get; set; } = [];
    public int     TotalCount { get; set; }
    public int     Page       { get; set; }
    public int     PageSize   { get; set; }
    public int     TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
