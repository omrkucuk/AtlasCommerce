using Basket.API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Basket.API.Services
{
    public sealed class OrderServiceClient : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServiceClient> _logger;

        public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> CreateOrderAsync(CustomerBasket basket, CheckoutRequest checkoutRequest, string accessToken, CancellationToken ct)
        {
            var payload = new
            {
                shippingAddress = new
                {
                    firstName = checkoutRequest.ShippingAddress.FirstName,
                    lastName = checkoutRequest.ShippingAddress.LastName,
                    phone = checkoutRequest.ShippingAddress.Phone,
                    city = checkoutRequest.ShippingAddress.City,
                    district = checkoutRequest.ShippingAddress.District,
                    fullAddress = checkoutRequest.ShippingAddress.FullAddress,
                    zipCode = checkoutRequest.ShippingAddress.ZipCode,
                    country = checkoutRequest.ShippingAddress.Country
                },
                billingAddress = new
                {
                    firstName = checkoutRequest.BillingAddress.FirstName,
                    lastName = checkoutRequest.BillingAddress.LastName,
                    phone = checkoutRequest.BillingAddress.Phone,
                    city = checkoutRequest.BillingAddress.City,
                    district = checkoutRequest.BillingAddress.District,
                    fullAddress = checkoutRequest.BillingAddress.FullAddress,
                    zipCode = checkoutRequest.BillingAddress.ZipCode,
                    country = checkoutRequest.BillingAddress.Country
                },
                paymentMethod = checkoutRequest.PaymentMethod,
                items = basket.Items.Select(i => new
                {
                    productId = i.ProductId,
                    productName = i.ProductName,
                    sku = i.Sku,
                    quantity = i.Quantity,
                    unitprice = i.UnitPrice,
                    imageUrl = i.ProductImageUrl
                }),
                shippingFee = checkoutRequest.ShippingFee,
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/orders")
            {
                Content = JsonContent.Create(payload)
            };

            // Kullanıcı token'ını Order Service'e ilet
            _logger.LogInformation("Order Service'e gönderilen token: {Token}",accessToken[..20] + "...");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Order Service sipariş oluşturamadı: {Error}", error);
                throw new InvalidOperationException($"Sipariş oluşturulamadı: {error}");
            }

            var body = await response.Content.ReadAsStringAsync(ct);

            return body.Trim('"');
        }
    }
}
