using System;
using FluentValidation;

namespace FlexyPark.Core.Validators
{
    public class CustomCreditCardValidator : AbstractValidator<string[]>
    {
        public CustomCreditCardValidator ()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o=>o[0]).NotNull().NotEmpty().WithName("Credit card number");
            RuleFor(o => o[1]).NotNull().NotEmpty().WithName("Credit card holder name");
            RuleFor (o => o [2]).NotNull ().NotEmpty ().WithName ("Credit card cryptogram");
            RuleFor (o => o [3]).NotNull ().NotEmpty ().WithName ("Credits");

        }
    }
}

