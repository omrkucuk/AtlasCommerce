using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public Guid KeycloakId { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        private User() { }

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        public static User Create(Guid keycloakId, string email, string firstName, string lastName)
        {
            if (keycloakId == Guid.Empty)
                throw new ArgumentException("KeycloakId boş olamaz.", nameof(keycloakId));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email boş olamaz.", nameof(email));

            return new User
            {
                KeycloakId = keycloakId,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            MarkUpdated();
        }

        public void Deactive()
        {
            IsActive = false;
            MarkUpdated();
        }

        public void AssignRole(Role role)
        {
            if (_userRoles.Any(ur => ur.RoleId == role.Id))
                return;

            _userRoles.Add(UserRole.Create(Id, role.Id));
        }
    }
}
