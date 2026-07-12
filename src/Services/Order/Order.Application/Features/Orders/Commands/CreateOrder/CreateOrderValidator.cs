using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId boş olamaz.");

            RuleFor(x => x.Items).NotEmpty().WithMessage("Sipariş en az bir ürün içermeli.");

            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Ödeme yöntemi seçilmeli.");

            RuleFor(x => x.ShippingFee)
                .GreaterThanOrEqualTo(0).WithMessage("Kargo ücreti negatif olamaz.");

            RuleFor(x => x.ShippingAddress.FirstName)
                .NotEmpty().WithMessage("Teslimat adresi: Ad boş olamaz.");
            RuleFor(x => x.ShippingAddress.LastName)
                .NotEmpty().WithMessage("Teslimat adresi: Soyad boş olamaz.");
            RuleFor(x => x.ShippingAddress.Phone)
                .NotEmpty().WithMessage("Teslimat adresi: Telefon boş olamaz.");
            RuleFor(x => x.ShippingAddress.City)
                .NotEmpty().WithMessage("Teslimat adresi: Şehir boş olamaz.");
            RuleFor(x => x.ShippingAddress.FullAddress)
                .NotEmpty().WithMessage("Teslimat adresi: Adres boş olamaz.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Ürün Id boş olamaz.");
                item.RuleFor(i => i.ProductName).NotEmpty().WithMessage("Ürün adı boş olamaz.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalı.");
                item.RuleFor(i => i.UnitPrice).GreaterThan(0).WithMessage("Birim fiyat sıfırdan büyük olmalı.");
            });
        }
    }
}
