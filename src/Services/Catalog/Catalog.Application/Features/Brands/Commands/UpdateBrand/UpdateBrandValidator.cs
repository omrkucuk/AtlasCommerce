using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Brands.Commands.UpdateBrand
{
    public sealed class UpdateBrandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Marka Id boş olamaz.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Marka adı boş olamaz.")
                .MaximumLength(200).WithMessage("Marka adı 200 karakterden uzun olamaz.");
        }
    }
}
