namespace Vortex.Application.Dtos;

public class ProjectCardsDto
{
    public virtual string? Title {get; set;}
    public virtual string? Description {get; set;}
    public bool IsAcvtive {get; set ;}
    public DateTime StartDate {get; set;}
    public int NumberOfTotalTasks {get; set;}
    public int NumberOfCompletedTasks {get; set;}
    public bool CanDelete {get; set;} = false;
    public bool CanMark {get; set;} = false;

    // CanDelete, CanMark
}