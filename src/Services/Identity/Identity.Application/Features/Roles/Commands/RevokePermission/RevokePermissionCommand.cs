using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Commands.RevokePermission
{
    public sealed record RevokePermissionCommand(Guid RoleId, Guid PermissionId) : IRequest<Result>;
}
