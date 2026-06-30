using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Permissions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Permissions.Queries.GetAllPermissions
{
    public sealed class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPermissionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IReadOnlyList<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _context.Permissions
                .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<PermissionDto>>(permissions);
        }
    }
}
