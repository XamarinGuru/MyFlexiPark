using System;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using Cirrious.CrossCore.Platform;
using System.Linq;
using FlexyPark.Core.Helpers;
using Akavache;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;

namespace FlexyPark.Core.ViewModels
{
    public class MyReservationsViewModel : BaseViewModel
    {
        #region Constructors

        public MyReservationsViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init()
        {
            await Task.Delay(100);

            //GetMyReservations()
        }

        #endregion

        #region Properties

        #region Reservations

        private ObservableCollection<ReservationItemViewModel> mReservations = new ObservableCollection<ReservationItemViewModel>();

        public ObservableCollection<ReservationItemViewModel> Reservations
        {
            get
            {
                return mReservations;
            }
            set
            {
                mReservations = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoParkingReservedCommand

        private MvxCommand<ReservationItemViewModel> mGotoParkingReservedCommand = null;

        public MvxCommand<ReservationItemViewModel> GotoParkingReservedCommand
        {
            get
            {
                if (mGotoParkingReservedCommand == null)
                {
                    mGotoParkingReservedCommand = new MvxCommand<ReservationItemViewModel>(this.GotoParkingReserved);
                }
                return mGotoParkingReservedCommand;
            }
        }

        private void GotoParkingReserved(ReservationItemViewModel itemVM)
        {
            mCacheService.CurrentReservation = itemVM.Reservation;
            ShowViewModel<ParkingReservedViewModel>(new { status = ParkingStatus.Reserved});
        }

        #endregion

        #region GotoParkingSearchCommand

        private MvxCommand mGotoParkingSearchCommand = null;

        public MvxCommand GotoParkingSearchCommand
        {
            get
            {
                if (mGotoParkingSearchCommand == null)
                {
                    mGotoParkingSearchCommand = new MvxCommand(this.GotoParkingSearch);
                }
                return mGotoParkingSearchCommand;
            }
        }

        private async void GotoParkingSearch()
        {
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

        public async void DeleteReservation()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                /*var result = await mApiService.DeleteVehicle("userId", "vehicleId");
                if(result != null)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                }*/
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }


        public async void GetMyReservations()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var results = await mApiService.RenterBookings(Mvx.Resolve<ICacheService>().CurrentUser.UserId, DateTime.UtcNow.ToLocalTime().DateTimeToTimeStamp(), "sorting", 20, 0);
                Reservations.Clear();
                if (results != null && results.Response.Count != 0)
                {
                    foreach (var item in results.Response)
                    {
                        Reservations.Add(new ReservationItemViewModel(this)
                            {
                                Reservation = item,   
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
                catch(KeyNotFoundException e)
                {
                    Debug.WriteLine(e.Message);
                }

                //if there is no cached vehicles, then call api to get
                if(cachedVehicles == null)
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

        #endregion
    }
}

