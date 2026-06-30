using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Auth.DTOs
{
    public sealed record KeycloakTokenResult(
        bool IsSuccess,
        string? AccessToken,
        string? RefreshToken,
        int ExpiresInSeconds,
        string? ErrorMessage);
}
