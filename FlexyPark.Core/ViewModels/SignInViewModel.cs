using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Validators;
using FluentValidation.Results;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using FlexyPark.Core.Models;
using Akavache;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Globalization;
using Cirrious.MvvmCross.Localization;


namespace FlexyPark.Core.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        #region Constructors

        public SignInViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init()
        {
            Debug.WriteLine(Mvx.Resolve<IPlatformService>().GetTimeZone());

#if DEBUG
            Email = "test@test.com";
            Password = "test1234";
#endif

            //Email = "nhan.nguyen@bss.com";
            //Password = "123456";


            await System.Threading.Tasks.Task.Delay(200);

        }

        #endregion

        #region Properties

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

        #endregion

        #region Commands

        #region SignInCommand

        private MvxCommand mSignInCommand = null;

        public MvxCommand SignInCommand
        {
            get
            {
                if (mSignInCommand == null)
                {
                    mSignInCommand = new MvxCommand(this.SignIn);
                }
                return mSignInCommand;
            }
        }

        private async void SignIn()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

            //validate email and password
            var toValidate = new string[] { Email, Password };
            SignInValidator validator = new SignInValidator();
            ValidationResult results = validator.Validate(toValidate);

            if (results.IsValid)
            {
                //sign in
                if (BaseView != null && BaseView.CheckInternetConnection())
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                    //login first 
                    var loginResult = await mApiService.Login(Email, Password, 
                        Mvx.Resolve<IPlatformService>().OS == OS.Touch ? AppConstants.iOSToken : AppConstants.AndroidToken);
                    if (loginResult != null)
                    {
						//#region exception
						//if (loginResult.Result == null)
						//{
						//	mCacheService.SessionId = loginResult.SessionId;
						//	mCacheService.CurrentUser = new User(Email, Password);
						//	mCacheService.CurrentUser.UserId = loginResult.UserId;

						//	//then get user information
						//	var result = await mApiService.PersonGet(mCacheService.CurrentUser.UserId);
						//	if (result != null)
						//	{
						//		mCacheService.CurrentUser = result.Response;
						//		mCacheService.CurrentUser.Password = Password;
						//		mCacheService.CurrentUser.UserId = result.Request.UserId;

						//		BlobCache.UserAccount.InsertObject("CurrentUser", mCacheService.CurrentUser);

						//		Mvx.Resolve<IPlatformService>().SetPreference(AppConstants.IsLogin, true);
						//	}
						//}else
						//	#endregion
						if (loginResult.Status.Equals("success"))
                        {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, loginResult.UserId));

                            mCacheService.SessionId = loginResult.SessionId; 
                            mCacheService.CurrentUser = new User(Email, Password);
                            mCacheService.CurrentUser.UserId = loginResult.UserId;

							BlobCache.UserAccount.InsertObject("SessionId", mCacheService.SessionId);

                            //then get user information
                            var result = await mApiService.PersonGet(mCacheService.CurrentUser.UserId);
                            if (result != null)
                            {
                                mCacheService.CurrentUser = result.Response;
                                mCacheService.CurrentUser.Password = Password; 
                                mCacheService.CurrentUser.UserId = result.Request.UserId;

                                BlobCache.UserAccount.InsertObject("CurrentUser", mCacheService.CurrentUser);

                                Mvx.Resolve<IPlatformService>().SetPreference(AppConstants.IsLogin, true);
                            }
                        }
                        else
                        {
                            if (loginResult.ErrorCode.Equals ("888")) {
                                Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, "Token is incorrect, please contact app@myflexipark.com", "Ok", null));
                            } else {
								Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}: {1}", loginResult.Status, loginResult.ErrorCode), "Ok", null));
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                            return;
                        }
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("Email or password is incorrect"), "Ok", null));
                        Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                        return;
                    }
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                }

                //then go to menu page
                ShowViewModel<MenuViewModel>();
                Close(this);
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, results.Errors[0].ToString().Trim()));
            }

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
        }

        #endregion

        #region GotoSignUpCommand

        private MvxCommand mGotoSignUpCommand = null;

        public MvxCommand GotoSignUpCommand
        {
            get
            {
                if (mGotoSignUpCommand == null)
                {
                    mGotoSignUpCommand = new MvxCommand(this.GotoSignUp);
                }
                return mGotoSignUpCommand;
            }
        }

        private void GotoSignUp()
        {
            //ShowViewModel<SignUpViewModel>();

            //ShowViewModel<MyProfileViewModel>(new {tabIndex = MyProfileTab.Common, isSignUp = true});

            ShowViewModel<CommonProfileViewModel>(new {isSignUp = true});
        }

        #endregion

        #region LostPasswordCommand

        private MvxCommand mLostPasswordCommand = null;

        public MvxCommand LostPasswordCommand {
            get {
                if (mLostPasswordCommand == null) {
                    mLostPasswordCommand = new MvxCommand (this.LostPassword);
                }
                return mLostPasswordCommand;
            }
        }

        private void LostPassword ()
        {
            ShowViewModel<LostPasswordViewModel>();
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

