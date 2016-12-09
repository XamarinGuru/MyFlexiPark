using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;

namespace FlexyPark.UI.Droid.Activity
{
    public class ParkingSummaryFragment : BaseFragment, IParkingSummaryView
    {
        #region UI Controls

        private Timer mTimer;

        #endregion

        #region Variables

        private string mSharedPreferences = "MyPrefs";

        #endregion

        #region Constructors

        public ParkingSummaryFragment()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().ParkingReservedViewModel.SummaryVM;
        }

        public new ParkingSummaryViewModel ViewModel
        {
            get { return base.ViewModel as ParkingSummaryViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.ParkingSummaryFragment, container, false);
            //DecreaseTime();
            ViewModel.View = this;
            SetButtonEffects(view , new List<int>()
            {
                Resource.Id.flReportBlue,
                Resource.Id.flLeaveBlue,
                Resource.Id.llNavigate,
            });
            return view;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Elapsed -= OnTimedEvent;
                mTimer.Dispose();
                mTimer = null;
            }
        }

        #endregion

        #region Implements

        public T GetPreference<T>(string key)
        {
            ISharedPreferences sharedPreferences = Activity.GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);
            string value = sharedPreferences.GetString(key, String.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            var t = default(T);
            return t;
        }

        public void NavigateUsingWaze(double lat, double lng, int zoomLevel = 1)
        {
            try
            {
                var url = string.Format("waze://?ll={0},{1}&navigate=yes&z={2}", lat, lng, zoomLevel);
                //var urlString = string.Format("waze://?ll={0},{1}&navigate=yes&z={2}", lat, lng, zoomLevel);
              
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                
                
                
                StartActivity(intent);
            }
            catch (ActivityNotFoundException ex)
            {
                Intent intent =
                  new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.waze"));
                StartActivity(intent);
            }
        }

        public void NavigateUsingGoogleMaps(double destinationLat, double destinationLng, int zoomLevel = 1,
            DirectionsMode directionsMode = DirectionsMode.Driving)
        {
            //try
            //{
            //    var mode = directionsMode == DirectionsMode.Driving ? "driving" : "walking";
            //    var url = string.Format("http://maps.google.com/maps?saddr={0},{1}&daddr={2},{3}&zoom={4}&directionsmode={5}", sourceLat, sourceLng, destinationLat, destinationLng, zoomLevel, mode);
            //    Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            //    StartActivity(intent);
            //}
            //catch (ActivityNotFoundException ex)
            //{
            //    Intent intent =
            //      new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.google.android.apps.maps"));
            //    StartActivity(intent);
            //}
        }

        public void NavigateUsingNativeMap(double destinationLat, double destinationLng, int zoomLevel = 1,
            DirectionsMode directionsMode = DirectionsMode.Driving)
        {
            
        }


      

        public void NavigateUsingNavmii(double lat, double lng)
        {
            throw new NotImplementedException();
        }


        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            ViewModel.OfferedTime--;

            //Update visual representation here
            //Remember to do it on UI thread

            if (ViewModel.OfferedTime == 0)
            {
                mTimer.Stop();
            }
        }

        #endregion

        #region Methods

        public void DecreaseTime()
        {
            if (mTimer == null)
            {
                mTimer = new System.Timers.Timer(60000);
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
            }


        }

        #endregion


    }
}