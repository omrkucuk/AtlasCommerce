using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Permissions.Commands.CreatePermission
{
    public sealed class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreatePermissionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _context.Permissions
                .AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (nameExists)
            {
                return Result.Failure<Guid>(
                   Error.Conflict("Permission.NameAlreadyExists", $"'{request.Name}' adında bir izin zaten var."));
            }

            var permission = Permission.Create(request.Name, request.Description);

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(permission.Id);
        }
    }
}
