using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Commands.AssignPermission
{
    public sealed class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public AssignPermissionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

            if (role is null)
                return Result.Failure(Error.NotFound("Role.NotFound", "Rol bulunamadı."));

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == request.PermissionId, cancellationToken);

            if (permission is null)
                return Result.Failure(Error.NotFound("Permission.NotFound", "İzin bulunamadı."));

            role.GrantPermission(permission);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
