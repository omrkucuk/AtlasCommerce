using AtlasCommerce.BuildingBlocks.Common.Abstract;
using AtlasCommerce.BuildingBlocks.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.Common.Interceptors
{
    public sealed class AuditInterceptor<TContext, TAuditLog>: SaveChangesInterceptor
        where TContext : DbContext where TAuditLog: AuditLogEntry, new()
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                ProcessAudit(eventData.Context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
                ProcessAudit(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        private void ProcessAudit(DbContext context)
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserService.UserId;
            
            var auditLog = new List<TAuditLog>();

            foreach(var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is not AuditableEntity auditableEntity)
                    continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedBy = userId;

                        auditLog.Add(CreateAuditLog(entry, "Create", null, GetCurrentValues(entry), userId, now));
                        break;

                    case EntityState.Modified:
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedBy = userId;

                        auditLog.Add(CreateAuditLog(entry, "Update", GetOriginalValues(entry), GetCurrentValues(entry), userId, now));
                        break;

                    case EntityState.Deleted:
                        auditLog.Add(CreateAuditLog(entry, "HardDelete", GetOriginalValues(entry), null, userId, now));
                        break;
                }
            }

            if (auditLog.Any())
                context.Set<TAuditLog>().AddRange(auditLog);
        }

        private TAuditLog CreateAuditLog(EntityEntry entry, string action, string? oldValues, string? newValues, string? changedBy, DateTime changedAt)
        {
            return new TAuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetEntityId(entry),
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                ChangedBy = changedBy,
                ChangedAt = changedAt
            };
        }

        private static string GetEntityId(EntityEntry entry)
        {
            var keyValues = entry.Metadata.FindPrimaryKey()?.Properties
                .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "null");

            return keyValues is not null ? string.Join("|", keyValues) : "unknown";
        }

        private static string? GetOriginalValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();

            foreach (var prop in entry.OriginalValues.Properties)
            {
                values[prop.Name] = entry.OriginalValues[prop];
            }

            return JsonSerializer.Serialize(values);
        }

        private static string? GetCurrentValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();

            foreach (var prop in entry.CurrentValues.Properties)
            {
                values[prop.Name] = entry.CurrentValues[prop];
            }

            return JsonSerializer.Serialize(values);
        }


    }
}
