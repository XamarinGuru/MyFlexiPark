
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;
using System.Collections.Generic;
using Cirrious.MvvmCross.Binding.ValueConverters;
using WYPopoverControllerBinding;
using CoreGraphics;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Xamarin.Geolocation;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using System.Diagnostics;
using CoreLocation;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Timers;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ParkingListsView : BaseView, IParkingListsView,  ICLLocationManagerDelegate
    {
        private CLLocationManager locationManager;
        private bool mGotLocation;
        private Timer mTimer;
        private Timer m60sTimer;
        private int mCount;

        public ParkingListsView()
            : base("ParkingListsView", null)
        {
        }

        public new ParkingListsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ParkingListsViewModel;
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
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			ViewModel.Title = "Acquiring location…";
			if (locationManager == null)
			{
				locationManager = new CLLocationManager();
				locationManager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters;
				locationManager.Delegate = this;
				if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
				{
					locationManager.RequestWhenInUseAuthorization();
				}
			}

			StartTimer();
			if (Mvx.Resolve<ICacheService>().SearchMode == SearchMode.Now) //park me now
			{
				Start60sTimer();
				locationManager.StartUpdatingLocation();
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, true, string.Empty, true));
			}
			else //park me later
			{
				ViewModel.GetParkingLists();
			}

			ViewModel.UpdateValidTime();
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

            ViewModel.View = this;
//            if (Mvx.Resolve<ICacheService>().SearchMode == SearchMode.Now)
//                SetTitle("Acquiring location...");
//            else
//                SetTitle("Please choose");

            if (DeviceHelper.IsPad)
                HideBackButton();
            //SetTitle("Please choose");
            SetBackButtonTitle("Back");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<ParkingListsView, ParkingListsViewModel>();

            var source = new ParkingSpotTableSource(tableParkingSlot, this);
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.ParkingSlots);

            set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.Title);

            set.Bind(tableParkingSlot).For(v => v.Hidden).To(vm => vm.IsShowParkingLists).WithConversion("BooleanToHidden");

            set.Bind(lbValidTime).To(vm => vm.ValidTime).WithConversion("ValidTime");

            set.Bind(sdTime).For(s => s.Value).To(vm => vm.ParkingTime);
            set.Bind(sdTime).For("ValueChanged").To(vm => vm.HandleValueChangedCommand);

            set.Bind(vEndDate).For(v => v.Hidden).To(vm => vm.IsShowEndBookingDate).WithConversion("BooleanToHidden");
            set.Bind(lbValidTime).For(v => v.Hidden).To(vm => vm.IsShowEndBookingDate);
            set.Bind(lbValidUntil).For(v => v.Hidden).To(vm => vm.IsShowEndBookingDate);

            set.Bind(tfEndDate).To(vm => vm.EndBookingDateTime).WithConversion("DateTimeToString", "Date");
            set.Bind(tfEndHour).To(vm => vm.EndBookingDateTime).WithConversion("DateTimeToString", "Time");

            set.Bind(btnEndDate).To(vm => vm.SelectEndDateCommand);
            set.Bind(btnEndHour).To(vm => vm.SelectEndHourCommand);

            #region Language Binding

            set.Bind(lbValidUntil).To(vm => vm.TextSource).WithConversion("Language", "ValidUntilText");
            set.Bind(lbLonger).To(vm => vm.TextSource).WithConversion("Language", "LongerText");
            set.Bind(lbEndDate).To(vm => vm.TextSource).WithConversion("Language", "EndDateText");
            set.Bind(lbEndHour).To(vm => vm.TextSource).WithConversion("Language", "EndHourText");

            #endregion

            set.Apply();

            tableParkingSlot.Source = source;
            tableParkingSlot.ReloadData();

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            mGotLocation = false;
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
        }

        #region DateTime Picker

        WYPopoverController popoverPicker;
        UIViewController pickerView;
        UIDatePicker picker;

        void InitPicker(bool isDate)
        {
            if (pickerView == null)
            {
                pickerView = new UIViewController();
                pickerView.View.Frame = new CGRect(0, 0, 320, 200);
            }

            if (picker == null)
            {
                picker = new UIDatePicker();
                pickerView.View.AddSubview(picker);

                picker.ValueChanged += (sender, e) =>
                {
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


        #region CLLocationManager Delegate

        [Export("locationManager:didUpdateLocations:")]
        public virtual void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            if (locations != null && locations.Length != 0)
            {
                if (mGotLocation)
                    return;

                if (ViewModel.Latitude != 0 && ViewModel.Longitude != 0)
                    return;
                
                Debug.WriteLine(mCount);
                if (mCount < 5)
                {
                    mCount++;
                    return;
                }
                
                Console.WriteLine("Position Status: {0}", locations[0].Timestamp);
                Console.WriteLine("Position Latitude: {0}", locations[0].Coordinate.Latitude);
                Console.WriteLine("Position Longitude: {0}", locations[0].Coordinate.Longitude);

                Console.WriteLine("Horizontal Accuracy : {0}", locations[0].HorizontalAccuracy);
                Console.WriteLine("Vertical Accuracy : {0}", locations[0].VerticalAccuracy);

                if (locations[0].HorizontalAccuracy <= 50)
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
                    mGotLocation = true;
                    mCount = 0;
                    Mvx.Resolve<ICacheService>().CurrentLat = locations[0].Coordinate.Latitude;
                    Mvx.Resolve<ICacheService>().CurrentLng = locations[0].Coordinate.Longitude;

                    locationManager.StopUpdatingLocation();
                    ViewModel.GetParkingLists();
                }
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


        #region IParkingListsView implementation

        public void SetSliderValue()
        {
            sdTime.SetValue((float)Math.Round(sdTime.Value), true);

        }

        public void ShowDatePicker()
        {
            InitPicker(true);

            // set default value
            picker.Date = ViewModel.EndBookingDateTime.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnEndDate.Frame, vEndDate, WYPopoverArrowDirection.Up, true);
        }

        public void ShowTimePicker()
        {
            InitPicker(false);

            // set default value
            picker.Date = ViewModel.EndBookingDateTime.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnEndHour.Frame, vEndDate, WYPopoverArrowDirection.Up, true);
        }

        public void ResetHeight()
        {
            cstVerticalTableToValidUntil.Constant = ViewModel.ParkingTime == 4 ? 60f : 10f;
        }

        public void StartTimer()
        {
            if (mTimer == null)
            {
                mTimer = new Timer(60000);
                mTimer.Elapsed += MTimer_Elapsed;
                mTimer.Start();
            }
        }

        void MTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ViewModel.StartTimeStamp != 0) //book later
                ViewModel.StartTimeStamp += 60;
            ViewModel.UpdateValidTime();
        }

        public void StopTimer()
        {
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Elapsed -= MTimer_Elapsed;
                mTimer.Dispose();
                mTimer = null;
            }
        }

        public void Start60sTimer()
        {
            if (m60sTimer == null)
            {
                m60sTimer = new Timer(1000);
                m60sTimer.Elapsed += M60sTimer_Elapsed;
                m60sTimer.Start();
            }
        }

        void M60sTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ViewModel.Counter++;
            if (ViewModel.Counter == 60)
            {
                //set true, prevent calling DidUpdateLocation when showing popup
                mGotLocation = true;

                ViewModel.ShowWeakSignalGPS();
            }
            
        }

        public void Stop60sTimer()
        {
            if (m60sTimer != null)
            {
                m60sTimer.Stop();
                m60sTimer.Elapsed -= M60sTimer_Elapsed;
                m60sTimer.Dispose();
                m60sTimer = null;
            }
        }

        #endregion

    }


}

