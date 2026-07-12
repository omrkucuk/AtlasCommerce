using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<RolePermission> RolePermissions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
    }
}
