using Basket.API.Models;
using System.ComponentModel.Design;

namespace Basket.API.Services
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(
            CustomerBasket basket,
            CheckoutRequest checkoutRequest,
            string accessToken,
            CancellationToken ct);
    }
}
