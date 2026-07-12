using Order.Domain.Entities;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.DTOs
{
    public static class OrderMappings
    {
        public static OrderDto ToDto(this Order.Domain.Entities.Order order) => new OrderDto(
            order.Id,
            order.OrderNumber,
            order.Status.ToString(),
            order.UserId,
            order.ShippingAddress.ToAddressDto(),
            order.BillingAddress.ToAddressDto(),
            order.PaymentInfo.ToDto(),
            order.Items.Select(i => i.ToDto()).ToList(),
            order.Notes
                .Where(n => true)
                .Select(n => n.ToDto()).ToList(),
            order.SubTotal.ToDto(),
            order.ShippingFee.ToDto(),
            order.TotalAmount.ToDto(),
            order.CargoTrackingNumber,
            order.CreatedAt,
            order.UpdatedAt);

        public static OrderListDto ToListDto(this Domain.Entities.Order order) => new(
        order.Id,
        order.OrderNumber,
        order.Status.ToString(),
        order.Items.Count,
        order.TotalAmount.ToDto(),
        order.CreatedAt);

        public static OrderItemDto ToDto(this OrderItem item) => new(
            item.Id,
            item.ProductId,
            item.ProductName,
            item.Sku,
            item.ProductImageUrl,
            item.Quantity,
            item.UnitPrice.ToDto(),
            item.TotalPrice.ToDto());

        public static OrderSummaryAddressDto ToAddressDto(this Address address) => new(
            address.FullName,
            address.Phone,
            address.City,
            address.District,
            address.FullAddress,
            address.ZipCode,
            address.Country);

        public static PaymentInfoDto ToDto(this PaymentInfo info) => new(
            info.Method.ToString(),
            info.Status.ToString(),
            info.TransactionId,
            info.PaidAt);

        public static OrderNoteDto ToDto(this OrderNote note) => new(
            note.Id,
            note.Content,
            note.AddedBy,
            note.AddedAt,
            note.IsCustomerVisible);

        public static MoneyDto ToDto(this Money money) => new(money.Amount, money.Currency);
    }
}
