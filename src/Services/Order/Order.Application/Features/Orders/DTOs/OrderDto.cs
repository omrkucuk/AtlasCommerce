using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.DTOs
{
    public sealed record OrderDto(
        Guid Id,
        string OrderNumber,
        string Status,
        Guid UserId,
        OrderSummaryAddressDto ShippingAddress,
        OrderSummaryAddressDto BillingAddress,
        PaymentInfoDto PaymentInfo,
        IReadOnlyList<OrderItemDto> Items,
        IReadOnlyList<OrderNoteDto> Notes,
        MoneyDto SubTotal,
        MoneyDto ShippingFee,
        MoneyDto TotalAmount,
        string? CargoTrackingNumber,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public sealed record OrderListDto(
        Guid Id,
        string OrderNumber,
        string Status,
        int ItemCount,
        MoneyDto TotalAmount,
        DateTime CreatedAt);

    public sealed record OrderItemDto(
        Guid Id,
        Guid ProductId,
        string ProductName,
        string Sku,
        string? ProductImageUrl,
        int Quantity,
        MoneyDto UnitPrice,
        MoneyDto TotalPrice);

    public sealed record OrderSummaryAddressDto(
        string FullName,
        string Phone,
        string City,
        string District,
        string FullAddress,
        string ZipCode,
        string Country);

    public sealed record PaymentInfoDto(
        string Method,
        string Status,
        string? TransactionId,
        DateTime? PaidAt);

    public sealed record OrderNoteDto(
        Guid Id,
        string Content,
        string AddedBy,
        DateTime AddedAt,
        bool IsCustomerVisible);

    public sealed record MoneyDto(decimal Amount, string Currency);
}
