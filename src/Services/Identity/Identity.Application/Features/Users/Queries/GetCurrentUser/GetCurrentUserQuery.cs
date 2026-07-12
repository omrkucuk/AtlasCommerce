using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Features.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Queries.GetCurrentUser
{
    public sealed record GetCurrentUserQuery(Guid KeycloakId) : IRequest<Result<UserDto>>;
}
