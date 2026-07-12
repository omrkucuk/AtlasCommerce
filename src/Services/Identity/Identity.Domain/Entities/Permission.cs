using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public sealed class Permission : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Permission() { }

        public static Permission Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("İzin adı boş olamaz.", nameof(name));

            return new Permission { Name = name, Description = description };
        }
    }
}
