using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        private RolePermission() { }

        internal static RolePermission Create(Guid roleId, Guid permissionId) =>
            new() { RoleId = roleId, PermissionId = permissionId };
    }
}
