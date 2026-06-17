using Vortex.Domain.Entities;

namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Composes all task filter predicates into a single IQueryable chain.
/// Adding a new filter dimension = one new property on TaskFilterQuery + one if-block here.
/// </summary>
public sealed class TaskFilterSpecification(Guid projectId)
    : IFilterSpecification<TaskEntity, TaskFilterQuery>
{
    private readonly Guid _projectId = projectId;

    public IQueryable<TaskEntity> Apply(
        IQueryable<TaskEntity> query,
        TaskFilterQuery filter)
    {
        // Scope — always filter by project
        query = query.Where(t => t.ProjectId == _projectId);

        if (filter.ParentTaskId.HasValue)
            query = query.Where(t => t.ParentTaskId == filter.ParentTaskId);

        // Multi-select enum filters
        if (filter.Statuses.Count > 0)
            query = query.Where(t => filter.Statuses.Contains(t.Status));

        if (filter.Priorities.Count > 0)
            query = query.Where(t => filter.Priorities.Contains(t.Priority));

        if (filter.TaskTypes.Count > 0)
            query = query.Where(t => filter.TaskTypes.Contains(t.TaskType));

        // Multi-select user filters
        if (filter.AssigneeIds.Count > 0)
            query = query.Where(t => t.AssigneeId != null && filter.AssigneeIds.Contains(t.AssigneeId.Value));

        if (filter.ReporterIds.Count > 0)
            query = query.Where(t => t.ReporterId != null && filter.ReporterIds.Contains(t.ReporterId.Value));

        // Labels
        if (filter.Labels.Count > 0)
            query = query.Where(t => t.Labels.Any(l => filter.Labels.Contains(l)));

        // Free-text search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(t =>
                (t.TaskName != null && t.TaskName.ToLower().Contains(term)) ||
                t.TaskKey.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        // Date ranges
        if (filter.DueDateFrom.HasValue)   query = query.Where(t => t.DueDate >= filter.DueDateFrom);
        if (filter.DueDateTo.HasValue)     query = query.Where(t => t.DueDate <= filter.DueDateTo);
        if (filter.StartDateFrom.HasValue) query = query.Where(t => t.StartDate >= filter.StartDateFrom);
        if (filter.StartDateTo.HasValue)   query = query.Where(t => t.StartDate <= filter.StartDateTo);
        if (filter.CreatedFrom.HasValue)   query = query.Where(t => t.CreatedAt >= filter.CreatedFrom);
        if (filter.CreatedTo.HasValue)     query = query.Where(t => t.CreatedAt <= filter.CreatedTo);

        // Sorting
        query = filter.SortBy switch
        {
            "DueDate"   => filter.SortDesc ? query.OrderByDescending(t => t.DueDate)    : query.OrderBy(t => t.DueDate),
            "Priority"  => filter.SortDesc ? query.OrderByDescending(t => t.Priority)   : query.OrderBy(t => t.Priority),
            "Status"    => filter.SortDesc ? query.OrderByDescending(t => t.Status)     : query.OrderBy(t => t.Status),
            "UpdatedAt" => filter.SortDesc ? query.OrderByDescending(t => t.UpdatedAt)  : query.OrderBy(t => t.UpdatedAt),
            "TaskName"  => filter.SortDesc ? query.OrderByDescending(t => t.TaskName)   : query.OrderBy(t => t.TaskName),
            _           => filter.SortDesc ? query.OrderByDescending(t => t.CreatedAt)  : query.OrderBy(t => t.CreatedAt),
        };

        return query;
    }
}
