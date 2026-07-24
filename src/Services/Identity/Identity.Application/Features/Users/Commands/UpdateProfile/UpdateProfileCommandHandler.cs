using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UpdateProfileCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Handler çalıştı. KeycloakId: {request.KeycloakId}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.KeycloakId == request.KeycloakId, cancellationToken);

            Console.WriteLine($"Kullanıcı bulundu mu: {user is not null}");

            if (user is null)
                return Result.Failure(Error.NotFound("User.NotFound", "Kullanıcı bulunamadı."));

            Console.WriteLine($"Mevcut: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Yeni: {request.FirstName} {request.LastName}");

            user.UpdateProfile(request.FirstName, request.LastName, request.Email);

            var saved = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Kaydedilen satır: {saved}");

            return Result.Success();
        }
    }
}
