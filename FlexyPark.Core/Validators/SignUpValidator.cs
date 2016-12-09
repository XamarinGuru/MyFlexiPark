using System;
using FluentValidation;

namespace FlexyPark.Core.Validators
{
    public class SignUpValidator : AbstractValidator<SignUpObject>
    {
        public SignUpValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o=>o.FirstName).NotNull().NotEmpty();
            RuleFor(o=>o.LastName).NotNull().NotEmpty();
            RuleFor(o=>o.PhoneNumber).NotNull().NotEmpty();
            RuleFor(o=>o.Email).EmailAddress();
            RuleFor(o=>o.Password).NotNull().NotEmpty();
            RuleFor(o=>o.AcceptTerms).Equal(true).WithMessage("Please accept terms of use !");
        }
    }

    public class SignUpObject
    {
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string PhoneNumber {get;set;}
        public string Email {get;set;}
        public string Password {get;set;}
        public bool AcceptTerms {get;set;}
    }
}

