using Microsoft.AspNetCore.Identity;

namespace Vortex.Domain.Entities;

public class UserEntity: IdentityUser<Guid>
{
    public string FisrtName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FisrtName} ${LastName}";
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
    public Guid UpdatedBy { get; set; }
    public string ProfilePicture { get; set; }
    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
}