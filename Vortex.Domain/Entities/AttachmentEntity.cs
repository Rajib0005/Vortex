namespace Vortex.Domain.Entities;

public class AttachmentEntity
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required  string FilePath { get; set; }
    public required  string ContentType { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    public Guid UpdatedBy { get; set; }
}