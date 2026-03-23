namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Abstract base for all domain-specific filter queries.
/// Provides pagination, sorting, and free-text search out of the box.
/// </summary>
public abstract class BaseFilterQuery
{
    private readonly int _pageSize = 50;

    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            10 => 10,
            25 => 25,
            50 => 50,
            100 => 100,
            _ => 50
        };
    }

    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDesc { get; init; } = true;
}