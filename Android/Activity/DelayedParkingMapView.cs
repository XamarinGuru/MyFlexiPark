using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using FlexyPark.Core;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using Java.Util;

namespace FlexyPark.UI.Droid.Activity
{
	[Activity(Label = "DelayedParkingMapView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
	public class DelayedParkingMapView : BaseView
	{
		#region UI Controls



		#endregion

		#region Variables

		private System.Timers.Timer mTimer;

		private string mSharedPreferences = "MyPrefs";
		private ISharedPreferences sharedPreferences;

		#endregion

		#region Overrides

		public new DelayedParkingMapViewModel ViewModel
		{
			get { return base.ViewModel as DelayedParkingMapViewModel; }
			set
			{
				base.ViewModel = value;

			}
		}

		private Dictionary<string, string> _parkingURL;
		public Dictionary<string, string> ParkingURL
		{
			get
			{
				return _parkingURL;
			}
			set
			{
				_parkingURL = value;

				var language = GetPreference<string>(AppConstants.Language);
				var formatedLanguage = "en";
				switch (language)
				{
					case "English":
						formatedLanguage = "en";
						break;
					case "French":
						formatedLanguage = "fr";
						break;
					case "Dutch":
						formatedLanguage = "nl";
						break;
					default:
						formatedLanguage = "en";
						break;
				}

				var baseURL = string.Format(ApiUrls.BaseWebURL, formatedLanguage);
				var sha256Password = "&password=" + SHA256StringHash(_parkingURL["Password"]);

				var parkingURL = baseURL + _parkingURL["Email"] + sha256Password + _parkingURL["ParkingId"] + _parkingURL["StartTimestamp"] + _parkingURL["EndTimestamp"] + _parkingURL["PlateNumber"];

				var webView = FindViewById<WebView>(Resource.Id.webView);

				webView.Settings.JavaScriptEnabled = true;
				webView.Settings.AllowContentAccess = true;
				webView.Settings.EnableSmoothTransition();
				webView.Settings.LoadsImagesAutomatically = true;
				webView.Settings.SetGeolocationEnabled(true);
				webView.SetWebViewClient(new WebViewClient());

				webView.ClearCache(true);
				webView.ClearHistory();

	            webView.LoadUrl(parkingURL);
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.DelayedParkingMapView);

			sharedPreferences = GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);
			//string LanguageValue = string.Empty;

			//LanguageValue = GetPreference<string>(AppConstants.Language);
			//Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(LanguageValue);

			var set = this.CreateBindingSet<DelayedParkingMapView, DelayedParkingMapViewModel>();
			set.Bind().For(s => s.ParkingURL).To(vm => vm.ParkingURL);
			set.Apply();

			//ViewModel.View = this;
			DecreaseTime();
			SetButtonEffects(new List<int>()
			{
				Resource.Id.rlBack,
				Resource.Id.llVehicle,
				Resource.Id.tvBuyCredits,
				Resource.Id.tvPayNow,


			});

		}

		protected override void OnResume()
		{
			base.OnResume();

		}

		protected override void OnPause()
		{
			base.OnPause();
			OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
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

		public void StopTimer()
		{
			if (mTimer != null)
			{
				mTimer.Stop();
				mTimer.Elapsed -= OnTimedEvent;
				mTimer.Dispose();
				mTimer = null;
			}
		}

		#endregion

		#region Methods

		public void DecreaseTime()
		{
			if (mTimer == null)
			{
				mTimer = new System.Timers.Timer(1000);
				mTimer.Elapsed += OnTimedEvent;
				mTimer.Enabled = true;
			}


		}


		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			//ViewModel.BookingTime--;

			////Update visual representation here
			////Remember to do it on UI thread

			//if (ViewModel.BookingTime == 0)
			//{
			//    mTimer.Stop();
			//}
		}

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