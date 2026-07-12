using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.DTOs
{
    public sealed record BrandDto(Guid Id, string Name, string? Description, string? LogoUrl, bool IsActive);
}
