using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductVariant
{
    public sealed class AddProductVariantValidator : AbstractValidator<AddProductVariantCommand>
    {
        public AddProductVariantValidator()
        {
            RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Ürün Id boş olamaz.");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("Variant SKU boş olamaz.")
                .MaximumLength(100).WithMessage("SKU 100 karakterden uzun olamaz.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(x => x.PriceOverride)
                .GreaterThanOrEqualTo(0).When(x => x.PriceOverride.HasValue)
                .WithMessage("Fiyat negatif olamaz.");

            RuleFor(x => x.Attributes)
                .NotEmpty().WithMessage("Variant en az bir attribute içermeli.");

            RuleForEach(x => x.Attributes).ChildRules(attr =>
            {
                attr.RuleFor(a => a.Name)
                    .NotEmpty().WithMessage("Attribute adı boş olamaz.");
                attr.RuleFor(a => a.Value)
                    .NotEmpty().WithMessage("Attribute değeri boş olamaz.");
            });
        }
    }
}
