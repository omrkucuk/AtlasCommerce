using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces
{
    public interface IKeycloakAdminService
    {
        Task<KeycloakAdminResult> CreateUserAsync(
            string username,
            string email,
            string password,
            string firstName,
            string lastName,
            CancellationToken cancellationToken);
    }

    public sealed record KeycloakAdminResult(
        bool IsSuccess,
        Guid? KeycloakId,
        string? ErrorMessage);
}
