using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.Validators;
using FluentValidation.Results;

namespace FlexyPark.Core.ViewModels
{
    public interface ILostPasswordView
    {
        string GetDeviceIdentifier ();
        void CloseKeyboard ();
    }

    public class LostPasswordViewModel : BaseViewModel
    {
        #region Constructors

        public LostPasswordViewModel (IApiService apiService, ICacheService cacheService) : base (apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init ()
        {

        }

        #endregion

        public ILostPasswordView View { get; set;}

        #region Properties

        #region Email

        private string mEmail = string.Empty;

        public string Email {
            get {
                return mEmail;
            }
            set {
                mEmail = value;
                RaisePropertyChanged ();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region OkCommand

        private MvxCommand mOkCommand = null;

        public MvxCommand OkCommand {
            get {
                if (mOkCommand == null) {
                    mOkCommand = new MvxCommand (this.Ok);
                }
                return mOkCommand;
            }
        }

        private async void Ok ()
        {
            if (View != null)
                View.CloseKeyboard ();

            Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, true));

            //validate email and password
            var toValidate = new string [] { Email , "FakePasswordToPassValidator"};
            SignInValidator validator = new SignInValidator ();
            ValidationResult results = validator.Validate (toValidate);

            if (results.IsValid) {
                //sign in
                if (BaseView != null && BaseView.CheckInternetConnection ()) {
                    Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, true));

                    if (View!=null)
                    {
                        
                        string identifier = View.GetDeviceIdentifier ();
                        var result = await mApiService.LostPassword (Email, identifier, Mvx.Resolve<IPlatformService> ().OS == OS.Touch ? AppConstants.iOSToken : AppConstants.AndroidToken);
                        if (result != null) {
                            if (result.Response != null && result.Response.Status.Equals ("success")) {
                                Close (this);
                            } else {
                                if (result.ApiError != null) {
                                    Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, result.ApiError.Error, "Ok", null));
                                }
                                else {
                                    Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                                }
                                Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                                return;
                            }
                        } else {
                            
                            Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                            return;
                        }
                    }

                } else {
                    Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, SharedTextSource.GetText ("TurnOnInternetText")));
                }



            } else {
                Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, results.Errors [0].ToString ().Trim ()));
            }

            Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
        }

        #endregion

        #endregion

        #region Methods

        #endregion


    }
}

