using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Queries.GetCurrentUser
{
    public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCurrentUserQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.KeycloakId == request.KeycloakId)
                .Select(u => new
                {
                    u.Id,
                    u.KeycloakId,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive,
                    RoleIds = u.UserRoles.Select(ur => ur.RoleId)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if(user is null)
            {
                return Result.Failure<UserDto>(
                    Error.NotFound("User.NotFound", "Kullanıcı PostgreSql'de bulunamadı. Senkronizasyon henüz tamamlanmamış olabilir."));
            }

            var roleNames = await _context.Roles
                .Where(r => user.RoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync(cancellationToken);

            var dto = new UserDto
            (
                user.Id,
                user.KeycloakId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                roleNames
            );

            return Result.Success(dto);
        }
    }
}
