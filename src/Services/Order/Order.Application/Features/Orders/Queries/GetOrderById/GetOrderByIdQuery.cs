using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.Features.Orders.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderDto>>;
}
