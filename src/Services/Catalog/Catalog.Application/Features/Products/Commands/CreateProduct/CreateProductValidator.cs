using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MaximumLength(300).WithMessage("Ürün adı 300 karakterden uzun olamaz.");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU boş olamaz.")
                .MaximumLength(100).WithMessage("SKU 100 karakterden uzun olamaz.");

            RuleFor(x => x.BasePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori seçilmeli.");

            RuleFor(x => x.BrandId)
                .NotEmpty().WithMessage("Marka seçilmeli.");

            RuleFor(x => x.Description)
                .MaximumLength(5000).When(x => x.Description is not null)
                .WithMessage("Açıklama 5000 karakterden uzun olamaz.");
        }
    }
}
