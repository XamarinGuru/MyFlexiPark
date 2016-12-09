
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreLocation;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using FlexyPark.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotAccuracyView : BaseView, ICLLocationManagerDelegate
    {
        CLLocationManager locationManager;

        public AddSpotAccuracyView()
            : base("AddSpotAccuracyView", null)
        {
        }

        public new AddSpotAccuracyViewModel ViewModel
        {
            get{
                return base.ViewModel as AddSpotAccuracyViewModel;
            }
            set{
                base.ViewModel = value;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (locationManager == null)
            {
                locationManager = new CLLocationManager();
                locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
                locationManager.Delegate = this;

                // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")) ) 
                {
                    locationManager.RequestWhenInUseAuthorization();
                }

                locationManager.StartUpdatingLocation();
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            if(locationManager !=null)
            {
                locationManager.Delegate = null;
                locationManager.Dispose();
                locationManager = null;
            }
            base.ViewDidDisappear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddSpotAccuracyView, AddSpotAccuracyViewModel>();

            set.Bind(lbAccuracy).To(vm => vm.CurrentAccuracy).WithConversion("MeterDouble");
            set.Bind(lbGPSTarget).To(vm => vm.GPSTarget).WithConversion("MeterDouble");

            set.Bind(lbLat).To(vm => vm.CurrentLat).WithConversion("Coordinates", "Lat");
            set.Bind(lbLng).To(vm => vm.CurrentLng).WithConversion("Coordinates", "Lng");

            #region Language Binding

            set.Bind(lbPleaseWait).To(vm=>vm.TextSource).WithConversion("Language", "PleaseWaitText");
            set.Bind(lbCurrentGPSAccuracy).To(vm=>vm.TextSource).WithConversion("Language", "CurrentGPSAccuracyText");
            set.Bind(lbCurrentGPSCoor).To(vm=>vm.TextSource).WithConversion("Language", "CurrentGPSCoorText");
            set.Bind(lbGPSTargetAccuracy).To(vm=>vm.TextSource).WithConversion("Language", "GPSTargetText");
            set.Bind(lbIfYourDevice).To(vm=>vm.TextSource).WithConversion("Language", "IfYourText");

            #endregion

            set.Apply();
        }


        #region CLLocationManager Delegate

        [Export("locationManager:didUpdateLocations:")]
        public virtual void LocationsUpdated (CLLocationManager manager, CLLocation[] locations)
        {
            if(locations != null && locations.Length != 0)
            {
                Console.WriteLine("Position Status: {0}", locations[0].Timestamp);
                Console.WriteLine("Position Latitude: {0}", locations[0].Coordinate.Latitude);
                Console.WriteLine("Position Longitude: {0}", locations[0].Coordinate.Longitude);

                Console.WriteLine("Horizontal Accuracy : {0}", locations[0].HorizontalAccuracy);
                Console.WriteLine("Vertical Accuracy : {0}", locations[0].VerticalAccuracy);

                ViewModel.CurrentAccuracy = locations[0].HorizontalAccuracy;
                ViewModel.CurrentLat = locations[0].Coordinate.Latitude;
                ViewModel.CurrentLng = locations[0].Coordinate.Longitude;

                if (locations[0].HorizontalAccuracy <= AppConstants.DesiredAccuracy)
                {
                    Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
                    Mvx.Resolve<ICacheService>().CurrentLat = locations[0].Coordinate.Latitude;
                    Mvx.Resolve<ICacheService>().CurrentLng = locations[0].Coordinate.Longitude;
                    ViewModel.CloseViewModel();
                }
            }
        }

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public virtual void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
        {
            Console.WriteLine(status.ToString());
            if (status == CLAuthorizationStatus.NotDetermined)
            {
                if (locationManager != null)
                {
                    // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                    if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")) ) 
                    {
                        locationManager.RequestWhenInUseAuthorization();
                    }
                    return;
                }
            }

            if(status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.Denied)
            {
                string title = (status == CLAuthorizationStatus.Denied) ? "Location services are off" : "Location Service is not enabled";
                string message = "To use Location Service you must turn on 'While using the app' in the Location Services Settings";

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "Cancel", "Settings");

                    alertView.Clicked += (object sender, UIButtonEventArgs e) =>
                        {
                            if (e.ButtonIndex == alertView.CancelButtonIndex) //cancel
                            {
                                ViewModel.CloseViewModel();
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
                    if (status == CLAuthorizationStatus.AuthorizedAlways)
                        return;

                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "OK" );

                    alertView.Clicked += (sender, e) => 
                        {
                            if (e.ButtonIndex == alertView.CancelButtonIndex) 
                                ViewModel.CloseViewModel();
                        };

                    alertView.Dismissed += (sender, e) =>
                        {
                            alertView.Dispose();
                            alertView = null;
                        };

                    alertView.Show();
                }
            }
        }

        #endregion
    }
}

