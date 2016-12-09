using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Helpers;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Collections.Generic;

namespace FlexyPark.Core.ViewModels
{
    public class LeaveParkingViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public LeaveParkingViewModel(IApiService apiService, ICacheService cacheService, IPlatformService platformService)
            : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
        }

        #endregion

        #region Init

        public void Init()
        {
            IsLikedThisSpot = true;
        }

        #endregion

        #region Properties

        #region IsLikedThisSpot

        private bool mIsLikedThisSpot = true;

        public bool IsLikedThisSpot
        {
            get
            {
                return mIsLikedThisSpot;
            }
            set
            {
                mIsLikedThisSpot = value;
                if (mIsLikedThisSpot)
                    Rating = "1/1";
                else
                    Rating = "0/1";
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Rating

        public string Rating { get; set; }

        #endregion

        #region Comment

        private string mComment = string.Empty;

        public string Comment
        {
            get
            {
                return mComment;
            }
            set
            {
                mComment = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GoToMenuCommand

        private MvxCommand mGoToMenuCommand = null;

        public MvxCommand GoToMenuCommand
        {
            get
            {
                if (mGoToMenuCommand == null)
                {
                    mGoToMenuCommand = new MvxCommand(this.GoToMenu);
                }
                return mGoToMenuCommand;
            }
        }

        private async void GoToMenu()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("WarningText"), SharedTextSource.GetText("LeaveThisParkingText"), SharedTextSource.GetText("NoText"), null, new string[] { SharedTextSource.GetText("YesText") }, 
                    () =>
                    {
                        LeaveParkingAndGotoMenu();
                    mPlatformService.SetPreference<long>(AppConstants.BookingExpiredTime, 0);
                    mPlatformService.SetPreference<long>(AppConstants.CurrentBookingId, 0);
                        /*if (BaseView != null && BaseView.CheckInternetConnection())
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                            var result = await mApiService.LeaveBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, mCacheService.CurrentReservation.Parking.ParkingId, mCacheService.CurrentReservation.BookingId);
                            if (result != null)
                            {
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));

                                ShowViewModel<MenuViewModel>(presentationFlag: PresentationBundleFlagKeys.Menu);
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                        }
                        else
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                        }*/
                    }
                ));
        }

        #endregion

        #endregion

        #region Methods

        private async void LeaveParkingAndGotoMenu()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
               
                var result = await mApiService.LeaveBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, mCacheService.CurrentReservation.Parking.ParkingId, mCacheService.CurrentReservation.BookingId, Rating, Comment, "left");
                if (result != null)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                    if (result.Response.Status.Equals("success"))
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                    }

                    ShowViewModel<MenuViewModel>(presentationFlag: PresentationBundleFlagKeys.Menu);
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion
    }
}

