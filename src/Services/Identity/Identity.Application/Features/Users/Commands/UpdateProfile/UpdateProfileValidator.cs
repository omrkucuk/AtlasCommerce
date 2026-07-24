using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad zorunlu.")
                .MaximumLength(100).WithMessage("Ad 100 karakterden uzun olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad zorunlu.")
                .MaximumLength(100).WithMessage("Soyad 100 karakterden uzun olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta zorunlu.")
                .EmailAddress().WithMessage("Geçerli bir e-posta girin.");
        }
    }
}
