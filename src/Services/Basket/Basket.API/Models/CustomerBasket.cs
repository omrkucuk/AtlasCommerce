namespace Basket.API.Models
{
    public sealed class CustomerBasket
    {
        public string UserId { get; set; } = string.Empty;
        public List<BasketItem> Items { get; set; } = new();
        public Coupon? Coupon { get; set; }

        public decimal SubTotal => Items.Sum(i => i.TotalPrice);
        public decimal DiscountAmount => Coupon?.CalculateDiscount(SubTotal) ?? 0;
        public decimal TotalAmount => SubTotal - DiscountAmount;
        public int TotalItemCount => Items.Sum(i => i.Quantity);
    }
}
