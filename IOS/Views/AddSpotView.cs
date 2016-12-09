
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;
using FlexyPark.UI.Touch.Views.TableSource;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using FlexyPark.Core;
using CoreLocation;
using Xamarin.Geolocation;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Security.Cryptography;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotView : BaseView, IAddSpotView, ICLLocationManagerDelegate
    {
        private CLLocationManager locationManager;

        public AddSpotView()
            : base("AddSpotView", null)
        {
        }

        public new AddSpotViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddSpotViewModel;
            }
            set
            {
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

            if (ViewModel.Status != Mvx.Resolve<ICacheService>().NextStatus)
                ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
            if (ViewModel.Parking != null && Mvx.Resolve<ICacheService>().CreateParkingRequest != null && Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate != null)
            {
                ViewModel.Parking.HourlyRate = Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate;

            }

            if (ViewModel.Status == AddSpotStatus.GPS)
            {
                if (locationManager == null)
                {
                    locationManager = new CLLocationManager();
                    locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
                    locationManager.Delegate = this;

                    // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                    if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                    {
                        locationManager.RequestWhenInUseAuthorization();
                    }

                    locationManager.StartUpdatingLocation();
                }
            }

            if (ViewModel.IsEditMode)
            {
                if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Activation)
                {
                    return;
                }
                if (Mvx.Resolve<ICacheService>().NextStatus != AddSpotStatus.GotoSpot && Mvx.Resolve<ICacheService>().NextStatus != AddSpotStatus.Complete)
                    ViewModel.DoAddOrSaveParkingSpot();
                else if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Complete)
                    ViewModel.DoAddOrSaveParkingSpot();
            }
            else
            {
                if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.SpotCalendar)
                    ViewModel.DoAddOrSaveParkingSpot();
                else if (Mvx.Resolve<ICacheService>().NextStatus == AddSpotStatus.Complete)
                    ViewModel.SaveParkingSpot();
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



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            if (DeviceHelper.IsPad)
                HideBackButton();
            
            //SetTitle(ViewModel.IsEditMode ? ViewModel.TextSource.GetText("EditPageTitle") : ViewModel.TextSource.GetText("PageTitle"));
            //SetBackButtonTitle("Back");
            // Perform any additional setup after loading the view, typically from a nib.

            ViewModel.View = this;

            var set = this.CreateBindingSet<AddSpotView, AddSpotViewModel>();

            var source = new TaskTableSource(tableTasks, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Tasks);

            set.Bind(btnAddSpot).To(vm => vm.AddSpotCommand);
            set.Bind(btnAddSpot).For(v => v.Enabled).To(vm => vm.IsAddButtonEnabled);
            //set.Bind(btnAddSpot).For("Title").To(vm => vm.ButtonTitle);

            #region Language Binding


            #endregion

            set.Apply();

            tableTasks.Source = source;
            tableTasks.ReloadData();

            #region UI Settings

            #endregion
        }

        #region CLLocationManager Delegate

        [Export("locationManager:didUpdateLocations:")]
        public virtual void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            if (locations != null && locations.Length != 0 && ViewModel.Status == AddSpotStatus.GPS)
            {
                Console.WriteLine("Position Status: {0}", locations[0].Timestamp);
                Console.WriteLine("Position Latitude: {0}", locations[0].Coordinate.Latitude);
                Console.WriteLine("Position Longitude: {0}", locations[0].Coordinate.Longitude);

                Console.WriteLine("Horizontal Accuracy : {0}", locations[0].HorizontalAccuracy);
                Console.WriteLine("Vertical Accuracy : {0}", locations[0].VerticalAccuracy);

                Mvx.Resolve<ICacheService>().CurrentLat = locations[0].Coordinate.Latitude;
                Mvx.Resolve<ICacheService>().CurrentLng = locations[0].Coordinate.Longitude;

                Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);

                /*if (ViewModel.Status == AddSpotStatus.GPS)
                {
                    Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                    ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
                }
                else if (ViewModel.Status == AddSpotStatus.Accuracy && locations[0].HorizontalAccuracy <= AppConstants.DesiredAccuracy)
                {
                    Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
                    ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
                }*/
            }

            /*if(locations != null && locations.Length != 0 && (ViewModel.Status == AddSpotStatus.GPS || ViewModel.Status == AddSpotStatus.Accuracy))
            {
                Console.WriteLine("Position Status: {0}", locations[0].Timestamp);
                Console.WriteLine("Position Latitude: {0}", locations[0].Coordinate.Latitude);
                Console.WriteLine("Position Longitude: {0}", locations[0].Coordinate.Longitude);

                lbHorizontalAccuracy.Text = string.Format("Accuracy: {0}m - Lat {1:N4} - Lng {2:N4}", locations[0].HorizontalAccuracy, locations[0].Coordinate.Latitude, locations[0].Coordinate.Longitude);

                Mvx.Resolve<ICacheService>().CurrentLat = locations[0].Coordinate.Latitude;
                Mvx.Resolve<ICacheService>().CurrentLng = locations[0].Coordinate.Longitude;

                if (ViewModel.Status == AddSpotStatus.GPS)
                {
                    Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                    ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
                }
            }*/
        }

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public virtual void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            Console.WriteLine(status.ToString());
            if (status == CLAuthorizationStatus.NotDetermined)
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

            if (status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.Denied)
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

                    UIAlertView alertView = new UIAlertView(title, message, null, "OK");

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

        #region IAddSpotView implementation

        public void ReloadTasks()
        {
            tableTasks.ReloadData();
        }

        public void GetGPS()
        {
            /*//TODO : change DesiredAccuracy
            var locator = new Geolocator { DesiredAccuracy = 10 };

            var position = await locator.GetPositionAsync (50000);
            if (position != null)
            {
                Console.WriteLine("Position Status: {0}", position.Timestamp);
                Console.WriteLine("Position Latitude: {0}", position.Latitude);
                Console.WriteLine("Position Longitude: {0}", position.Longitude);

                Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Accuracy;
                ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);

                return true;
            }
            else
            {
                Console.WriteLine("Cannot get location");   
                return false;
            }
            //TODO : if we can retry here ???*/

            if (ViewModel.Status == AddSpotStatus.GPS)
            {
                if (locationManager == null)
                {
                    locationManager = new CLLocationManager();
                    locationManager.Delegate = this;
                }

                // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                {
                    locationManager.RequestWhenInUseAuthorization();
                }

                locationManager.StartUpdatingLocation();
            }
        }

        public bool DetectLocationService()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedWhenInUse)
                {
                    if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return false;
        }

        public void GetAccuracy()
        {
            /*await Task.Delay(2000);
            Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.SpotAddress;
            ViewModel.UpdateTasksStatus(Mvx.Resolve<ICacheService>().NextStatus);
            return true;*/
        }

        #endregion
    }
}

