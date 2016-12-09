using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using ILocationListener = Android.Locations.ILocationListener;

namespace FlexyPark.UI.Droid.Activity
{

    [Activity(Label = "ParkingSearchView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustPan, Theme = "@style/AppBaseTheme")]
    public class ParkingSearchView : BaseView, TimePickerDialog.IOnTimeSetListener, DatePickerDialog.IOnDateSetListener, IParkingSearchView, Android.Locations.ILocationListener, Android.Gms.Location.ILocationListener, Android.Gms.Common.Apis.IGoogleApiClientConnectionCallbacks, Android.Gms.Common.Apis.IGoogleApiClientOnConnectionFailedListener
    {
        #region UI Controls

        private TextRegular etStartHour, etStartDate;
        private TextView tvSearchingFor;
        private DatePickerDialog mDatePickerDialog;
        private TimePickerDialog mTimePickerDialog;
        private IGoogleApiClient apiClient;
        private LocationRequest locRequest;
        private LocationManager locationManager;

        #endregion

        #region Variables

        private bool IsStartClick = false;

        #endregion

        #region Constructors

        public new ParkingSearchViewModel ViewModel
        {
            get { return base.ViewModel as ParkingSearchViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParkingSearchView);
            ViewModel.View = this;
            Init();
            tvSearchingFor = FindViewById<TextView>(Resource.Id.tvSearchingFor);
            SetClickOutsideToHideKeyboard(FindViewById<View>(Resource.Id.parentView));
            
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvVehicle,
                Resource.Id.tvSearch,
            });
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #endregion

        #region Implements

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            if (IsStartClick)
            {
                etStartDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;
                DateTime mStartDateTime = new DateTime(year, monthOfYear + 1, dayOfMonth, ViewModel.StartBookingDateTime.Hour, ViewModel.StartBookingDateTime.Minute, 0);

                ViewModel.StartBookingDateTime = mStartDateTime;
            }
            else
            {
                //etDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;

                DateTime mEndDateTime = new DateTime(year, monthOfYear + 1, dayOfMonth, ViewModel.EndBookingDateTime.Hour, ViewModel.EndBookingDateTime.Minute, 0);

                ViewModel.EndBookingDateTime = mEndDateTime;
            }

        }

        public void Dispose()
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        void Android.Gms.Location.ILocationListener.OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            if (IsStartClick)
            {
                etStartHour.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
                DateTime tmp = new DateTime(ViewModel.StartBookingDateTime.Year, ViewModel.StartBookingDateTime.Month, ViewModel.StartBookingDateTime.Day, hourOfDay, minute, 0);
                ViewModel.StartBookingDateTime = tmp;
            }
            else
            {
                //etHour.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
                DateTime tmp = new DateTime(ViewModel.EndBookingDateTime.Year, ViewModel.EndBookingDateTime.Month, ViewModel.EndBookingDateTime.Day, hourOfDay, minute, 0);
                ViewModel.EndBookingDateTime = tmp;

            }


        }

        public void ShowDatePicker(bool isStart)
        {

        }

        public void ShowTimePicker(bool isStart)
        {
        }

        public void StartGetLocation()
        {
            if (apiClient != null)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(ViewModel, true));
                apiClient.Connect();
            }
        }

        void ILocationListener.OnLocationChanged(Location location)
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(ViewModel, true));
            Mvx.Resolve<ICacheService>().CurrentLat = location.Latitude;
            Mvx.Resolve<ICacheService>().CurrentLng = location.Longitude;
            apiClient.Disconnect();
            ViewModel.CheckPlacesCommand.Execute();
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        public void OnConnected(Bundle connectionHint)
        {
            RequestUpdateLocations();
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        #endregion

        #region Methods

        #region Init

        public void Init()
        {
            mDatePickerDialog = new DatePickerDialog(this, this, ViewModel.EndBookingDateTime.Year, ViewModel.EndBookingDateTime.Month - 1, ViewModel.EndBookingDateTime.Day);
            mTimePickerDialog = new TimePickerDialog(this, this, ViewModel.EndBookingDateTime.Hour, ViewModel.EndBookingDateTime.Minute, true);
            etStartDate = FindViewById<TextRegular>(Resource.Id.etStartDate);
            etStartHour = FindViewById<TextRegular>(Resource.Id.etStartHour);
            etStartDate.Click += (sender, args) =>
            {
                IsStartClick = true;
                mDatePickerDialog.UpdateDate(ViewModel.StartBookingDateTime.Year, ViewModel.StartBookingDateTime.Month - 1, ViewModel.StartBookingDateTime.Day);
                mDatePickerDialog.Show();
            };
            etStartHour.Click += (sender, args) =>
            {
                IsStartClick = true;
                mTimePickerDialog.UpdateTime(ViewModel.StartBookingDateTime.Hour, ViewModel.StartBookingDateTime.Minute);
                mTimePickerDialog.Show();
            };

            locationManager = (LocationManager)GetSystemService(LocationService);
            bool GPSEnable = DetectLocationService();

            var _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new GoogleApiClientBuilder(this, this, this)
                    .AddApi(LocationServices.API).Build();

                // generate a location request that we will pass into a call for location updates
                locRequest = new LocationRequest();

            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, ViewModel.SharedTextSource.GetText("GGPlayNotInstalledText"), ToastLength.Long).Show();

            }
        }

        #endregion

        #region RequestUpdateLocations

        private void RequestUpdateLocations()
        {
            if (apiClient.IsConnected)
            {
                // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
                locRequest.SetPriority(100);

                // Setting interval between updates, in milliseconds
                // NOTE: the default FastestInterval is 1 minute. If you want to receive location updates more than 
                // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
                locRequest.SetFastestInterval(500);
                locRequest.SetInterval(1000);

                Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms",
                    locRequest.Priority.ToString(), locRequest.Interval.ToString());

                // pass in a location request and LocationListener
                LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
                // In OnLocationChanged (below), we will make calls to update the UI
                // with the new location data
            }
            else
            {
                Log.Info("LocationClient", "Please wait for Client to connect");
            }
        }

        #endregion

        #region DetectLocationService


        public bool DetectLocationService() // GPS
        {

            var lm = (LocationManager)GetSystemService(LocationService);

            try
            {
                var gpsEnabled = lm.IsProviderEnabled(LocationManager.GpsProvider);

                if (gpsEnabled)
                    return true;
            }
            catch (Exception ex)
            {
                var str = ex.Message;

            }

            try
            {
                var networkEnabled = lm.IsProviderEnabled(LocationManager.NetworkProvider);

                if (networkEnabled)
                    return true;
            }
            catch (Exception exx)
            {
                var str = exx.Message;
            }

            return false;

        }
        #endregion

        #region IsGooglePlayServicesInstalled

        private bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);

            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(Class.Name, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(Class.Name, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
            }
            return true;
        }

        #endregion

        #endregion


    }
}