using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vortex.Application.Interfaces;
using Vortex.Domain.Common;
using Vortex.Domain.Entities;

namespace Vortex.Infrastructure.Interceptors;

public class AuditableEntityInterceptor(
    ICorrelationIdService correlationIdService,
    ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IAuditable>().ToList();
        var auditLogs = new List<AuditLog>();
        var userId = currentUserService.UserId;
        var correlationId = correlationIdService.CorrelationId;
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CorrelationId = correlationId,
                DateTime = utcNow,
                ChangeType = entry.State.ToString(),
                EntityType = entry.Entity.GetType().Name
            };

            // Get Primary Key (assuming single generic Id)
            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
            if (idProperty != null && idProperty.CurrentValue != null)
            {
                 if (Guid.TryParse(idProperty.CurrentValue.ToString(), out var guidId))
                 {
                     auditEntry.EntityId = guidId;
                 }
            }

            // Support Parent Entity
            if (entry.Entity is ISupportParent supportParent)
            {
                var (parentType, parentId) = supportParent.GetParentInfo();
                auditEntry.ParentEntityType = parentType;
                auditEntry.ParentEntityId = parentId;
            }

            // Project Related
            if (entry.Entity is IProjectRelated projectRelated)
            {
                auditEntry.ProjectId = projectRelated.ProjectId;
            }
            // Fallback: If the entity itself IS a ProjectEntity, use its ID (handled via IProjectRelated on ProjectEntity above, but let's be explicit if needed or just rely on interface)
            // Since we added IProjectRelated to ProjectEntity, the above block covers it!


            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();
            var affectedColumns = new List<string>();

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue;
             
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey()) continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        newValues[propertyName] = property.CurrentValue;
                        affectedColumns.Add(propertyName);
                        break;

                    case EntityState.Deleted:
                        oldValues[propertyName] = property.OriginalValue;
                        affectedColumns.Add(propertyName);
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            oldValues[propertyName] = property.OriginalValue;
                            newValues[propertyName] = property.CurrentValue;
                            affectedColumns.Add(propertyName);
                        }
                        break;
                }
            }

            auditEntry.OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues);
            auditEntry.NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues);
            auditEntry.AffectedColumns = affectedColumns.Count == 0 ? null : JsonSerializer.Serialize(affectedColumns);

            auditLogs.Add(auditEntry);
        }

        if (auditLogs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }
    }
}
