using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Commands.RevokePermission
{
    public sealed class RevokePermissionCommandHandler : IRequestHandler<RevokePermissionCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public RevokePermissionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(RevokePermissionCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

            if (role is null)
                return Result.Failure(Error.NotFound("Role.NotFound", "Rol bulunamadı."));

            role.RevokePermission(request.PermissionId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
