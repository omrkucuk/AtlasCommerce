using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Roles.Dtos
{
    public sealed record RoleDto(
        Guid Id,
        string Name,
        string? Description,
        IReadOnlyList<string> Permissions);
}
