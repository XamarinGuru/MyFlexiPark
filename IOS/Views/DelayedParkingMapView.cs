
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;
using System.Timers;
using FlexyPark.Core;
using System.Collections.Generic;

namespace FlexyPark.UI.Touch.Views
{
	public partial class DelayedParkingMapView : BaseView
	{
		public DelayedParkingMapView()
			: base("DelayedParkingMapView", null)
		{
		}

		public new DelayedParkingMapViewModel ViewModel
		{
			get
			{
				return base.ViewModel as DelayedParkingMapViewModel;
			}
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

				var language = NSUserDefaults.StandardUserDefaults.StringForKey(AppConstants.Language);
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

				webView.LoadRequest(new NSUrlRequest(new NSUrl(parkingURL)));//web view
				//UIApplication.SharedApplication.OpenUrl(new NSUrl(parkingURL));//in-app browser
			}
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			//SetTitle(""); 
			//SetBackButtonTitle(ViewModel.TextSource.GetText("BackTitle"));
			// Perform any additional setup after loading the view, typically from a nib.

			var set = this.CreateBindingSet<DelayedParkingMapView, DelayedParkingMapViewModel>();

			set.Bind().For(s => s.ParkingURL).To(vm => vm.ParkingURL);

			#region Language Binding

			#endregion

			set.Apply();
		}

	}
}
