using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Commands.SyncUser
{
    public sealed record SyncUserCommand(
        Guid KeycloakId,
        string Email,
        string FirstName,
        string LastName,
        IReadOnlyList<string> Roles) : IRequest<Result<Guid>>;
}
