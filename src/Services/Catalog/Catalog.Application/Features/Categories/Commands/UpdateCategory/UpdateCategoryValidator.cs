using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Kategori Id boş olamaz.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MaximumLength(200).WithMessage("Kategori adı 200 karakterden uzun olamaz.");
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sıra numarası negatif olamaz.");
        }
    }
}
