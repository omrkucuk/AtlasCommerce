using Basket.API.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.API.Services
{
    public sealed class RedisBasketService : IBasketService
    {
        private readonly IDatabase _db;
        private readonly TimeSpan _expiry = TimeSpan.FromDays(7);

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisBasketService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private static string GetKey(string userId) => $"basket:{userId}";

        public async Task DeleteBasketAsync(string userId, CancellationToken ct)
        {
            await _db.KeyDeleteAsync(GetKey(userId));
        }

        public async Task<CustomerBasket?> GetBasketAsync(string userId, CancellationToken ct)
        {
            var data = await _db.StringGetAsync(GetKey(userId));
            if (data.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<CustomerBasket?>(data!, _jsonOptions);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(basket);
            await _db.StringSetAsync(GetKey(basket.UserId), json, _expiry);
            return basket;
        }
    }
}
