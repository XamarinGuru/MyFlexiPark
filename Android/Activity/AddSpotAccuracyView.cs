using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
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
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "AddSpotAccuracyView")]
    public class AddSpotAccuracyView : BaseView, Android.Locations.ILocationListener, Android.Gms.Location.ILocationListener, Android.Gms.Common.Apis.IGoogleApiClientConnectionCallbacks, Android.Gms.Common.Apis.IGoogleApiClientOnConnectionFailedListener
    {
        #region UI Controls

        #endregion

        #region Variables

        IGoogleApiClient apiClient;
        LocationRequest locRequest;
        private LocationManager locationManager;
        private AlertDialog builder;
        private string best;

        private long MinTime = 0;
        private float Distance = 0;

        #endregion

        #region Constructors

        public new AddSpotAccuracyViewModel ViewModel
        {
            get { return base.ViewModel as AddSpotAccuracyViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddSpotAccuracyView);

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true, ""));

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



            builder = new AlertDialog.Builder(this).Create();
            builder.SetTitle(ViewModel.SharedTextSource.GetText("MessageTitle"));
            builder.SetMessage(ViewModel.SharedTextSource.GetText("TurnOnGPSText"));
            builder.SetCancelable(false);
            builder.SetButton(ViewModel.SharedTextSource.GetText("CancelText"), (sender, args) => Finish());
            builder.SetButton2(ViewModel.SharedTextSource.GetText("SettingText"), (sender, args) =>
            {
                StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            });

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack
            });

        }

        protected override void OnResume()
        {
            base.OnResume();

            apiClient.Connect();
        }



        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            if (apiClient.IsConnected)
            {
                LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);
                apiClient.Disconnect();
            }
            try
            {
                if (builder != null)
                {
                    builder.Cancel();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Implements

        public void OnLocationChanged(Location location)
        {
            ViewModel.CurrentAccuracy = location.Accuracy;
            ViewModel.CurrentLat = location.Latitude;
            ViewModel.CurrentLng = location.Longitude;
            var accuracy = AppConstants.DesiredAccuracy;

            var timeZone = TimeZone.CurrentTimeZone.StandardName;

            if (timeZone == "ICT")
            {
                accuracy = 20;
            }

            if (ViewModel.CurrentAccuracy <= accuracy)
            {
                Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
                Mvx.Resolve<ICacheService>().CurrentLat = location.Latitude;
                Mvx.Resolve<ICacheService>().CurrentLng = location.Longitude;
                ViewModel.CloseViewModel();

            }

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false, ""));
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
            Task.Run(async () =>
            {
                await Task.Delay(10000);

            });
        }


        public void OnConnectionSuspended(int cause)
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {

        }

        #endregion

        #region Methods

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