using AtlasCommerce.BuildingBlocks.Common.Results;
using Identity.Application.Features.Permissions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Permissions.Queries.GetAllPermissions
{
    public sealed record GetAllPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionDto>>>;
}
