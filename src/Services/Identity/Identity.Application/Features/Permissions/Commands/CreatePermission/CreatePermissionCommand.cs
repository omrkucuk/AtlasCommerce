using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Permissions.Commands.CreatePermission
{
    public sealed record CreatePermissionCommand(string Name, string? Description): IRequest<Result<Guid>>;
}
