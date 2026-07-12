using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using Order.Application.Features.Orders.DTOs;
using Order.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IOrderRepository _repository;

        public GetOrderByIdQueryHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Failure<OrderDto>(Error.NotFound("Order.NotFound", "Sipariş bulunamadı."));

            return Result.Success(order.ToDto());
        }
    }
}
