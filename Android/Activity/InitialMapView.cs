
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

using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using System.Collections.ObjectModel;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Droid
{
	[Activity(Label = "Init Map View", MainLauncher = false, Icon = "@drawable/icon", NoHistory = false, ScreenOrientation = ScreenOrientation.SensorPortrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme", LaunchMode = LaunchMode.SingleTask)]
	public class InitialMapView : BaseView, IOnMapReadyCallback
	{
		#region UI Controls

		#endregion

		#region Variables

		private string mSharedPreferences = "MyPrefs";
		private ISharedPreferences sharedPreferences;

		#endregion

		#region Overrides

		public new InitialMapViewModel ViewModel
		{
			get { return base.ViewModel as InitialMapViewModel; }
			set
			{
				base.ViewModel = value;
			}
		}

		private GoogleMap mapView;

		private ObservableCollection<Parking> _parkings;
		public ObservableCollection<Parking> Parkings
		{
			get
			{
				return _parkings;
			}
			set
			{
				_parkings = value;

				if (mapView == null) return;

				foreach (var parking in _parkings)
				{
					MarkerOptions markerOpt = new MarkerOptions();
					markerOpt.SetPosition(new LatLng(double.Parse(parking.Latitude), double.Parse(parking.Longitude)));
					markerOpt.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_location));
					mapView.AddMarker(markerOpt);
				}
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
			SetContentView(Resource.Layout.InitialMapView);

			Crittercism.Initialize(this, "55fcbf43d224ac0a00ed3d83");

			sharedPreferences = GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);
			string LanguageValue = string.Empty;

			LanguageValue = GetPreference<string>(AppConstants.Language);
			Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(LanguageValue);

			SupportMapFragment mapViewFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView);
			mapViewFragment.GetMapAsync(this);

			// Create your application here

			var set = this.CreateBindingSet<InitialMapView, InitialMapViewModel>();
			set.Bind().For(s => s.Parkings).To(vm => vm.Parkings);
			set.Apply();

			SetButtonEffects(new List<int>()
			{
				Resource.Id.tvGoToSigin
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

		public void OnMapReady(GoogleMap googleMap)
		{
			mapView = googleMap;

			CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(AppConstants.locCenterOfBelgium[0], AppConstants.locCenterOfBelgium[1]), 7);
			mapView.MoveCamera(cu_center);

			mapView.UiSettings.MapToolbarEnabled = false;

			ViewModel.AvaiableParkingsInMap();
		}

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

