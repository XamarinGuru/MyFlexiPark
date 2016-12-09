using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Localization;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Akavache;
using System.Reactive.Linq;
using FlexyPark.Core.Models;

namespace FlexyPark.Core.ViewModels
{
    public interface IMenuView
    {
        void ShowParkMePopup();

        void ClosePopup();
    }

    public class MenuViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public MenuViewModel(IApiService apiService, ICacheService cacheService, IPlatformService platformService)
            : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
        }

        #endregion

        #region Init

        public async void Init(double credits)
        {
            double remainCredits = 0;

            User currentUser = null;
            if (mPlatformService.IsLogin())
            {
                //trying to get currentuser from local database
                try
                {
                    currentUser = await BlobCache.UserAccount.GetObject<User>("CurrentUser");
                    mCacheService.CurrentUser = currentUser;

                    if(credits == -1)
                    {
                        var result = await mApiService.PersonGet(mCacheService.CurrentUser.UserId);
                        if (result != null)
                        {
                            mCacheService.CurrentUser.RemainingCredits = result.Response.RemainingCredits;
                            if (Double.TryParse(mCacheService.CurrentUser.RemainingCredits, out remainCredits))
                            {
                                Credits = remainCredits;
                            }
                            else
                            {
                                Debug.WriteLine("Error when Parse from String to Double (Remaining Credits)");
                            }
                        }
                    }
                    else
                    {
                        if (Double.TryParse(mCacheService.CurrentUser.RemainingCredits, out remainCredits))
                        {
                            Credits = remainCredits;
                        }
                        else
                        {
                            Debug.WriteLine("Error when Parse from String to Double (Remaining Credits)");
                        }
                    }
                }
                catch (KeyNotFoundException e)
                {
                    Debug.WriteLine(e.Message);
                }


            }
//            else
//            {
//                if (mCacheService.CurrentUser != null && mCacheService.CurrentUser.RemainingCredits != "na")
//                {
//                    double remainCredits = 0;
//                    if (Double.TryParse(mCacheService.CurrentUser.RemainingCredits, out remainCredits))
//                    {
//                        Credits = remainCredits;
//                    }
//                    else
//                    {
//                        Debug.WriteLine("Error when Parse from String to Int (Remaining Credits)");
//                    }
//                }
//            }

            GetVehiclesCount();
        }

        #endregion

        public IMenuView View { get; set; }

        #region Properties

        #region Credits

        private double mCredits = -1;

        public double Credits
        {
            get
            {
                return mCredits;
            }
            set
            {
                mCredits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoSettingsCommand

        private MvxCommand mGotoSettingsCommand = null;

        public MvxCommand GotoSettingsCommand
        {
            get
            {
                if (mGotoSettingsCommand == null)
                {
                    mGotoSettingsCommand = new MvxCommand(this.GotoSettings);
                }
                return mGotoSettingsCommand;
            }
        }

        private void GotoSettings()
        {
            ShowViewModel<SettingsViewModel>();
        }

        #endregion

        #region GotoParkMeCommand

        private MvxCommand mGotoParkMeCommand = null;

        public MvxCommand GotoParkMeCommand
        {
            get
            {
                if (mGotoParkMeCommand == null)
                {
                    mGotoParkMeCommand = new MvxCommand(this.GotoParkMe);
                }
                return mGotoParkMeCommand;
            }
        }

        private void GotoParkMe()
        {
            //ShowViewModel<ParkMeViewModel>();

            if (View != null)
                View.ShowParkMePopup();
        }

        #endregion

        #region GotoMyReservationsCommand

        private MvxCommand mGotoMyReservationsCommand = null;

        public MvxCommand GotoMyReservationsCommand
        {
            get
            {
                if (mGotoMyReservationsCommand == null)
                {
                    mGotoMyReservationsCommand = new MvxCommand(this.GotoMyReservations);
                }
                return mGotoMyReservationsCommand;
            }
        }

        private void GotoMyReservations()
        {
            ShowViewModel<MyReservationsViewModel>();
        }

        #endregion

        #region GotoMyOwnParkingCommand

        private MvxCommand mGotoMyOwnParkingCommand = null;

        public MvxCommand GotoMyOwnParkingCommand
        {
            get
            {
                if (mGotoMyOwnParkingCommand == null)
                {
                    mGotoMyOwnParkingCommand = new MvxCommand(this.GotoMyOwnParking);
                }
                return mGotoMyOwnParkingCommand;
            }
        }

        private void GotoMyOwnParking()
        {
            ShowViewModel<MyOwnParkingViewModel>();
            // ShowViewModel<MyProfileViewModel>(new { tabIndex = MyProfileTab.Own, isSignUp = false});
        }

        #endregion

        #region GotoCreditsCommand

        private MvxCommand mGotoCreditsCommand = null;

        public MvxCommand GotoCreditsCommand
        {
            get
            {
                if (mGotoCreditsCommand == null)
                {
                    mGotoCreditsCommand = new MvxCommand(this.GotoCredits);
                }
                return mGotoCreditsCommand;
            }
        }

        private void GotoCredits()
        {
            //ShowViewModel<BuyCreditsViewModel>();
            ShowViewModel<RentProfileViewModel> ();
        }

        #endregion

        #region ParkMeNowCommand

        private MvxCommand mParkMeNowCommand = null;

        public MvxCommand ParkMeNowCommand
        {
            get
            {
                if (mParkMeNowCommand == null)
                {
                    mParkMeNowCommand = new MvxCommand(this.ParkMeNow);
                }
                return mParkMeNowCommand;
            }
        }

        public async void ParkMeNow()
        {
            if (View != null)
                View.ClosePopup();

            var vehiclesCount = await GetVehiclesCount();
            if (vehiclesCount < 0)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Error get vehicles count"));
                return;
            }

            mCacheService.SearchMode = SearchMode.Now;
            if (vehiclesCount == 1)
            {
                mCacheService.SelectedVehicle = mCacheService.UserVehicles.FirstOrDefault();
                ShowViewModel<ParkingListsViewModel>();
            }
            else if (vehiclesCount == 0)
            {
                ShowViewModel<ChooseVehicleViewModel>(new {mode = ChooseVehicleMode.FirstSelect});
                await Task.Delay(200);
                ShowViewModel<AddVehicleViewModel>(new {isEditMode = false});
            }
            else
                ShowViewModel<ChooseVehicleViewModel>(new {mode = ChooseVehicleMode.FirstSelect});
        }

        #endregion

        #region ParkMeLaterCommand

        private MvxCommand mParkMeLaterCommand = null;

        public MvxCommand ParkMeLaterCommand
        {
            get
            {
                if (mParkMeLaterCommand == null)
                {
                    mParkMeLaterCommand = new MvxCommand(this.ParkMeLater);
                }
                return mParkMeLaterCommand;
            }
        }

        public async void ParkMeLater()
        {
            if (View != null)
                View.ClosePopup();

            var vehiclesCount = await GetVehiclesCount();
            if (vehiclesCount < 0)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Error get vehicles count"));
                return;
            }

            mCacheService.SearchMode = SearchMode.Later;

            if (vehiclesCount == 1)
            {
                mCacheService.SelectedVehicle = mCacheService.UserVehicles.FirstOrDefault();
                ShowViewModel<ParkingSearchViewModel>(new {mode = SearchMode.Later});
            }
            else if (vehiclesCount == 0)
            {
                ShowViewModel<ChooseVehicleViewModel>(new {mode = ChooseVehicleMode.FirstSelect});
                await Task.Delay(200);
                ShowViewModel<AddVehicleViewModel>(new {isEditMode = false});
            }
            else
                ShowViewModel<ChooseVehicleViewModel>(new {mode = ChooseVehicleMode.FirstSelect});
        }

        #endregion

        #endregion

        #region Methods

        private async Task<int> GetVehiclesCount()
        {
            int toReturn = -1;

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

            //if the vehicles list has already been loaded
            if (mCacheService.UserVehicles != null)
            {
                toReturn = mCacheService.UserVehicles.Count;
            }
            else
            {
                //trying to get vehicles from local database
                List<Vehicle> cachedVehicles = null;
                try
                {
                    cachedVehicles = await BlobCache.UserAccount.GetObject<List<Vehicle>>(mCacheService.CurrentUser.UserId);
                }
                catch (KeyNotFoundException e)
                {
                    Debug.WriteLine(e.Message);
                }

                //if there is no cached vehicles, then call api to get
                if (cachedVehicles == null)
                {
                    if (BaseView != null && BaseView.CheckInternetConnection())
                    {
                        var vehicles = await mApiService.GetVehicles((mCacheService.CurrentUser != null && !string.IsNullOrEmpty(mCacheService.CurrentUser.UserId)) ? mCacheService.CurrentUser.UserId : string.Empty);

                        if (vehicles != null)
                        {
                            //cache all vehicles, to prevent reload the next attempt
                            mCacheService.UserVehicles = vehicles.Response;
                            toReturn = mCacheService.UserVehicles.Count;
                            //save to local database
                            BlobCache.UserAccount.InsertObject<List<Vehicle>>(mCacheService.CurrentUser.UserId, mCacheService.UserVehicles);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                    }
                }
                else
                {
                    mCacheService.UserVehicles = cachedVehicles;
                    toReturn = mCacheService.UserVehicles.Count;
                }
            }

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            return toReturn;
        }


        public void UpdateRemainCredit()
        {
            if (mCacheService.CurrentUser != null && mCacheService.CurrentUser.RemainingCredits != "na")
            {
                double remainCredits = 0;
                if (Double.TryParse(mCacheService.CurrentUser.RemainingCredits, out remainCredits))
                {
                    Credits = remainCredits;
                }
                else
                {
                    Debug.WriteLine("Error when Parse from String to Int (Remaining Credits)");
                }
            }
        }

        #endregion
    }
}

