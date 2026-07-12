using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.ConfirmOrder
{
    public sealed record ConfirmOrderCommand(
        Guid OrderId,
        string TransactionId) : IRequest<Result>;
}
