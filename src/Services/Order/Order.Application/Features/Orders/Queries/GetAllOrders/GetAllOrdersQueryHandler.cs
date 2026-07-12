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

namespace Order.Application.Features.Orders.Queries.GetAllOrders
{
    public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PagedResult<OrderListDto>>>
    {
        private readonly IOrderRepository _repository;

        public GetAllOrdersQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<OrderListDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, request.Status, cancellationToken);

            var dtos = items.Select(o => o.ToListDto()).ToList();

            return Result.Success(PagedResult<OrderListDto>.Create(dtos, (int)totalCount, request.Page, request.PageSize));
        }
    }
}
