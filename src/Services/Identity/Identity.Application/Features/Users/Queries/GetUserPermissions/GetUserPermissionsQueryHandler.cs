using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Queries.GetUserPermissions
{
    public sealed class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<IReadOnlyList<string>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserPermissionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IReadOnlyList<string>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {

            var userExists = await _context.Users
                .AnyAsync(u => u.KeycloakId == request.KeycloakId, cancellationToken);

            if (!userExists)
            {
                return Result.Failure<IReadOnlyList<string>>(
                    Error.NotFound("User.NotFound", "Kullanıcı bulunamadı."));
            }

            var permissions = await _context.Users
                .Where(user => user.KeycloakId == request.KeycloakId)
                .Join(_context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => userRole)
                .Join(_context.RolePermissions,
                    userRole => userRole.RoleId,
                    rolePermission => rolePermission.RoleId,
                    (userRole, rolePermission) => rolePermission)
                .Join(_context.Permissions,
                    rolePermission => rolePermission.PermissionId,
                    permissions => permissions.Id,
                    (rolePermission, permission) => permission.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<string>>(permissions);
        }
    }
}
