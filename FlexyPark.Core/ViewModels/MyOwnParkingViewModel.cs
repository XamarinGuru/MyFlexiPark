using System;
using FlexyPark.Core.Services;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using Cirrious.CrossCore.Platform;

namespace FlexyPark.Core.ViewModels
{
    public interface IMyOwnParkingView
    {
        void SetModeTitle(string title);
    }

    public class MyOwnParkingViewModel : BaseViewModel
    {
        #region Constructors

        public MyOwnParkingViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init()
        {
            await Task.Delay(100);


            //GetMyOwnParking();
        }

        #endregion

        public IMyOwnParkingView View { get; set; }

        #region Properties

        #region Parkings

        private ObservableCollection<OwnerParkingItemViewModel> mParkings = new ObservableCollection<OwnerParkingItemViewModel>();

        public ObservableCollection<OwnerParkingItemViewModel> Parkings
        {
            get
            {
                return mParkings;
            }
            set
            {
                mParkings = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsEditMode

        private bool mIsEditMode = false;

        public bool IsEditMode
        {
            get
            {
                return mIsEditMode;
            }
            set
            {
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region SwitchModeCommand

        private MvxCommand mSwitchModeCommand = null;

        public MvxCommand SwitchModeCommand
        {
            get
            {
                if (mSwitchModeCommand == null)
                {
                    mSwitchModeCommand = new MvxCommand(this.SwitchMode);
                }
                return mSwitchModeCommand;
            }
        }

        private void SwitchMode()
        {
            IsEditMode = !IsEditMode;
            foreach (var parking in Parkings)
            {
                parking.IsEditMode = IsEditMode;
            }

            if (View != null)
                //View.SetModeTitle(IsEditMode ? "Cancel" : "Edit");
                View.SetModeTitle(IsEditMode ? mCacheService.TextSource.GetText("CancelText") : mCacheService.TextSource.GetText("DeleteText"));
        }

        #endregion

        #region GotoAddSpotCommand

        private MvxCommand mGotoAddSpotCommand = null;

        public MvxCommand GotoAddSpotCommand
        {
            get
            {
                if (mGotoAddSpotCommand == null)
                {
                    mGotoAddSpotCommand = new MvxCommand(this.GotoAddSpot);
                }
                return mGotoAddSpotCommand;
            }
        }

        private void GotoAddSpot()
        {
            mCacheService.SpotAddress = string.Empty;
            mCacheService.NextStatus = AddSpotStatus.GotoSpot;
            ShowViewModel<AddSpotViewModel>(new {isEditMode = false});
        }

        #endregion

        #region OwnParkingItemSelectedCommand

        private MvxCommand<OwnerParkingItemViewModel> mOwnParkingItemSelectedCommand = null;

        public MvxCommand<OwnerParkingItemViewModel> OwnParkingItemSelectedCommand
        {
            get
            {
                if (mOwnParkingItemSelectedCommand == null)
                {
                    mOwnParkingItemSelectedCommand = new MvxCommand<OwnerParkingItemViewModel>(this.OwnParkingItemSelected);
                }
                return mOwnParkingItemSelectedCommand;
            }
        }

        private void OwnParkingItemSelected(OwnerParkingItemViewModel itemVM)
        {
            mCacheService.SpotAddress = string.Empty;
            mCacheService.NextStatus = AddSpotStatus.GotoSpot;
            var parameterObject = Mvx.Resolve<IMvxJsonConverter>().SerializeObject(itemVM.Parking);
            ShowViewModel<AddSpotViewModel>(new {isEditMode = true, parameterObject = parameterObject});
        }

        #endregion

        #endregion

        #region Methods

        public async void GetMyOwnParking()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var results = await mApiService.OwnerParkings(Mvx.Resolve<ICacheService>().CurrentUser.UserId);
                if (results != null && results.Response.Count != 0)
                {
                    Parkings.Clear();
                    foreach (var item in results.Response)
                    {
                        Parkings.Add(new OwnerParkingItemViewModel(mCacheService, this)
                            {
                                Parking = item,   
                            });
                    }
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        public async void DeleteParkingSpot()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                //var result = await mApiService.
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion
    }
}

