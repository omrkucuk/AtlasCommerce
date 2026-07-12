using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.ShipOrder
{
    public sealed record ShipOrderCommand(
        Guid OrderId,
        string CargoTrackingNumber) : IRequest<Result>;
}
