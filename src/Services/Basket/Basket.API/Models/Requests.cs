namespace Basket.API.Models
{
    public sealed record AddItemRequest(
        Guid ProductId,
        string ProductName,
        string Sku,
        int Quantity,
        decimal UnitPrice,
        string? ProductImageUrl = null);

    public sealed record UpdateItemQuantityRequest(Guid ProductId, int Quantity);

    public sealed record ApplyCouponRequest(string Code);

    public sealed record CheckoutAddressDto(
        string FirstName,
        string LastName,
        string Phone,
        string City,
        string District,
        string FullAddress,
        string ZipCode,
        string Country = "TR");

    public sealed record CheckoutRequest(
        CheckoutAddressDto ShippingAddress,
        CheckoutAddressDto BillingAddress,
        string PaymentMethod,
        decimal ShippingFee = 0);
}
