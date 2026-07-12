using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.CreateBrand
{
    public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı boş olamaz.")
            .MaximumLength(200).WithMessage("Marka adı 200 karakterden uzun olamaz.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).When(x => x.Description is not null)
                .WithMessage("Açıklama 1000 karakterden uzun olamaz.");
        }
    }
}
