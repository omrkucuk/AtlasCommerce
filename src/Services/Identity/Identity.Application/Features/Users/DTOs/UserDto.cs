using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.DTOs
{
    public sealed record UserDto(
        Guid Id,
        Guid KeycloakId,
        string Email,
        string FirstName,
        string LastName,
        bool IsActive,
        IReadOnlyList<string> Roles);
}
