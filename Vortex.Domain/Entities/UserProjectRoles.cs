using System.ComponentModel.DataAnnotations;

namespace Vortex.Domain.Entities;

public class UserProjectRole
{
    
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public ProjectEntity Project { get; set; } = null!;
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
}
