using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Ürün Id boş olamaz.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(300).WithMessage("Ürün adı 300 karakterden uzun olamaz.");

            RuleFor(x => x.BasePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kategori seçilmeli.");
            RuleFor(x => x.BrandId).NotEmpty().WithMessage("Marka seçilmeli.");
        }
    }
}
