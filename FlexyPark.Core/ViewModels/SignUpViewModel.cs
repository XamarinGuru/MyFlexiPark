using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Validators;
using FluentValidation.Results;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Text.RegularExpressions;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        #region Constructors

        public SignUpViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init()
        {
                
        }

        #endregion

        #region Properties

        #region FirstName

        private string mFirstName = string.Empty;

        public string FirstName
        {
            get
            {
                return mFirstName;
            }
            set
            {
                mFirstName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region LastName

        private string mLastName = string.Empty;

        public string LastName
        {
            get
            {
                return mLastName;
            }
            set
            {
                mLastName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PhoneNumber

        private string mPhoneNumber = string.Empty;

        public string PhoneNumber
        {
            get
            {
                return mPhoneNumber;
            }
            set
            {
                Regex regex = new Regex(@"^[0-9]+$");
                if (regex.IsMatch(value))
                {
                    mPhoneNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region PromoCode


        private string mPromoCode;

        public string PromoCode
        {
            get
            {
                return mPromoCode;
            }
            set
            {
                mPromoCode = value;
                RaisePropertyChanged();
            }
        }

        #endregion  

        #region ReferenceName

        private string mReferenceName = string.Empty;

        public string ReferenceName
        {
            get
            {
                return mReferenceName;
            }
            set
            {
                mReferenceName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Email

        private string mEmail = string.Empty;

        public string Email
        {
            get
            {
                return mEmail;
            }
            set
            {
                mEmail = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Password

        private string mPassword = string.Empty;

        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                mPassword = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsTermsAccepted

        private bool mIsTermsAccepted = false;

        public bool IsTermsAccepted
        {
            get
            {
                return mIsTermsAccepted;
            }
            set
            {
                mIsTermsAccepted = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoTermsOfUseCommand

        private MvxCommand mGotoTermsOfUseCommand = null;

        public MvxCommand GotoTermsOfUseCommand
        {
            get
            {
                if (mGotoTermsOfUseCommand == null)
                {
                    mGotoTermsOfUseCommand = new MvxCommand(this.GotoTermsOfUse);
                }
                return mGotoTermsOfUseCommand;
            }
        }

        private void GotoTermsOfUse()
        {

        }

        #endregion

        #region SignUpCommand

        private MvxCommand mSignUpCommand = null;

        public MvxCommand SignUpCommand
        {
            get
            {
                if (mSignUpCommand == null)
                {
                    mSignUpCommand = new MvxCommand(this.SignUp);
                }
                return mSignUpCommand;
            }
        }

        private void SignUp()
        {
            var toValidate = new SignUpObject() { FirstName = FirstName, LastName = LastName, PhoneNumber = PhoneNumber, Email = Email, Password = Password, AcceptTerms = IsTermsAccepted };
            SignUpValidator validator = new SignUpValidator();
            ValidationResult results = validator.Validate(toValidate);

            if(results.IsValid)
            {
                //call api to sign up

                //then go to menu page
                ShowViewModel<MenuViewModel>();
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, results.Errors[0].ToString().Trim()));
            }
        }

        #endregion

        #region AcceptTermsCommand

        private MvxCommand mAcceptTermsCommand = null;

        public MvxCommand AcceptTermsCommand
        {
            get
            {
                if (mAcceptTermsCommand == null)
                {
                    mAcceptTermsCommand = new MvxCommand(this.AcceptTerms);
                }
                return mAcceptTermsCommand;
            }
        }

        private void AcceptTerms()
        {
            IsTermsAccepted = !IsTermsAccepted;
        }

        #endregion

        #endregion

        #region Methods

        #endregion

       
    }
}

