using AtlasCommerce.BuildingBlocks.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderRequest_AddressDto(
        string FirstName, string LastName, string Phone,
        string City, string District, string FullAddress,
        string ZipCode, string Country = "TR");

    public sealed record CreateOrderRequest_ItemDto(
        Guid ProductId, string ProductName, string Sku,
        int Quantity, decimal UnitPrice, string? ImageUrl = null);

    public sealed record CreateOrderCommand(
        Guid UserId,
        CreateOrderRequest_AddressDto ShippingAddress,
        CreateOrderRequest_AddressDto BillingAddress,
        string PaymentMethod,
        List<CreateOrderRequest_ItemDto> Items,
        decimal ShippingFee = 0) : IRequest<Result<string>>;
}
