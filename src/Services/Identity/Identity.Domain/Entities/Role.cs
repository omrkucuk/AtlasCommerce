using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class Role : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Role() { }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        public static Role Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Rol adı boş olamaz.", nameof(name));

            return new Role { Name = name, Description = description };
        }

        public void GrantPermission(Permission permission)
        {
            if (_rolePermissions.Any(rp => rp.PermissionId == permission.Id))
                return;

            _rolePermissions.Add(RolePermission.Create(Id, permission.Id));
        }

        public void RevokePermission(Guid permissionId)
        {
            var existing = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (existing is not null)
                _rolePermissions.Remove(existing);
        }
    }
}
