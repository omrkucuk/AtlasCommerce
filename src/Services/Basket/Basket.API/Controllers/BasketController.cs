using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            IBasketService basketService, 
            IOrderService orderService, 
            ILogger<BasketController> logger)
        {
            _basketService = basketService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasket(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct)
                ?? new CustomerBasket { UserId = userId };

            return Ok(basket);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            if (request.Quantity <= 0)
                return BadRequest(new { message = "Miktar sıfırdan büyük olmalı." });

            var basket = await _basketService.GetBasketAsync(userId, ct)
                ?? new CustomerBasket { UserId = userId };

            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if(existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UnitPrice = request.UnitPrice; // Fiyat güncellenmiş olabilir
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    Sku = request.Sku,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    ProductImageUrl = request.ProductImageUrl
                });
            }

            var updated = await _basketService.UpdateBasketAsync(basket, ct);
            return Ok(updated);
        }

        [HttpPut("items/{productId:guid}")]
        public async Task<IActionResult> UpdateItemQuantity(
            Guid productId, 
            [FromBody] UpdateItemQuantityRequest request,
            CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct);
            if (basket is null) return NotFound(new { message = "Sepet bulunamadı." });

            var item = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item is null) return NotFound(new { message = "Ürün sepette bulunamadı" });

            if(request.Quantity <= 0)
            {
                basket.Items.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            var updated = await _basketService.UpdateBasketAsync(basket, ct);
            return Ok(updated);
        }

        [HttpDelete("items/{productId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct);
            if (basket is null) return NotFound(new { message = "Sepet bulunamadı." });

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return NotFound(new { message = "Ürün sepette bulunamadı." });

            basket.Items.Remove(item);
            await _basketService.UpdateBasketAsync(basket, ct);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearBasket(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            await _basketService.DeleteBasketAsync(userId, ct);
            return NoContent();
        }

        [HttpPost("coupon")]
        public async Task<IActionResult> ApplyCoupon(
        [FromBody] ApplyCouponRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct);
            if (basket is null) return NotFound(new { message = "Sepet bulunamadı." });

            // Şimdilik sabit kuponlar — ileride DB'den okunabilir
            var coupon = GetCoupon(request.Code);
            if (coupon is null)
                return BadRequest(new { message = "Geçersiz kupon kodu." });

            basket.Coupon = coupon;
            var updated = await _basketService.UpdateBasketAsync(basket, ct);
            return Ok(updated);
        }

        [HttpDelete("coupon")]
        public async Task<IActionResult> RemoveCoupon(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct);
            if (basket is null) return NotFound(new { message = "Sepet bulunamadı." });

            basket.Coupon = null;
            await _basketService.UpdateBasketAsync(basket, ct);
            return NoContent();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var basket = await _basketService.GetBasketAsync(userId, ct);
            if (basket is null || !basket.Items.Any())
                return BadRequest(new { message = "Sepet boş veya bulunamadı." });

            // Authorization header'ından token al
            var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                _logger.LogWarning("Authorization header bulunamadı.");
                return Unauthorized();
            }

            var accessToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader["Bearer ".Length..].Trim()
                : authHeader.Trim();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Token boş.");
                return Unauthorized();
            }

            try
            {
                var orderNumber = await _orderService.CreateOrderAsync(basket, request, accessToken, ct);

                // Siparişten sonra sepeti temizle
                await _basketService.DeleteBasketAsync(userId, ct);

                return Ok(new { orderNumber, message = "Sipariş başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout sırasında hata oluştu. UserId: {UserId}", userId);
                return StatusCode(500, new { message = "Sipariş oluşturulurken bir hata oluştu." });
            }
        }

        private static Coupon? GetCoupon(string code) => code.ToUpper() switch
        {
            "ATLAS10" => new Coupon { Code = "ATLAS10", DiscountType = DiscountType.Percentage, DiscountValue = 10 },
            "ATLAS50" => new Coupon { Code = "ATLAS50", DiscountType = DiscountType.Fixed, DiscountValue = 50 },
            "ATLAS20" => new Coupon { Code = "ATLAS20", DiscountType = DiscountType.Percentage, DiscountValue = 20 },
            _ => null
        };

        private string? GetUserId()
        {
            return User.FindFirst("sub")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
