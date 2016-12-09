using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "AddSpotView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AddSpotView : BaseView, IAddSpotView, Android.Gms.Location.ILocationListener, Android.Gms.Common.Apis.IGoogleApiClientConnectionCallbacks, Android.Gms.Common.Apis.IGoogleApiClientOnConnectionFailedListener, Android.Locations.ILocationListener
    {
        #region UI Controls
        LinearLayout ln;
        private AlertDialog builder;
        private TextView tvTitle;
        // tvAdd, tvAddVirtual;
        private RelativeLayout rlHeader, rlBack;

        #endregion

        #region Variables

        bool _isGooglePlayServicesInstalled;
        IGoogleApiClient apiClient;
        LocationRequest locRequest;
        private LocationManager locationManager;
        #endregion

        #region Constructors

        #endregion

        #region Overrides

        public new AddSpotViewModel ViewModel
        {
            get { return base.ViewModel as AddSpotViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddSpotView);
            ViewModel.View = this;
            locationManager = (LocationManager)GetSystemService(LocationService);
            tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);
            // tvAdd = FindViewById<TextView>(Resource.Id.tvAdd);
            //tvAddVirtual = FindViewById<TextView>(Resource.Id.tvAddVirtual);
            //rlHeader = FindViewById<RelativeLayout>(Resource.Id.rlHeader);
            rlBack = FindViewById<RelativeLayout>(Resource.Id.rlBack);



            //var set = this.CreateBindingSet<AddSpotView, AddSpotViewModel>();
            //set.Bind(tvTitle)
            //    .For(v => v.Text)
            //    .To(vm => vm.Title)
            //    .WithConversion("AddEditPageTitleConverter", (ViewModel.Title == ViewModel.mCacheService.TextSource.GetText("")) ? "Edit" : "Add");

            //set.Bind(tvAdd)
            //    .For(v => v.Text)
            //    .To(vm => vm.ButtonTitle)
            //    .WithConversion("AddEditButtonTitleConverter", (ViewModel.ButtonTitle == "Add") ? "Add" : "Edit");

            //set.Bind(tvAddVirtual)
            //   .For(v => v.Text)
            //   .To(vm => vm.ButtonTitle)
            //   .WithConversion("AddEditButtonTitleConverter", (ViewModel.ButtonTitle == "Add") ? "Add" : "Edit");

            //set.Apply();
            bool GPSEnable = DetectLocationService();
            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            //Mvx.Resolve<ICacheService>().CurrentLat = 10.7939;
            //Mvx.Resolve<ICacheService>().CurrentLng = 106.659;
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
                Resource.Id.rlBack,
                //Resource.Id.tvAdd
            });
        }



        protected override void OnResume()
        {
            base.OnResume();
            apiClient.Connect();

            if (ViewModel.IsEditMode)
            {
                if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Activation)
                {
                    return;
                }

                if (Mvx.Resolve<ICacheService>().NextStatus != AddSpotStatus.GotoSpot &&
                    Mvx.Resolve<ICacheService>().NextStatus != AddSpotStatus.Complete)
                {
                    ViewModel.DoAddOrSaveParkingSpot();

                }

                else if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Complete)
                {
                    ViewModel.DoAddOrSaveParkingSpot();
                }
            }
            else
            {
                if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.SpotCalendar)
                {
                    ViewModel.DoAddOrSaveParkingSpot();
                }
                else if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Complete)
                {
                    ViewModel.SaveParkingSpot();
                }
            }

            if (Mvx.Resolve<ICacheService>().NextStatus != ViewModel.Status)
            {
                ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
            }
            if (ViewModel.Status == AddSpotStatus.GPS)
            {
                GetGPS();
            }

            if (ViewModel.Parking != null && Mvx.Resolve<ICacheService>().CreateParkingRequest != null && Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate != null)
            {
                ViewModel.Parking.HourlyRate = Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate;

            }


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

        public void ReloadTasks()
        {

        }

        public async void GetGPS()
        {
            await Task.Delay(2000);
            if (Mvx.Resolve<ICacheService>().CurrentLng == 0 || Mvx.Resolve<ICacheService>().CurrentLng == 0)
            {

                builder.Show();
            }
            else
            {
                Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
            }

        }

        public async void GetAccuracy()
        {
            //var tcs = new TaskCompletionSource<bool>();
            //await Task.Delay(2000);
            //Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
            //ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
            //tcs.TrySetResult(true);

            //var criteriaForLocationService = new Criteria
            //{

            //};
            //var best = locationManager.GetBestProvider(criteriaForLocationService, true);

            //locationManager.RequestLocationUpdates(best, long.Parse("1500"), float.Parse("10"), this);

            //var acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
            ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);

        }

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

        public void OnLocationChanged(Location location)
        {


            if (location != null)
            {
                if (Mvx.Resolve<ICacheService>().CurrentLng == 0)
                {
                    Mvx.Resolve<ICacheService>().CurrentLat = location.Latitude;
                    Mvx.Resolve<ICacheService>().CurrentLng = location.Longitude;
                    if (ViewModel.Status == AddSpotStatus.GPS)
                    {

                        Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                        ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
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
                }
                else
                {
                    Mvx.Resolve<ICacheService>().CurrentLat = location.Latitude;
                    Mvx.Resolve<ICacheService>().CurrentLng = location.Longitude;

                }



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
            //SetResult(Result.Canceled);
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
            //SetResult(Result.Canceled);
        }

        #endregion

        #region Methods

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

        #region SetResult



        #endregion


        #endregion


    }
}