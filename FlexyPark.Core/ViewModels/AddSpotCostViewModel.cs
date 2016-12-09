using System;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Globalization;

namespace FlexyPark.Core.ViewModels
{
    public interface IAddSpotCostView
    {
        void SetSliderValue();
    }

    public class AddSpotCostViewModel : BaseViewModel
    {
        #region Constructors

        public AddSpotCostViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init(double hourlyRate, double lat, double lng)
        {
            if (hourlyRate != 0)
            {
                //Mvx.Resolve<ICacheService>().CurrentLat = ;
                SelectedValue = (float)hourlyRate - 1;
                Longitude = lng;
                Latitude = lat;
            }
            else
            {
                SelectedPrice = SelectedValue + 1;
                Longitude = Mvx.Resolve<ICacheService>().CurrentLng;
                Latitude = Mvx.Resolve<ICacheService>().CurrentLat;
            }

            await Task.Delay(100);

            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var results = await mApiService.RecommendPrice(Latitude, Longitude);
                if (results != null)
                {
                    //RecommendedPrice = double.Parse(results.Prices.ParkingRateHourly, CultureInfo.InvariantCulture);
                  
                    decimal value = (decimal)Math.Round((decimal)results.Prices.ParkingRateHourly * 2m, MidpointRounding.AwayFromZero) / 2m;
                    RecommendedPrice = (double)value;
                    if (hourlyRate == 0)
                    {
                        
                        SelectedValue = (float)RecommendedPrice - 1;
                      
                    }
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Api Invalid rec. price"));
                    SelectedValue = 0;
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Please turn on internet"));
            }
        }

        #endregion

        #region View

        public IAddSpotCostView View { get; set; }

        #endregion

        #region Properties

        #region SelectedValue

        /// <summary>
        /// Binding to Slider/SeekBar value.
        /// </summary>
        private float mSelectedValue = 1;

        public float SelectedValue
        {
            get
            {
                return mSelectedValue;
            }
            set
            {
                mSelectedValue = value;
                RaisePropertyChanged();


                SelectedPrice = SelectedValue + 1;
            }
        }

        #endregion

        #region SelectedPrice

        /// <summary>
        /// Binding to UILabel/TextView. Using "MoneyConverter".
        /// </summary>
        private double mSelectedPrice = 1;

        public double SelectedPrice
        {
            get
            {
                return mSelectedPrice;
            }
            set
            {
                mSelectedPrice = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RecommendedPrice

        private double mRecommendedPrice = 1;

        public double RecommendedPrice
        {
            get
            {
                return mRecommendedPrice;
            }
            set
            {
                mRecommendedPrice = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Longtitude

        public double Longitude { get; set; }


        #endregion

        #region Latitude

        public double Latitude { get; set; }

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
            mCacheService.CreateParkingRequest.HourlyRate = SelectedPrice;
            mCacheService.NextStatus = AddSpotStatus.SpotCalendar;
            Close(this);
        }

        #endregion

        #region HandleValueChangedCommand

        private MvxCommand mHandleValueChangedCommand = null;

        public MvxCommand HandleValueChangedCommand
        {
            get
            {
                if (mHandleValueChangedCommand == null)
                {
                    mHandleValueChangedCommand = new MvxCommand(this.HandleValueChanged);
                }
                return mHandleValueChangedCommand;
            }
        }

        public void HandleValueChanged()
        {
            if (View != null)
                View.SetSliderValue();
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }


}

