using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Roles.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Queries.GetAllRoles
{
    public sealed class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _context.Roles
                .Select(role => new RoleDto(
                    role.Id,
                    role.Name,
                    role.Description,
                    _context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id)
                        .Join(_context.Permissions,
                            rp => rp.PermissionId,
                            permission => permission.Id,
                            (rp, permission) => permission.Name)
                        .ToList()))
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<RoleDto>>(roles);
        }
    }
}
