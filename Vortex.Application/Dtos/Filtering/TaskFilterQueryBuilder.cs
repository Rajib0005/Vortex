using TaskStatus = Vortex.Domain.TaskStatus;
using Vortex.Domain;

namespace Vortex.Application.Dtos.Filtering;

/// <summary>
/// Fluent builder for constructing immutable TaskFilterQuery instances.
/// Used by controllers (to enforce route-level ProjectId) and for programmatic construction
/// (e.g. replaying saved views, tests).
/// </summary>
public sealed class TaskFilterQueryBuilder
{
    private Guid?                 _parentTaskId;
    private List<TaskStatus>      _statuses    = [];
    private List<TaskPriority>    _priorities  = [];
    private List<TaskType>        _taskTypes   = [];
    private List<Guid>            _assigneeIds = [];
    private List<Guid>            _reporterIds = [];
    private List<string>          _labels      = [];
    private string?               _searchTerm;
    private DateTime?             _dueDateFrom, _dueDateTo;
    private DateTime?             _startDateFrom, _startDateTo;
    private DateTime?             _createdFrom, _createdTo;
    private int _page = 1, _pageSize = 50;
    private string _sortBy = "CreatedAt";
    private bool _sortDesc = true;

    public TaskFilterQueryBuilder UnderParent(Guid? parentId)                   { _parentTaskId = parentId; return this; }
    public TaskFilterQueryBuilder WithStatuses(params TaskStatus[] s)           { _statuses.AddRange(s); return this; }
    public TaskFilterQueryBuilder WithPriorities(params TaskPriority[] p)       { _priorities.AddRange(p); return this; }
    public TaskFilterQueryBuilder WithTaskTypes(params TaskType[] t)            { _taskTypes.AddRange(t); return this; }
    public TaskFilterQueryBuilder WithAssignees(params Guid[] ids)              { _assigneeIds.AddRange(ids); return this; }
    public TaskFilterQueryBuilder WithReporters(params Guid[] ids)              { _reporterIds.AddRange(ids); return this; }
    public TaskFilterQueryBuilder WithLabels(params string[] l)                 { _labels.AddRange(l); return this; }
    public TaskFilterQueryBuilder Search(string? term)                          { _searchTerm = term; return this; }
    public TaskFilterQueryBuilder DueBetween(DateTime? from, DateTime? to)      { _dueDateFrom = from; _dueDateTo = to; return this; }
    public TaskFilterQueryBuilder StartedBetween(DateTime? from, DateTime? to)  { _startDateFrom = from; _startDateTo = to; return this; }
    public TaskFilterQueryBuilder CreatedBetween(DateTime? from, DateTime? to)  { _createdFrom = from; _createdTo = to; return this; }
    public TaskFilterQueryBuilder OnPage(int page, int pageSize = 50)           { _page = page; _pageSize = pageSize; return this; }
    public TaskFilterQueryBuilder SortBy(string field, bool desc = true)        { _sortBy = field; _sortDesc = desc; return this; }

    public TaskFilterQuery Build() => new()
    {
        ParentTaskId  = _parentTaskId,
        Statuses      = _statuses.AsReadOnly(),
        Priorities    = _priorities.AsReadOnly(),
        TaskTypes     = _taskTypes.AsReadOnly(),
        AssigneeIds   = _assigneeIds.AsReadOnly(),
        ReporterIds   = _reporterIds.AsReadOnly(),
        Labels        = _labels.AsReadOnly(),
        SearchTerm    = _searchTerm,
        DueDateFrom   = _dueDateFrom,
        DueDateTo     = _dueDateTo,
        StartDateFrom = _startDateFrom,
        StartDateTo   = _startDateTo,
        CreatedFrom   = _createdFrom,
        CreatedTo     = _createdTo,
        Page          = _page,
        PageSize      = _pageSize,
        SortBy        = _sortBy,
        SortDesc      = _sortDesc,
    };
}
