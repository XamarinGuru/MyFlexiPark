using System;
using FluentValidation;

namespace FlexyPark.Core.Validators
{
    public class SignInValidator : AbstractValidator<string[]>
    {
        public SignInValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o=>o[0]).NotNull().NotEmpty().WithName("Email");
            RuleFor(o=>o[0]).EmailAddress().WithName("Email");
            RuleFor(o => o[1]).NotNull().NotEmpty().WithName("Password");
        }
    }
}

