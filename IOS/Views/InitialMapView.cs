using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using BigTed;
using FlexyPark.UI.Touch.Helpers;
using System.Diagnostics;
using CoreLocation;
using MapKit;
using System.Collections.ObjectModel;

using Google.Maps;
using CoreGraphics;
using System.Drawing;
using FlexyPark.Core;
using System.Security.Cryptography;
using System.Globalization;

namespace FlexyPark.UI.Touch.Views
{
	public partial class InitialMapView : BaseView
	{
		public InitialMapView()
			: base("InitialMapView", null)
		{
		}

		public new InitialMapViewModel ViewModel
		{
			get
			{
				return base.ViewModel as InitialMapViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		private MapView mapView;

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

				foreach (var parking in _parkings)
				{
					var marker = new Marker
					{
						Position = new CLLocationCoordinate2D(double.Parse(parking.Latitude), double.Parse(parking.Longitude)),
						Map = mapView,
						Icon = UIImage.FromBundle("icon_location_30.png"),
					};
				}
			}
		}


		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillLayoutSubviews()
		{
			if (mapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			HideBackButton();
			HideNavigationBar(true);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			// Perform any additional setup after loading the view, typically from a nib.

			var set = this.CreateBindingSet<InitialMapView, InitialMapViewModel>();

			set.Bind().For(s => s.Parkings).To(vm => vm.Parkings);

			SetTitle("InitialMapView");

			set.Bind(btnGoToSignIn).To(vm => vm.GotoSignInCommand);

			var camera = CameraPosition.FromCamera(AppConstants.locCenterOfBelgium[0], AppConstants.locCenterOfBelgium[1], zoom: 7);
			mapView = MapView.FromCamera(RectangleF.Empty, camera);
			mapView.MyLocationEnabled = false;


			#region Language Binding

			//set.Bind(btnGoToSignIn).For("Title").To(vm => vm.TextSource).WithConversion("Language", "SignInText");

			#endregion

			ViewModel.AvaiableParkingsInMap();

			set.Apply();

			SignThis("PUT\n\n\n0\n\n\n\n\n\n\n\n\nx-ms-blob-public-access:blob\nx-ms-date:Fri, 02 Dec 2016 10:25:02 GMT\nx-ms-version:2009-09-19\n/hbaudit/yusukecontainer\nrestype:container", "YQVUjR8oZ2IRG/u/rZvDQXBE+Au0vKryuL/NLjDhEJiX+ehjeZ/O3ycNffROPPqKANur4hAOJcOFTc7J/0RTOg==");

		}



		//private String CreateHmacSignature(String unsignedString, Byte[] key)
		//{
		//    if (String.IsNullOrEmpty(unsignedString))
		//    {
		//        throw new ArgumentNullException("unsignedString");
		//    }

		//    if (key == null)
		//    {
		//        throw new ArgumentNullException("key");
		//    }

		//    Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(unsignedString);
		//    using (HMACSHA256 hmacSha256 = new HMACSHA256(key))
		//    {
		//        return Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
		//    }
		//}
		private static String SignThis(String StringToSign, string key)
		{
			String signature = string.Empty;
			byte[] unicodeKey = Convert.FromBase64String(key);
			using (HMACSHA256 hmacSha256 = new HMACSHA256(unicodeKey))
			{
				Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(StringToSign);
				signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
			}

			String authorizationHeader = String.Format(
				  CultureInfo.InvariantCulture,
				  "{0} {1}:{2}",
				  "SharedKey",
				  "hbaudit",
				  signature);

			return authorizationHeader;
		}





		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			var width = viewMapContent.Window.Frame.Width;
			var height = viewMapContent.Window.Frame.Height;
			mapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mapView);
		}
	}
}

