using TaskStatus = Vortex.Domain.TaskStatus;
using Vortex.Domain;

namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Task-specific filter query extending BaseFilterQuery.
/// Each dimension uses IReadOnlyList — empty list means "no filter" (return all).
/// Immutable once built via TaskFilterQueryBuilder.
/// </summary>
public sealed class TaskFilterQuery : BaseFilterQuery
{
    // Scope
    public Guid  ProjectId    { get; init; }
    public Guid? ParentTaskId { get; init; }

    // Multi-select enum filters
    public IReadOnlyList<TaskStatus>   Statuses   { get; init; } = [];
    public IReadOnlyList<TaskPriority> Priorities { get; init; } = [];
    public IReadOnlyList<TaskType>     TaskTypes  { get; init; } = [];

    // Multi-select user filters
    public IReadOnlyList<Guid> AssigneeIds { get; init; } = [];
    public IReadOnlyList<Guid> ReporterIds { get; init; } = [];

    // Labels
    public IReadOnlyList<string> Labels { get; init; } = [];

    // Date ranges
    public DateTime? DueDateFrom    { get; init; }
    public DateTime? DueDateTo      { get; init; }
    public DateTime? StartDateFrom  { get; init; }
    public DateTime? StartDateTo    { get; init; }
    public DateTime? CreatedFrom    { get; init; }
    public DateTime? CreatedTo      { get; init; }
}
