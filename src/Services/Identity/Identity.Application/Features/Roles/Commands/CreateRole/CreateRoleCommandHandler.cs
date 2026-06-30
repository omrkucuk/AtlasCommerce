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

namespace Identity.Application.Features.Roles.Commands.CreateRole
{
    public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _context.Roles
                .AnyAsync(r => r.Name == request.Name, cancellationToken);

            if (nameExists)
            {
                return Result.Failure<Guid>(
                    Error.Conflict("Role.NameAlreadyExists", $"'{request.Name} adında bir rol zaten var'"));
            }

            var role = Role.Create(request.Name, request.Description);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(role.Id);
        }
    }
}
