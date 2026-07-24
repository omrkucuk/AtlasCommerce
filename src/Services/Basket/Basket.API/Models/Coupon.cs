namespace Basket.API.Models
{
    public enum DiscountType { Fixed, Percentage }
    public sealed class Coupon
    {
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }

        public decimal CalculateDiscount(decimal subTotal) => DiscountType switch
        {
            DiscountType.Fixed => Math.Min(DiscountValue, subTotal),
            DiscountType.Percentage => Math.Round(subTotal * (DiscountValue / 100), 2),
            _ => 0
        };
    }
}
