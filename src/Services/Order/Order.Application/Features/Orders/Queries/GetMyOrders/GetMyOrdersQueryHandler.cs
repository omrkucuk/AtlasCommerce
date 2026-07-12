using AtlasCommerce.BuildingBlocks.Common.Pagination;
using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.Features.Orders.DTOs;
using Order.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.GetMyOrders
{
    public sealed class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderListDto>>>
    {
        private readonly IOrderRepository _repository;

        public GetMyOrdersQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<OrderListDto>>> Handle(
            GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetByUserIdAsync(
                request.UserId, request.Page, request.PageSize, cancellationToken);

            var dtos = items.Select(o => o.ToListDto()).ToList();

            return Result.Success(PagedResult<OrderListDto>.Create(
                dtos, (int)totalCount, request.Page, request.PageSize));
        }
    }
}
