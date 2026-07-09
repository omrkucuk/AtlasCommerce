using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.DTOs
{
    public sealed record ProductImageDto(
        Guid Id,
        string ImageUrl,
        bool IsMain,
        int DisplayOrder);
}
