using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Commands.CreateRole
{
    public sealed record CreateRoleCommand(string Name, string? Description) : IRequest<Result<Guid>>;
}
