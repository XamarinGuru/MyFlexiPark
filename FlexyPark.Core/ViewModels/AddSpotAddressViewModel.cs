using System;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.ViewModels;
using System.Text.RegularExpressions;
using FlexyPark.Core.Messengers;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;

namespace FlexyPark.Core.ViewModels
{
    public class AddSpotAddressViewModel : BaseViewModel
    {
        private readonly ICacheService mCacheService;
        private readonly IApiService mApiService;

        #region Constructors

        public AddSpotAddressViewModel(ICacheService cacheService, IApiService apiService)
            : base(apiService, cacheService)
        {
            this.mCacheService = cacheService;
            this.mApiService = apiService;
        }

        #endregion

        #region Init

        public async void Init(string location)
        {
            await Task.Delay(100);

            //if user has edited parking spot address
            if (!string.IsNullOrEmpty(mCacheService.SpotAddress))
            {
                Address = mCacheService.SpotAddress;
                return;
            }

            //if we are editing a parking spot
            if (!string.IsNullOrEmpty(location))
            {
                Address = location;
                return;
            }

            //if we are adding a new parking spot
            if (mCacheService.CurrentLat != 0 && mCacheService.CurrentLng != 0)
            {
                if (BaseView != null && BaseView.CheckInternetConnection())
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                    var result = await mApiService.AddressOf(mCacheService.CurrentLat, mCacheService.CurrentLng);
                    if (result != null)
                    {
                        Address = result.Address;
                    }

                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                }
            }
        }

        #endregion

        #region Properties

        #region Address

        private string mAddress = string.Empty;

        public string Address
        {
            get
            {
                return mAddress;
            }
            set
            {
                mAddress = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region City

        private string mCity = string.Empty;

        public string City
        {
            get
            {
                return mCity;
            }
            set
            {
                mCity = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ZipCode

        private string mZipCode = string.Empty;

        public string ZipCode
        {
            get
            {
                return mZipCode;
            }
            set
            {
                Regex regex = new Regex(@"^[0-9]+$");
                if (regex.IsMatch(value))
                {
                    mZipCode = value;
                    RaisePropertyChanged();
                }
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
            mCacheService.SpotAddress = Address;
            mCacheService.NextStatus = AddSpotStatus.SpotSize;
            mCacheService.CreateParkingRequest.Location = Address;
            mCacheService.CreateParkingRequest.Latitude = mCacheService.CurrentLat;
            mCacheService.CreateParkingRequest.Longitude = mCacheService.CurrentLng;
   
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

