using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using Timer = System.Timers.Timer;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "Parking List View", MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class ParkingListView : BaseView, TimePickerDialog.IOnTimeSetListener, DatePickerDialog.IOnDateSetListener, Android.Locations.ILocationListener, Android.Gms.Location.ILocationListener, Android.Gms.Common.Apis.IGoogleApiClientConnectionCallbacks, Android.Gms.Common.Apis.IGoogleApiClientOnConnectionFailedListener, IParkingListsView
    {
        #region UI Controls

        private SeekBar mSeekBar;
        private TextView etHour, etDate;
        private DatePickerDialog mDatePickerDialog;
        private TimePickerDialog mTimePickerDialog;
        private IGoogleApiClient apiClient;
        private LocationRequest locRequest;
        private LocationManager locationManager;
        private System.Timers.Timer timer, m60sTimer;
        #endregion

        #region Variables

        private int mCurrentProgress;
        private int mCount = 0;

        #endregion

        #region Overrides

        public new ParkingListsViewModel ViewModel
        {
            get { return base.ViewModel as ParkingListsViewModel; }
            set
            {
                base.ViewModel = value;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParkingListView);
            ViewModel.View = this;
            Init();
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
            });


        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            apiClient.Disconnect();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModel.Title = "Acquiring location…";
            StartTimer();
            if (Mvx.Resolve<ICacheService>().SearchMode == SearchMode.Now) //park me now
            {
                Start60sTimer();
                apiClient.Connect();
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, true));
            }
            else //park me later
            {
                ViewModel.GetParkingLists();
            }

            ViewModel.UpdateValidTime();
        }

        #endregion

        #region Implements

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            etDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;
            var endDate = new DateTime(year, monthOfYear, dayOfMonth, ViewModel.EndBookingDateTime.Hour, ViewModel.EndBookingDateTime.Minute, 0);
            ViewModel.EndBookingDateTime = endDate;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            etHour.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
            var endDate = new DateTime(ViewModel.EndBookingDateTime.Year, ViewModel.EndBookingDateTime.Month, ViewModel.EndBookingDateTime.Day, hourOfDay, minute, 0);
            ViewModel.EndBookingDateTime = endDate;


        }

        public void OnLocationChanged(Location location)
        {
            if (ViewModel.Latitude != 0 && ViewModel.Longitude != 0)
                return;
            if (mCount < 5)
            {
                mCount++;
                return;
            }

            if (location.Accuracy <= 50)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
                mCount = 0;
                Mvx.Resolve<ICacheService>().CurrentLat = location.Latitude;
                Mvx.Resolve<ICacheService>().CurrentLng = location.Longitude;

                apiClient.Disconnect();

                ViewModel.GetParkingLists();
            }
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

        public void OnConnectionFailed(ConnectionResult result)
        {

        }

        #endregion

        #region Methods

        #region Init

        public void Init()
        {
            mDatePickerDialog = new DatePickerDialog(this, this, ViewModel.EndBookingDateTime.Year, ViewModel.EndBookingDateTime.Month - 1, ViewModel.EndBookingDateTime.Day);
            mTimePickerDialog = new TimePickerDialog(this, this, ViewModel.EndBookingDateTime.Hour, ViewModel.EndBookingDateTime.Minute, true);
            etDate = FindViewById<TextView>(Resource.Id.etDate);
            etHour = FindViewById<TextView>(Resource.Id.etHour);
            mSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            mSeekBar.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Up)
                {
                    if (mSeekBar.Progress != mCurrentProgress)
                    {
                        mCurrentProgress = mSeekBar.Progress;
                        ViewModel.HandleValueChanged();
                        args.Handled = true;
                        return;
                    }
                }

                args.Handled = false;
            };
            mCurrentProgress = 0;
            etDate.Click += (sender, args) =>
            {
                mDatePickerDialog.UpdateDate(ViewModel.EndBookingDateTime.Year, ViewModel.EndBookingDateTime.Month - 1, ViewModel.EndBookingDateTime.Day);
                mDatePickerDialog.Show();
            };
            etHour.Click += (sender, args) =>
            {
                mTimePickerDialog.UpdateTime(ViewModel.EndBookingDateTime.Hour, ViewModel.EndBookingDateTime.Minute);
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
                locRequest.SetInterval(100);

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

        #region StartTimer

        public void SetSliderValue()
        {

        }

        public void ShowDatePicker()
        {
        }

        public void ShowTimePicker()
        {
        }

        public void ResetHeight()
        {
        }

        public void StartTimer()
        {
            if (timer == null)
            {
                timer = new Timer(60000);
                timer.Elapsed += TimerOnElapsed;
                timer.Start();
            }
        }

        void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (ViewModel.StartTimeStamp != 0) //book later
                ViewModel.StartTimeStamp += 60;
            ViewModel.UpdateValidTime();
        }

        #endregion

        #region StopTimer

        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= TimerOnElapsed;
                timer.Dispose();
                timer = null;
            }
        }

        public void Start60sTimer()
        {
            if (m60sTimer == null)
            {
                m60sTimer = new Timer(1000);
                m60sTimer.Elapsed += M60sTimer_Elapsed;
                m60sTimer.Start();
            }
        }

        void M60sTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ViewModel.Counter++;
            if (ViewModel.Counter == 60)
            {
                ViewModel.ShowWeakSignalGPS();
            }

        }

        public void Stop60sTimer()
        {
            if (m60sTimer != null)
            {
                m60sTimer.Stop();
                m60sTimer.Elapsed -= M60sTimer_Elapsed;
                m60sTimer.Dispose();
                m60sTimer = null;
            }
        }

        #endregion

        #endregion


    }
}