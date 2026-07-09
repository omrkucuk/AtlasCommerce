using AtlasCommerce.BuildingBlocks.Common.Results;
using Catalog.Application.Features.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Queries.GetProductById
{
    public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
}
