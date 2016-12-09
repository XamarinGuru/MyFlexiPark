
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using FlexyPark.Core;
using MapKit;
using CoreLocation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Globalization;
using FlexyPark.UI.Touch.Extensions;
using System.Runtime.CompilerServices;
using Xamarin.Geolocation;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ParkingSummaryView : BaseView, IParkingSummaryView, ICLLocationManagerDelegate
    {
        private CLLocationManager locationManager;

        public ParkingSummaryView()
            : base("ParkingSummaryView", null)
        {
        }

        public new ParkingSummaryViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ParkingSummaryViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }

        public override bool HidesBottomBarWhenPushed
        {
            get
            {
                return true;
            }
            set
            {
                base.HidesBottomBarWhenPushed = value;
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

            ViewModel.View = this;

            // Perform any additional setup after loading the view, typically from a nib.
            var set = this.CreateBindingSet<ParkingSummaryView, ParkingSummaryViewModel>();

            set.Bind(btnNavigate).To(vm => vm.NavigateCommand);
            set.Bind(btnLeave).To(vm => vm.GotoLeaveParkingCommand);
            set.Bind(btnReport).To(vm => vm.ReportProblemCommand);

            set.Bind(vRented).For(v => v.Hidden).To(vm => vm.Status).WithConversion("ParkingStatusToBool", "Rented");
            set.Bind(vReserved).For(v => v.Hidden).To(vm => vm.Status).WithConversion("ParkingStatusToBool", "Reserved");

            set.Bind(vbtnLeave).For(v => v.Hidden).To(vm => vm.IsShowLeaveReportButton).WithConversion("BooleanToHidden");
            set.Bind(vbtnReport).For(v => v.Hidden).To(vm => vm.IsShowLeaveReportButton).WithConversion("BooleanToHidden");

            set.Bind(lbOfferedTime).To(vm => vm.OfferedTime).WithConversion("OfferedTime");
            set.Bind(vOfferedTime).For(v => v.Hidden).To(vm => vm.IsShowOfferedTime).WithConversion("BooleanToHidden");

            set.Bind(lbReStart).To(vm => vm.StartTime).WithConversion("DateTimeToString", "Reservation");
            set.Bind(lbReEnd).To(vm => vm.EndTime).WithConversion("DateTimeToString", "Reservation");
            set.Bind(lbAddress).To(vm => vm.Reservation.Parking.Location);
            set.Bind(lbEndTime).To(vm => vm.EndTime).WithConversion("DateTimeToString", "Time");
            set.Bind(lbReAddress).To(vm => vm.Reservation.Parking.Location);
            set.Bind(lbReMoney).To(vm => vm.Reservation.Cost).WithConversion("MoneyString");
            set.Bind(lbNumberVehicle).To(vm => vm.Reservation.PlateNumber);
            set.Bind(lbTypeVehicle).To(vm => vm.Reservation.VehicleType);

            #region Language Binding

            set.Bind(lbAddr).To(vm => vm.TextSource).WithConversion("Language", "AddressText");
            set.Bind(lbReAddr).To(vm => vm.TextSource).WithConversion("Language", "AddressText");
            set.Bind(lbReStarttime).To(vm => vm.TextSource).WithConversion("Language", "StartTimeText");
            set.Bind(lbEndtime).To(vm => vm.TextSource).WithConversion("Language", "EndTimeText");
            set.Bind(lbReEndtime).To(vm => vm.TextSource).WithConversion("Language", "EndTimeText");
            set.Bind(lbReCost).To(vm => vm.TextSource).WithConversion("Language", "CostText");
            set.Bind(lbVehicle).To(vm => vm.TextSource).WithConversion("Language", "VehicleText");

            set.Bind(lbNavigate).To(vm => vm.TextSource).WithConversion("Language", "NavigateText");
            set.Bind(lbLeave).To(vm => vm.TextSource).WithConversion("Language", "LeaveText");
            set.Bind(lbLeaveDisabled).To(vm => vm.TextSource).WithConversion("Language", "LeaveText");
            set.Bind(lbReport).To(vm => vm.TextSource).WithConversion("Language", "ReportText");
            set.Bind(lbReportDisabled).To(vm => vm.TextSource).WithConversion("Language", "ReportText");

            #endregion

            set.Apply();

            #region UI Settings

            //     cstBottomSpace.Constant = UIScreen.MainScreen.Bounds.Height / 8f;

            if (ViewModel.Status == ParkingStatus.Rented)
            {
                if (UIScreen.MainScreen.Bounds.Height > 480)
                    cstTopSpaceVehicle.Constant = 200f; //237
                else
                    cstTopSpaceVehicle.Constant = 170f; //207
                sclMain.ScrollEnabled = false;
            }
            else if (ViewModel.Status == ParkingStatus.Reserved)
            {
                if (UIScreen.MainScreen.Bounds.Height > 480)
                    cstTopSpaceVehicle.Constant = 270f; //327
                else
                    cstTopSpaceVehicle.Constant = 240f; //277
                sclMain.ScrollEnabled = true;
            }

            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {   
            base.ViewDidAppear(animated);
            if (!AppDelegate.IsPad)
            {
                cstHeightViewContent.Constant = vbtnLeave.Frame.Y + vbtnLeave.Frame.Height + 20;
            }

            ViewModel.RaisePropertyChanged("OfferedTime");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (locationManager == null)
            {
                locationManager = new CLLocationManager();
                locationManager.DesiredAccuracy = CLLocation.AccurracyBestForNavigation;
                locationManager.Delegate = this;
                if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (locationManager != null)
            {
                locationManager.Delegate = null;
                locationManager.Dispose();
                locationManager = null;
            }
            base.ViewDidDisappear(animated);
        }

        private void AdjustFont()
        {
            lbAddress.Font = FontHelper.AdjustFontSize(lbAddress.Font);
            lbEndTime.Font = FontHelper.AdjustFontSize(lbEndTime.Font);
            lbNumberVehicle.Font = FontHelper.AdjustFontSize(lbNumberVehicle.Font);
            lbTypeVehicle.Font = FontHelper.AdjustFontSize(lbTypeVehicle.Font);
        }

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public virtual void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            Console.WriteLine(status.ToString());
        }

        #region IParkingSummaryView implementation

        public T GetPreference<T>(string key)
        {
            var value = NSUserDefaults.StandardUserDefaults.StringForKey(key);
            return value != null ? (T)Convert.ChangeType(value, typeof(T)) : default(T);
        }

        public void NavigateUsingWaze(double lat, double lng, int zoomLevel = 1)
        {
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("waze://")))
            {
                // Waze is installed. Launch Waze and start navigation
                var urlString = string.Format("waze://?ll={0},{1}&navigate=yes&z={2}", lat.ParseToCultureInfo(new CultureInfo("en-US")), lng.ParseToCultureInfo(new CultureInfo("en-US")), zoomLevel);

                UIApplication.SharedApplication.OpenUrl(new NSUrl(urlString));
            }
            else
            {
                // Waze is not installed. Launch AppStore to install Waze app
                UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id323229106"));
            }

        }

        public void NavigateUsingNavmii(double lat, double lng)
        {
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("navfree://")))
            {
                // Navmii is installed. Launch Navmii and start navigation
                var urlString = string.Format("navfree://{0},{1}", lat.ParseToCultureInfo(new CultureInfo("en-US")), lng.ParseToCultureInfo(new CultureInfo("en-US")));

                UIApplication.SharedApplication.OpenUrl(new NSUrl(urlString));
            }
            else
            {
                // Navmii is not installed. Launch AppStore to install Navmii app
                UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id434365587"));
            }

        }

        public async void NavigateUsingGoogleMaps(double destinationLat, double destinationLng, int zoomLevel = 1, DirectionsMode directionsMode = DirectionsMode.Driving)
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
            {
                if (locationManager != null)
                {
                    // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                    if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                    {
                        locationManager.RequestWhenInUseAuthorization();
                    }
                    return;
                }
            }

            if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                string title = (CLLocationManager.Status == CLAuthorizationStatus.Denied) ? "Location services are off" : "Location Service is not enabled";
                string message = "To use Location Service you must turn on 'While using the app' in the Location Services Settings";

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "Cancel", "Settings");

                    alertView.Clicked += (object sender, UIButtonEventArgs e) =>
                    {
                        if (e.ButtonIndex == alertView.CancelButtonIndex) //cancel
                        {
                            //ViewModel.CloseViewModel();
                        }
                        else
                        {
                            //go to settings
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                        }
                    };

                    alertView.Dismissed += (sender, e) =>
                    {
                        alertView.Dispose();
                        alertView = null;
                    };

                    alertView.Show();
                }
                else
                {
                    // ios 7 only has two CLAuthorizationStatus : Denied and AuthorizedAlways
                    if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways)
                        return;

                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "OK");

                    alertView.Clicked += (sender, e) =>
                    {
                        //if (e.ButtonIndex == alertView.CancelButtonIndex) 
                        //ViewModel.CloseViewModel();
                    };

                    alertView.Dismissed += (sender, e) =>
                    {
                        alertView.Dispose();
                        alertView = null;
                    };

                    alertView.Show();
                }

                return;
            }
            
            //get current location
            Geolocator locator = new Geolocator(){ DesiredAccuracy = 100 };
            var location = await locator.GetPositionAsync(50000);

            Console.WriteLine("Position Status: {0}", location.Timestamp);
            Console.WriteLine("Position Latitude: {0}", location.Latitude);
            Console.WriteLine("Position Longitude: {0}", location.Longitude);

            var sourceLat = location.Latitude;
            var sourceLng = location.Longitude;

            var mode = directionsMode == DirectionsMode.Driving ? "driving" : "walking";

            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("comgooglemaps://")))
            {
                // GoogleMaps is installed. Launch GoogleMaps and start navigation
                var urlString = string.Format("comgooglemaps://?saddr={0},{1}&daddr={2},{3}&zoom={4}&directionsmode={5}", sourceLat.ParseToCultureInfo(new CultureInfo("en-US")), sourceLng.ParseToCultureInfo(new CultureInfo("en-US")), destinationLat.ParseToCultureInfo(new CultureInfo("en-US")), destinationLng.ParseToCultureInfo(new CultureInfo("en-US")), zoomLevel, mode);

                UIApplication.SharedApplication.OpenUrl(new NSUrl(urlString));
            }
            else
            {
                // GoogleMaps is not installed. Launch AppStore to install GoogleMaps app
                UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id585027354"));
            }
        }

        public async void NavigateUsingNativeMap(double destinationLat, double destinationLng, int zoomLevel = 1, DirectionsMode directionsMode = DirectionsMode.Driving)
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined || CLLocationManager.Status == CLAuthorizationStatus.Denied)
                return;

            //get current location
            Geolocator locator = new Geolocator(){ DesiredAccuracy = 100 };
            var location = await locator.GetPositionAsync(50000);

            Console.WriteLine("Position Status: {0}", location.Timestamp);
            Console.WriteLine("Position Latitude: {0}", location.Latitude);
            Console.WriteLine("Position Longitude: {0}", location.Longitude);

            var sourceLat = location.Latitude;
            var sourceLng = location.Longitude;

            var urlString = string.Format("http://maps.apple.com/?saddr={0},{1}&daddr={2},{3}", sourceLat.ParseToCultureInfo(new CultureInfo("en-US")), sourceLng.ParseToCultureInfo(new CultureInfo("en-US")), destinationLat.ParseToCultureInfo(new CultureInfo("en-US")), destinationLng.ParseToCultureInfo(new CultureInfo("en-US")));
            UIApplication.SharedApplication.OpenUrl(new NSUrl(urlString));
        }

        #endregion
    }
}

