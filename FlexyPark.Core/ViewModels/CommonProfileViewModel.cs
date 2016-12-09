using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Models;
using Akavache;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Globalization;

namespace FlexyPark.Core.ViewModels
{
    public interface ICommonProfileView
    {
        string GetDeviceIdentifier();
        void OpenURL (string url);
		void CloseKeyboard();
    }

    public class CommonProfileViewModel : BaseViewModel
    {
        #region Constructors

        public CommonProfileViewModel( IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
            
        }

        #endregion

        #region Init

        public void Init(bool isSignUp)
        {
            IsShowInfo = !isSignUp;
        }

        #endregion

        public ICommonProfileView View { get; set; }

        #region Properties

        #region User

        /// <summary>
        /// Temp user information.
        /// </summary>
        private User mUser = new User();

        public User User
        {
            get
            {
                return mUser;
            }
            set
            {
                mUser = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsAcceptTerms

        private bool mIsAcceptTerms = false;

        public bool IsAcceptTerms
        {
            get
            {
                return mIsAcceptTerms;
            }
            set
            {
                mIsAcceptTerms = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowInfo

        private bool mIsShowInfo = false;

        public bool IsShowInfo
        {
            get
            {
                return mIsShowInfo;
            }
            set
            {
                mIsShowInfo = value;
                RaisePropertyChanged();
            }
        }

        #endregion

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

        #region MobileNumber

        private string mMobileNumber = string.Empty;

        public string MobileNumber
        {
            get
            {
                return mMobileNumber;
            }
            set
            {
                mMobileNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        //#region PromoCode

        //private string mPromoCode = string.Empty;

        //public string PromoCode
        //{
        //    get
        //    {
        //        return mPromoCode;
        //    }
        //    set
        //    {
        //        mPromoCode = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //#endregion

        #endregion

        #region Commands

        #region SignUpOrUpdateCommand

        private MvxCommand mSignUpOrUpdateCommand = null;

        public MvxCommand SignUpOrUpdateCommand
        {
            get
            {
                if (mSignUpOrUpdateCommand == null)
                {
                    mSignUpOrUpdateCommand = new MvxCommand(this.SignUpOrUpdate);
                }
                return mSignUpOrUpdateCommand;
            }
        }

        private async void SignUpOrUpdate()
        {
			if (View != null)
				View.CloseKeyboard();
			
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, true));

                if (IsShowInfo)//update
                {
					var result = await mApiService.PersonPut (mCacheService.CurrentUser.UserId, User.FirstName, User.LastName, User.Email, Password, User.PhoneNumber, User.FacebookId, User.Street, User.PostalCode, User.City, User.Country, User.DateOfBirth);
                    if (result != null) {
                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                        if (result.Response.Status.Equals ("success")) {
                            Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, result.Response.Status));
                        } else {
                            Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                        }

                    }
                    Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                    Close (this);
                } else {
                    if (IsAcceptTerms) {

						if (string.IsNullOrEmpty(Password))
						{
							Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Please fill a password"));
							Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));		
							return;
						}

                        //sign up
                        var deviceIdentifier = View.GetDeviceIdentifier ();
                        var result = await mApiService.PersonRegister (User.Email, Password, User.FirstName, User.LastName, User.PhoneNumber,
                            Mvx.Resolve<IPlatformService> ().OS == OS.Touch ? AppConstants.iOSToken : AppConstants.AndroidToken, deviceIdentifier);
                        if (result != null) {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? string.Format("{0}: {1}", result.Response.Result, result.Response.PersonId) : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                            if (result.Response != null && result.Response.Status.Equals ("success")) {
                                Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, result.Response.Status));
                            } else {
                                if (result.ApiError != null && result.ApiError.Status.Equals ("777")) {
                                    Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, "Token is incorrect, please contact app@myflexipark.com", "Ok", null));

                                } else {
                                    Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                                }
                                Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                                return;
                            }
                            var result2 = await mApiService.Login (User.Email, Password,
                                Mvx.Resolve<IPlatformService> ().OS == OS.Touch ? AppConstants.iOSToken : AppConstants.AndroidToken);
                            if (result2 != null) {
                                mCacheService.SessionId = result2.SessionId;
                                mCacheService.CurrentUser = new User ();
                                mCacheService.CurrentUser.UserId = result2.UserId;

                                //then get user information
                                var result3 = await mApiService.PersonGet (mCacheService.CurrentUser.UserId);
                                if (result3 != null) {
                                    mCacheService.CurrentUser = result3.Response;
                                    mCacheService.CurrentUser.UserId = result3.Request.UserId;
                                }

                                //then update user information (F1.3) 
								var result4 = await mApiService.PersonPut (mCacheService.CurrentUser.UserId, User.FirstName, User.LastName, User.Email, Password, User.PhoneNumber, User.FacebookId, User.Street, User.PostalCode, User.City, User.Country, User.DateOfBirth);
                                if (result4 != null) {
                                    if (result4.Response.Status.Equals ("success")) {
                                        Debug.WriteLine ("Update person information : " + result4.Response.Status);
                                    } else {
                                        Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}: {1}", result4.Response.Status, result4.Response.ErrorCode), "Ok", null));
                                    }
                                }

                                ShowViewModel<MenuViewModel> ();
                            }

                        } else {
                            //Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, "This email is already registered. Please login or ask for a new password at info@myflexypark.com"));
							Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Please check the email."));
                        }
                        Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                    } else {
                        Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this, SharedTextSource.GetText ("AcceptTermsText")));
                    }
                    Mvx.Resolve<IMvxMessenger> ().Publish (new ProgressMessage (this, false));
                }
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion

        #region ReadTermsOfUseCommand

        private MvxCommand mReadTermsOfUseCommand;

        public MvxCommand ReadTermsOfUseCommand {
            get {
                if (mReadTermsOfUseCommand == null) {
                    mReadTermsOfUseCommand = new MvxCommand (this.ReadTermsOfUse);
                }
                return mReadTermsOfUseCommand;
            }
        }

        private void ReadTermsOfUse ()
		{
			if (View != null)
			{
				View.CloseKeyboard();
				View.OpenURL(Mvx.Resolve<IPlatformService>().OS == OS.Touch ? AppConstants.iOSTermsOfUse : AppConstants.ADTermsOfUse);
			}
        }

        #endregion

        #region LogoutCommand

        private MvxCommand mLogoutCommand;

        public MvxCommand LogoutCommand
        {
            get
            {
                if (mLogoutCommand == null)
                {
                    mLogoutCommand = new MvxCommand(this.Logout);
                }
                return mLogoutCommand;
            }
        }

        private void Logout()
        {
			ClearStackAndShowViewModel<InitialMapViewModel>();
            BlobCache.UserAccount.InvalidateObject<List<Vehicle>>(mCacheService.CurrentUser.UserId);
            BlobCache.UserAccount.InvalidateObject<User>("CurrentUser");
			BlobCache.UserAccount.InvalidateObject<string>("SessionId");
            mCacheService.CurrentUser = null;
			mCacheService.SessionId = null;
            Mvx.Resolve<IPlatformService>().SetPreference(AppConstants.IsLogin, false);
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

