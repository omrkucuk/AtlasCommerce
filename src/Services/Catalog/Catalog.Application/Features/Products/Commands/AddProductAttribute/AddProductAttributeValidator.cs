using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Products.Commands.AddProductAttribute
{
    public sealed class AddProductAttributeValidator : AbstractValidator<AddProductAttributeCommand>
    {
        public AddProductAttributeValidator()
        {
            RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Ürün Id boş olamaz.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Attribute adı boş olamaz.")
                .MaximumLength(100).WithMessage("Attribute adı 100 karakterden uzun olamaz.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Attribute değeri boş olamaz.")
                .MaximumLength(500).WithMessage("Attribute değeri 500 karakterden uzun olamaz.");
        }
    }
}
