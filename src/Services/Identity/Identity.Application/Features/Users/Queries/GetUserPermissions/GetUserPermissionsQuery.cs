using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Queries.GetUserPermissions
{
    public sealed record GetUserPermissionsQuery(Guid KeycloakId) : IRequest<Result<IReadOnlyList<string>>>;
}
