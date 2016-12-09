
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using WYPopoverControllerBinding;
using CoreGraphics;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Services;
using CoreLocation;
using FlexyPark.Core.Messengers;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ParkingSearchView : BaseView, IParkingSearchView, ICLLocationManagerDelegate
    {
        private CLLocationManager locationManager;
        private bool mGotLocation= false ;
        public ParkingSearchView()
            : base("ParkingSearchView", null)
        {
        }

        public new ParkingSearchViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ParkingSearchViewModel;
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

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            mGotLocation = false;
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
			
            SetTitle("Search");
            // Perform any additional setup after loading the view, typically from a nib.

            ViewModel.View = this;

            var set = this.CreateBindingSet<ParkingSearchView, ParkingSearchViewModel>();

            set.Bind(lbTitle).To(vm => vm.SearchTitle);
            set.Bind(lbPlateNumber).To(vm => vm.PlateNumber);

            //set.Bind(btnChooseVehicle).To(vm => vm.GotoChooseVehicleCommand);

            set.Bind(vBooking).For(v => v.Hidden).To(vm => vm.SearchMode).WithConversion("SearchModeToBool", "AA");

            set.Bind(tfStreet).To(vm => vm.Street);
            set.Bind(tfStrNumber).To(vm => vm.StrNumber);

            set.Bind(tfStartDate).To(vm => vm.StartBookingDateTime).WithConversion("DateTimeToString", "Date");
            set.Bind(tfStartHour).To(vm => vm.StartBookingDateTime).WithConversion("DateTimeToString", "Time");

            set.Bind(btnStartDate).To(vm => vm.SelectStartDateCommand);
            set.Bind(btnStartHour).To(vm => vm.SelectStartHourCommand);

            set.Bind(tfEndDate).To(vm => vm.EndBookingDateTime).WithConversion("DateTimeToString", "Date");
            set.Bind(tfEndHour).To(vm => vm.EndBookingDateTime).WithConversion("DateTimeToString", "Time");

            set.Bind(btnCheck).For(v => v.Enabled).To(vm => vm.IsCheckVisible);
            set.Bind(btnCheck).For(v => v.UserInteractionEnabled).To(vm => vm.IsCheckVisible);
            set.Bind(btnCheck).To(vm => vm.CheckPlacesCommand);

            var source = new PlaceTableSource(tablePlaces, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Places);

            #region Language Binding

            set.Bind(lbStartDate).To(vm => vm.TextSource).WithConversion("Language", "StartDateText");
            set.Bind(lbStartHour).To(vm => vm.TextSource).WithConversion("Language", "StartHourText");
            set.Bind(lbEndDate).To(vm => vm.TextSource).WithConversion("Language", "EndDateText");
            set.Bind(lbEndHour).To(vm => vm.TextSource).WithConversion("Language", "EndHourText");
            set.Bind(lbLocation).To(vm => vm.TextSource).WithConversion("Language", "LocationText");
            set.Bind (lbStreet).To (vm => vm.TextSource).WithConversion ("Language", "StreetText");

            #endregion

            set.Apply();

            tablePlaces.Source = source;
            tablePlaces.ReloadData();


            #region UI Settings

            if(ViewModel.SearchMode == FlexyPark.Core.SearchMode.Now)
                cstContentHeight.Constant = 320f;
            else
                cstContentHeight.Constant = 635f;
                    
            #endregion

            ViewModel.RaisePropertyChanged("IsCheckVisible");
        }

        #region DateTime Picker

        WYPopoverController popoverPicker;
        UIViewController pickerView;
        UIDatePicker picker;
        bool isStart;

        void InitPicker (bool isDate, bool _isStart)
        {
            isStart = _isStart;

            if (pickerView == null)
            {
                pickerView = new UIViewController();
                pickerView.View.Frame = new CGRect(0, 0, 320, 200);
            }

            if (picker == null)
            {
                picker = new UIDatePicker();
                pickerView.View.AddSubview (picker);

                picker.ValueChanged += (sender, e) => {
                    if(isStart)
                    {
                        int compare = picker.Date.NSDateToDateTime().CompareTo(ViewModel.EndBookingDateTime);
                        if(compare == 1)
                            ViewModel.EndBookingDateTime = picker.Date.NSDateToDateTime();

                        ViewModel.StartBookingDateTime = picker.Date.NSDateToDateTime();
                    }
                    else
                        ViewModel.EndBookingDateTime = picker.Date.NSDateToDateTime();
                };
            }

            //set time format to 24h format
            picker.Locale = isDate ? new NSLocale("US") : new NSLocale("UK");

            picker.Mode = isDate ? UIDatePickerMode.Date : UIDatePickerMode.Time;

            if (popoverPicker == null)
            {
                popoverPicker = new WYPopoverController(pickerView);
                popoverPicker.PopoverContentSize = pickerView.View.Frame.Size;
            }
        }

        #endregion

        #region IParkingSearchView implementation

        public void ShowDatePicker(bool isStart)
        {
            InitPicker(true, isStart);

            // set default value
            picker.Date = isStart ? ViewModel.StartBookingDateTime.DateTimeToNSDate() : ViewModel.EndBookingDateTime.DateTimeToNSDate();
            if(isStart)
                popoverPicker.PresentPopoverFromRect (btnStartDate.Frame, vStartDate, WYPopoverArrowDirection.Up, true);
//            else
//                popoverPicker.PresentPopoverFromRect (btnEndDate.Frame, vEndDate, WYPopoverArrowDirection.Up, true);
        }

        public void ShowTimePicker(bool isStart)
        {
            InitPicker(false, isStart);

            // set default value
            picker.Date = isStart ? ViewModel.StartBookingDateTime.DateTimeToNSDate() : ViewModel.EndBookingDateTime.DateTimeToNSDate();
            if(isStart)
                popoverPicker.PresentPopoverFromRect (btnStartHour.Frame, vStartDate, WYPopoverArrowDirection.Up, true);
//            else
//                popoverPicker.PresentPopoverFromRect (btnEndHour.Frame, vEndDate, WYPopoverArrowDirection.Up, true);
        }

        public void StartGetLocation()
        {
            if (locationManager == null)
            {
                locationManager = new CLLocationManager();
                locationManager.DesiredAccuracy = CLLocation.AccuracyHundredMeters;
                locationManager.Delegate = this;
                if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }

            locationManager.StartUpdatingLocation();
        }

        #endregion

        #region CLLocationManager Delegate

        [Export("locationManager:didUpdateLocations:")]
        public virtual void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            if (locations != null && locations.Length != 0)
            {
                if (mGotLocation)
                    return;

                Console.WriteLine("Position Status: {0}", locations[0].Timestamp);
                Console.WriteLine("Position Latitude: {0}", locations[0].Coordinate.Latitude);
                Console.WriteLine("Position Longitude: {0}", locations[0].Coordinate.Longitude);

                Console.WriteLine("Horizontal Accuracy : {0}", locations[0].HorizontalAccuracy);
                Console.WriteLine("Vertical Accuracy : {0}", locations[0].VerticalAccuracy);

                mGotLocation = true;
                Mvx.Resolve<ICacheService>().CurrentLat = locations[0].Coordinate.Latitude;
                Mvx.Resolve<ICacheService>().CurrentLng = locations[0].Coordinate.Longitude;

                ViewModel.CheckPlacesCommand.Execute();

                locationManager.StopUpdatingLocation();
            }

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
    }
}

