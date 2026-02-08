using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Vortex.Domain.Entities;

public class UserEntity: IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}";
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    public Guid UpdatedBy { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public Guid RoleId { get; set; }

    [ForeignKey("RoleId")]
    public RoleEntity Role { get; set; } = null!;

    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
}