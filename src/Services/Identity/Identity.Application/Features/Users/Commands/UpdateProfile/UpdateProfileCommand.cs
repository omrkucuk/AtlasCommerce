using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Commands.UpdateProfile
{
    public sealed record UpdateProfileCommand(
        Guid KeycloakId,
        string FirstName,
        string LastName,
        string Email) : IRequest<Result>;
}
