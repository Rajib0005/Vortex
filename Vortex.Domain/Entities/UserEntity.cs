using Microsoft.AspNetCore.Identity;

namespace Vortex.Domain.Entities;

public class UserEntity: IdentityUser<Guid>
{
    public string? FisrtName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FisrtName} ${LastName}";
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    public Guid UpdatedBy { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
}