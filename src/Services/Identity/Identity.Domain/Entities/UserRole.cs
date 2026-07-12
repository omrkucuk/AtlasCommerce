using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class UserRole
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

        private UserRole() { }

        internal static UserRole Create(Guid userId, Guid roleId) =>
            new() { UserId = userId, RoleId = roleId };
    }
}
