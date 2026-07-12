using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Permissions.DTOs
{
    public sealed record PermissionDto(Guid Id, string Name, string? Description);
}
