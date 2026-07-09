using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.DTOs
{
    public sealed record ProductAttributeDto(
        Guid Id,
        string Name,
        string Value);
}
