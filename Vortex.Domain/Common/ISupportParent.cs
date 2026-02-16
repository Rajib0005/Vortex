namespace Vortex.Domain.Common;

public interface ISupportParent
{
    /// <summary>
    /// Returns the type and ID of the parent entity for auditing correlation.
    /// </summary>
    (string ParentType, Guid? ParentId) GetParentInfo();
}
