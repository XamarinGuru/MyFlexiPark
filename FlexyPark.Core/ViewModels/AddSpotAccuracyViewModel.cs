using System;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using Cirrious.CrossCore;

namespace FlexyPark.Core.ViewModels
{
    public class AddSpotAccuracyViewModel : BaseViewModel
    {
        #region Constructors

        public AddSpotAccuracyViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init()
        {
            
        }
        #endregion

        #region Properties

        #region GPSTarget

        private double mGPSTarget = AppConstants.DesiredAccuracy;

        public double  GPSTarget
        {
            get
            {
                return mGPSTarget;
            }
            set
            {
                mGPSTarget = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region CurrentAccuracy

        private double mCurrentAccuracy = 0.0;

        public double CurrentAccuracy
        {
            get
            {
                return mCurrentAccuracy;
            }
            set
            {
                mCurrentAccuracy = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region CurrentLat

        private double mCurrentLat = 0.0;

        public double CurrentLat
        {
            get
            {
                return mCurrentLat;
            }
            set
            {
                mCurrentLat = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region CurrentLng

        private double mCurrentLng = 0.0;

        public double CurrentLng
        {
            get
            {
                return mCurrentLng;
            }
            set
            {
                mCurrentLng = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Command

        #endregion

        #region Methods

        public async void CloseViewModel()
        {
            await Task.Delay(500);
            Mvx.Resolve<ICacheService>().CreateParkingRequest.GpsAccuracy = "5";

            Close(this);
        }

        #endregion
    }
}

