
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlexyPark.Core;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using Com.Crittercism.App;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Activity;


namespace FlexyPark.UI.Droid
{
    [Activity(Label = "My Flexy Park", MainLauncher = false, Icon = "@drawable/icon", NoHistory = false, ScreenOrientation = ScreenOrientation.SensorPortrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme", LaunchMode = LaunchMode.SingleTask)]
    public class SignInActivity : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        private string mSharedPreferences = "MyPrefs";
        private ISharedPreferences sharedPreferences;

        #endregion

        #region Overrides

        public new SignInViewModel ViewModel
        {
            get { return base.ViewModel as SignInViewModel; }
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
            SetContentView(Resource.Layout.SignInView);

            Crittercism.Initialize(this, "55fcbf43d224ac0a00ed3d83");

            sharedPreferences = GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);
            string LanguageValue = string.Empty;

            LanguageValue = GetPreference<string>(AppConstants.Language);
            Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(LanguageValue);
            // Create your application here
            SetButtonEffects(new List<int>()
            {
                Resource.Id.tvSigin,
                Resource.Id.tvSignUp,
                Resource.Id.tvForgotPassword,
				Resource.Id.rlBack
            });

            //get preferences
            //Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(string.Empty);
            //Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(value);
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #endregion

        #region Implements

        #endregion

        #region Methods

        public T GetPreference<T>(string key)
        {
            string value = sharedPreferences.GetString(key, String.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            var t = default(T);
            return t;
        }

        #endregion

    }
}

