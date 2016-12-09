using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlexyPark.Core;
using FlexyPark.Core.ViewModels;
using Uri = Android.Net.Uri;

namespace FlexyPark.UI.Droid.Activity
{

    [Activity(ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AppSettingsView : BaseView, IAppSettingsView
    {
        #region UI Controls

        #endregion

        #region Variables

        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor SharedPreferencesEditor;
        private string mSharedPreferences = "MyPrefs";
        

        #endregion

        #region Constructors

        public new AppSettingsViewModel ViewModel
        {
            get { return base.ViewModel as AppSettingsViewModel; }
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
            SetContentView(Resource.Layout.AppSettingsView);
            ViewModel.View = this;
            
            sharedPreferences = GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                //Resource.Id.tvDone
            });
        }
        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected async override void OnResume()
        {
            base.OnResume();
        
            await Task.Delay(500);
            ViewModel.IsLoading = false;

        }

        #endregion

        #region Implements

        public T GetPreference<T>(string key)
        {
            string value = sharedPreferences.GetString(key, String.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                return  (T) Convert.ChangeType(value, typeof(T));
            }
            var t = default(T);
            return t;
        }

        public void SetPreference<T>(string key, T value)
        {
            SharedPreferencesEditor = sharedPreferences.Edit();
            SharedPreferencesEditor.PutString(key, value.ToString());
            SharedPreferencesEditor.Commit();
        }

        public string GetAppVersion()
        {
            PackageInfo pInfo = PackageManager.GetPackageInfo(PackageName, 0);
            string version = pInfo.VersionName;
            return version;
        }

        public bool CheckIfAppInstalled(string appName)
        {
            


            PackageManager packageManager = this.PackageManager;
            try
            {
                packageManager.GetPackageInfo(GetPackageName(appName), PackageInfoFlags.Activities);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void GotoStoreToInstallApp(string appName)
        {
             //startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
            StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id="+ GetPackageName(appName))));
        }

        public void ShowLanguagePicker()
        {
            
        }

        #endregion

        #region Methods

        public string GetPackageName(string appName)
        {
            string PackageName = string.Empty;

            switch (appName)
            {
                case AppConstants.GoogleMaps:
                    PackageName = "com.google.android.apps.maps";
                    break;
                case AppConstants.Waze:
                    PackageName = "com.waze";
                    break;
               
            }
            return PackageName;
        }

        #endregion


    }
}