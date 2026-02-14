using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vortex.Domain.Entities;

[Table("tbl_audit_logs")]
public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    public string ChangeType { get; set; } = string.Empty;

    [Required]
    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public string? ParentEntityType { get; set; }

    public Guid? ParentEntityId { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime DateTime { get; set; }

    [Column(TypeName = "jsonb")]
    public string? OldValues { get; set; }

    [Column(TypeName = "jsonb")]
    public string? NewValues { get; set; }

    [Column(TypeName = "jsonb")]
    public string? AffectedColumns { get; set; }
}
