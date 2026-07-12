using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(
        Guid OrderId,
        string Reason,
        string CancelledBy) : IRequest<Result>;
}
