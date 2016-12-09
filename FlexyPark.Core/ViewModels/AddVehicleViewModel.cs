using System;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using System.Linq;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using System.Diagnostics;
using Akavache;
using System.Collections.Generic;

namespace FlexyPark.Core.ViewModels
{
    public class AddVehicleViewModel : BaseViewModel
    {
        #region Constructors

        public AddVehicleViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init(bool isEditMode, string vehicleId, string plateNumber, string type)
        {
            IsEditMode = isEditMode;
            Title = isEditMode ? "Edit Event" : "Add Event";
            ButtonTitle = isEditMode ? "Edit" : "Add";

            //demo data
            VehicleTypes.Add(new VehicleTypeItemViewModel()
                {
                    Type = "compact",
                });
            VehicleTypes.Add(new VehicleTypeItemViewModel()
                {
                    Type = "sedan"
                });

            PlateNumber = !string.IsNullOrEmpty(plateNumber) ? plateNumber : string.Empty;
            if (!string.IsNullOrEmpty(type))
            {
                var vehicleType = VehicleTypes.FirstOrDefault(x => x.Type.Equals(type));
                if (vehicleType != null)
                    vehicleType.IsChecked = true;
            }

            VehicleId = vehicleId;
        }

        #endregion

        #region Properties

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

        #region Title

        private string mTitle = string.Empty;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ButtonTitle

        private string mButtonTitle = string.Empty;

        public string ButtonTitle
        {
            get
            {
                return mButtonTitle;
            }
            set
            {
                mButtonTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PlateNumber

        private string mPlateNumber = string.Empty;

        public string PlateNumber
        {
            get
            {
                return mPlateNumber;
            }
            set
            {
                mPlateNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region VehicleTypes

        private ObservableCollection<VehicleTypeItemViewModel> mVehicleTypes = new ObservableCollection<VehicleTypeItemViewModel>();

        public ObservableCollection<VehicleTypeItemViewModel> VehicleTypes
        {
            get
            {
                return mVehicleTypes;
            }
            set
            {
                mVehicleTypes = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region VehicleId

        private string mVehicleId = string.Empty;

        public string VehicleId
        {
            get
            {
                return mVehicleId;
            }
            set
            {
                mVehicleId = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region AddNewVehicleCommand

        private MvxCommand mAddNewVehicleCommand = null;

        public MvxCommand AddNewVehicleCommand
        {
            get
            {
                if (mAddNewVehicleCommand == null)
                {
                    mAddNewVehicleCommand = new MvxCommand(this.AddNewVehicle);
                }
                return mAddNewVehicleCommand;
            }
        }

        private async void AddNewVehicle()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                var isChecked = VehicleTypes.FirstOrDefault(x => x.IsChecked == true);
                if (isChecked != null && !string.IsNullOrEmpty(PlateNumber))
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                    if (IsEditMode)
                    {
                        var result = await mApiService.PutVehicle(mCacheService.CurrentUser.UserId, VehicleId, isChecked.Type, PlateNumber);
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

                            //update user vehicles that has been cached
                            if (result.Response.Status.Equals("success"))
                            {
                                var vehicle = mCacheService.UserVehicles.FirstOrDefault(x => x.VehicleId.Equals(VehicleId));
                                if (vehicle != null)
                                {
                                    vehicle.Type = isChecked.Type;
                                    vehicle.PlateNumber = PlateNumber;
                                }
                                Mvx.Resolve<IMvxMessenger>().Publish(new UpdateSuccessMessage(this));
                                //save to local database
                                BlobCache.UserAccount.InsertObject<List<Vehicle>>(mCacheService.CurrentUser.UserId, mCacheService.UserVehicles);
                            }

                            Close(this);
                        }
                    }
                    else
                    {
                        var result = await mApiService.CreateVehicle(mCacheService.CurrentUser.UserId, isChecked.Type, PlateNumber);
                        if (result != null)
                        {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result + "\n" + result.VehicleId));
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
							if (result.Response.Status.Equals("success"))
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                            }
                            else
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                            }
                            if (result.Response.Status.Equals("success"))
                            {
                                var vehicle = new Vehicle()
                                {
                                    VehicleId = result.Response.VehicleId,
                                    Type = isChecked.Type,
                                    PlateNumber = PlateNumber
                                };
                                if (!mCacheService.UserVehicles.Contains(vehicle))
                                    mCacheService.UserVehicles.Add(vehicle);
                                Mvx.Resolve<IMvxMessenger>().Publish(new UpdateSuccessMessage(this));
                                //save to local database
                                BlobCache.UserAccount.InsertObject<List<Vehicle>>(mCacheService.CurrentUser.UserId, mCacheService.UserVehicles);
                            }

                            Close(this);
                        }  
                    }


                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                }
                else
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, isChecked != null ? mCacheService.TextSource.GetText("PleaseEnterPlateNumberText") : mCacheService.TextSource.GetText("PleaseChooseVehicleTypeText")));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion

        #region VehicleItemClickCommand

        private MvxCommand<VehicleTypeItemViewModel> mVehicleItemClickCommand;

        public MvxCommand<VehicleTypeItemViewModel> VehicleItemClickCommand
        {
            get
            {
                return mVehicleItemClickCommand ??
                (mVehicleItemClickCommand = new MvxCommand<VehicleTypeItemViewModel>(VehicleItemClick));
            }
        }

        private void VehicleItemClick(VehicleTypeItemViewModel item)
        {
            foreach (var vehicleItemViewModel in VehicleTypes)
            {
                vehicleItemViewModel.IsChecked = false;
            }

            item.IsChecked = true;

            RaisePropertyChanged("VehicleTypes");
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

