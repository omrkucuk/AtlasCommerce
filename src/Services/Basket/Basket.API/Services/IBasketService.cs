using Basket.API.Models;

namespace Basket.API.Services
{
    public interface IBasketService
    {
        Task<CustomerBasket?> GetBasketAsync(string userId, CancellationToken ct);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket, CancellationToken ct);
        Task DeleteBasketAsync(string userId, CancellationToken ct);
    }
}
