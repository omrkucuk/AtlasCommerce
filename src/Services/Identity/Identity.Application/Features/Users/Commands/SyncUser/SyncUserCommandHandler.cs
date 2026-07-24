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

namespace Identity.Application.Features.Users.Commands.SyncUser
{
    public sealed class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public SyncUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.KeycloakId == request.KeycloakId, cancellationToken);

            if(existingUser is not null)
            {
                await SyncRolesAsync(existingUser, request.Roles, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(existingUser.Id);
            }

            var newUser = User.Create(request.KeycloakId, request.Email, request.FirstName, request.LastName);

            await SyncRolesAsync(newUser, request.Roles, cancellationToken);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(newUser.Id);
        }

        private async Task SyncRolesAsync(User user, IReadOnlyList<string> roleNames, CancellationToken cancellationToken)
        {
            var roles = await _context.Roles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync(cancellationToken);

            foreach(var role in roles)
            {
                user.AssignRole(role);
            }
        }
    }
}
