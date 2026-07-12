using Identity.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces
{
    public interface IKeycloakTokenService
    {
        Task<KeycloakTokenResult> LoginAsync(string username, string password, CancellationToken cancellationToken);
        Task<KeycloakTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
