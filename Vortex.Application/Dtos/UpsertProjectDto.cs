namespace Vortex.Application.Dtos;

public class UpsertProjectDto
{
    public required string ProjectName { get; set; }
    public required string ProjectDescription { get; set; }
    public required string ProjectKey { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}