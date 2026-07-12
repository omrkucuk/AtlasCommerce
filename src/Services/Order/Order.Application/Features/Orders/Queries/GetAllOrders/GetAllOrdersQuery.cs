using AtlasCommerce.BuildingBlocks.Common.Pagination;
using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.Features.Orders.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.GetAllOrders
{
    public sealed record GetAllOrdersQuery(
        int Page = 1,
        int PageSize = 20,
        string? Status = null) : IRequest<Result<PagedResult<OrderListDto>>>;
}
