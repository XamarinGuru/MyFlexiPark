using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using FlexyPark.Core.Services;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Cirrious.CrossCore.Platform;
using Akavache;
using System.Collections.Generic;

namespace FlexyPark.Core.ViewModels
{
    public interface IChooseVehicleView
    {
        void SetModeTitle(string title);
    }

    public class ChooseVehicleViewModel : BaseViewModel
    {
        #region Constructors

        public ChooseVehicleViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init(ChooseVehicleMode mode)
        {
            await Task.Delay(100);

            Mode = mode;

            //GetVehicles (iOS ViewWillAppear - AD OnResume)
        }

        #endregion

        public IChooseVehicleView View { get; set; }

        #region Properties

        #region Mode

        private ChooseVehicleMode mMode = ChooseVehicleMode.FirstSelect;

        public ChooseVehicleMode Mode
        {
            get
            {
                return mMode;
            }
            set
            {
                mMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Vehicles

        private ObservableCollection<VehicleItemViewModel> mVehicles = new ObservableCollection<VehicleItemViewModel>();

        public ObservableCollection<VehicleItemViewModel> Vehicles
        {
            get
            {
                return mVehicles;
            }
            set
            {
                mVehicles = value;
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

        #region GotoAddVehicleCommand

        private MvxCommand mGotoAddVehicleCommand = null;

        public MvxCommand GotoAddVehicleCommand
        {
            get
            {
                if (mGotoAddVehicleCommand == null)
                {
                    mGotoAddVehicleCommand = new MvxCommand(this.GotoAddVehicle);
                }
                return mGotoAddVehicleCommand;
            }
        }

        private void GotoAddVehicle()
        {
            ShowViewModel<AddVehicleViewModel>(new {isEditMode = false});
        }

        #endregion

        #region VehicleItemClickCommand

        private MvxCommand<VehicleItemViewModel> mVehicleItemClickCommand;

        public MvxCommand<VehicleItemViewModel> VehicleItemClickCommand
        {
            get
            {
                return mVehicleItemClickCommand ??
                (mVehicleItemClickCommand = new MvxCommand<VehicleItemViewModel>(VehicleItemClick));
            }
        }

        private void VehicleItemClick(VehicleItemViewModel item)
        {
            /*item.Checked = true;

            foreach (var vehicleItemViewModel in Vehicles)
            {
                if(vehicleItemViewModel.Equals(item))
                    continue;

                vehicleItemViewModel.Checked = false;
            }*/

            if (Mode == ChooseVehicleMode.NoAction)
                return;

            if (Mode == ChooseVehicleMode.ReSelect)
                Close(this);
            else
            {
                mCacheService.SelectedVehicle = item.Vehicle;

                if (mCacheService.SearchMode == SearchMode.Later)
                    ShowViewModel<ParkingSearchViewModel>(new {mode = mCacheService.SearchMode});
                else
                    ShowViewModel<ParkingListsViewModel>();
            }
        }

        #endregion

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
            if (!IsEditMode)
            {
                IsEditMode = !IsEditMode;
                foreach (var vehicle in Vehicles)
                {
                    vehicle.IsEditMode = IsEditMode;
                }

                if (View != null)
                    View.SetModeTitle(IsEditMode ? mCacheService.TextSource.GetText("CancelTitle") : mCacheService.TextSource.GetText("EditTitle"));
            }
        }



        #endregion

        #endregion

        #region Methods

        private void SwitchToDefaultMode()
        {
            foreach (var vehicle in Vehicles)
            {
                vehicle.IsEditMode = false;
            }

            if (View != null)
                View.SetModeTitle(mCacheService.TextSource.GetText("EditTitle"));
        }

        public async void GetVehicles()
        {
            IsEditMode = false;
            SwitchToDefaultMode();

            //load user vehicles from cache
            if (mCacheService.UserVehicles != null)
            {                                
                Vehicles.Clear();
                foreach (var vehicle in mCacheService.UserVehicles)
                {
                    Vehicles.Add(new VehicleItemViewModel(this){ Vehicle = vehicle });
                }

                return;
            }

            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                var vehicles = await mApiService.GetVehicles(mCacheService.CurrentUser.UserId);

                if (vehicles != null && vehicles.Response.Count > 0)
                {
                    Vehicles.Clear();
                    foreach (var vehicle in vehicles.Response)
                    {
                        Vehicles.Add(new VehicleItemViewModel(this){ Vehicle = vehicle });
                    }

                    //cache all vehicles, to prevent reload the next attempt
                    mCacheService.UserVehicles = vehicles.Response;
                }

                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        public async void DeleteVehicle(string vehicleId)
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var result = await mApiService.DeleteVehicle(mCacheService.CurrentUser.UserId, vehicleId);
                if (result)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result ? "Success" : "Error"));
                    if (result)
                    {

                        var vehicle = mCacheService.UserVehicles.FirstOrDefault(x => x.VehicleId.Equals(vehicleId));
                        if (vehicle != null)
                        {
                            mCacheService.UserVehicles.Remove(vehicle);
                            //save to local database
                            BlobCache.UserAccount.InsertObject<List<Vehicle>>(mCacheService.CurrentUser.UserId, mCacheService.UserVehicles);
                        }

                        var vehicleVM = Vehicles.FirstOrDefault(x => x.Vehicle.VehicleId == vehicleId);
                        if (vehicle != null)
                            Vehicles.Remove(vehicleVM);
                    }
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

