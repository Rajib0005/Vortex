using System.ComponentModel.DataAnnotations;
using Vortex.Domain.Common;

namespace Vortex.Domain.Entities;

public class CommentEntity : IAuditable, ISupportParent, IProjectRelated
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;

    public Guid TaskId { get; set; }
    public virtual TaskEntity Task { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public virtual ProjectEntity Project { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }
    public virtual CommentEntity? ParentComment { get; set; }
    public virtual ICollection<CommentEntity> Replies { get; set; } = new List<CommentEntity>();

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }

    public (string ParentType, Guid? ParentId) GetParentInfo()
    {
        return ("Task", TaskId);
    }
}
