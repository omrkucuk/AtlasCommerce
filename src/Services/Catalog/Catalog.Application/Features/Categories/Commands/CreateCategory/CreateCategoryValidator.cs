using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MaximumLength(200).WithMessage("Kategori adı 200 karakterden uzun olamaz.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sıra numarası negatif olamaz.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).When(x => x.Description is not null)
                .WithMessage("Açıklama 1000 karakterden uzun olamaz.");
        }
    }
}
