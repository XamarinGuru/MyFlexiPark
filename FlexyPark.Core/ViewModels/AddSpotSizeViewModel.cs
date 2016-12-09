using System;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public class AddSpotSizeViewModel : BaseViewModel
    {
        #region Constructors

        public AddSpotSizeViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init(string type)
        {
            VehicleTypes.Add(new VehicleTypeItemViewModel()
                {
                    Type = "compact"
                });
            VehicleTypes.Add(new VehicleTypeItemViewModel()
                {
                    Type = "sedan"
                });
            if (!string.IsNullOrEmpty(type))
            {
                foreach (var item in VehicleTypes)
                {
                    if (item.Type.Equals(type))
                        item.IsChecked = true;
                }
            }
        }

        #endregion

        #region Properties

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

        #endregion

        #region Commands

        #region DoneCommand

        private MvxCommand mDoneCommand = null;

        public MvxCommand DoneCommand
        {
            get
            {
                if (mDoneCommand == null)
                {
                    mDoneCommand = new MvxCommand(this.Done);
                }
                return mDoneCommand;
            }
        }

        private void Done()
        {
            var isChecked = VehicleTypes.FirstOrDefault(x => x.IsChecked == true);
            if (isChecked != null)
            {
                mCacheService.NextStatus = AddSpotStatus.SpotCost;
                mCacheService.CreateParkingRequest.CarType = isChecked.Type;
                Close(this);
            }
            else
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, Mvx.Resolve<ICacheService>().TextSource.GetText("PleaseChooseSpotSizeText")));
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

            //get checked size
            var isChecked = VehicleTypes.FirstOrDefault(x => x.IsChecked == true);

            mCacheService.CreateParkingRequest.CarType = isChecked.Type;
            mCacheService.NextStatus = AddSpotStatus.SpotCost;
            Close(this);
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

