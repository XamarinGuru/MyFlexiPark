
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.UI.Touch.Views.Base;
using CoreGraphics;
using System.Timers;
using FlexyPark.Core;
using AudioToolbox;
using Cirrious.CrossCore.Touch.Views;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.Helpers;


namespace FlexyPark.UI.Touch.Views
{
	public partial class ParkingReservedView : BaseTabBarView, IParkingReservedView, IUITabBarControllerDelegate
	{
		public Timer timer;
		SystemSound systemSound;
		NSUrl url;

		Timer buzzTimer;

		UIBarButtonItem btnBarMenu, btnBarExtend, btnBarStart, btnBarOverviewResume;

        MvxSubscriptionToken mTextSourceToken;

		public ParkingReservedView ()
		{
			ViewDidLoad ();
		}

		public new ParkingReservedViewModel ViewModel {
			get {
				return base.ViewModel as ParkingReservedViewModel;
			}
			set {
				base.ViewModel = value;
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
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

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			//change tab bar height
            /*CGRect rect = TabBar.Frame;
			rect.Y = rect.Y + (rect.Height - UIScreen.MainScreen.Bounds.Height / 8f);
			rect.Height = UIScreen.MainScreen.Bounds.Height / 8f;//
			TabBar.Frame = rect;*/


            //10-13-2015 : customer wants to remove the tabs
            TabBar.Hidden = true;
            TabBar.RemoveFromSuperview();
//            TabBar.BackgroundColor = UIColor.White;

//            TabBar.Frame = new CGRect(0, UIScreen.MainScreen.Bounds.Height, TabBar.Frame.Width, TabBar.Frame.Height);
//            TabBar.AccessibilityFrame = new CGRect(0, 0, TabBar.Frame.Width, TabBar.Frame.Height);

        }

		public override void ViewWillAppear (bool animated)
		{
            base.ViewWillAppear(animated);

//            this.AutomaticallyAdjustsScrollViewInsets = false;
//            this.View.AutosizesSubviews = false;
//            this.AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers = false;

//            this.View.Subviews[1].Hidden = true;
//            this.View.Subviews[0].Frame = UIScreen.MainScreen.Bounds;
//            this.View.BringSubviewToFront(this.View.Subviews[0]);

            if(Mvx.Resolve<ICacheService>().ExtendHours != 0)
            {
                if(ViewModel.SummaryVM != null)
                {
                    ViewModel.SummaryVM.EndTime = ViewModel.SummaryVM.EndTime.AddHours(Mvx.Resolve<ICacheService>().ExtendHours);
                    ViewModel.SummaryVM.Reservation.EndTimestamp = ViewModel.SummaryVM.EndTime.DateTimeToTimeStamp().ToString();
                    //ViewModel.SummaryVM.Reservation.EndTimestamp = (long.Parse(ViewModel.SummaryVM.Reservation.EndTimestamp) + (3600 * Mvx.Resolve<ICacheService>().ExtendHours)).ToString();
                    ViewModel.TotalParkingTime = ViewModel.TotalParkingTime + (int)(3600 * Mvx.Resolve<ICacheService>().ExtendHours);
                    var bookingExpiredTime = Mvx.Resolve<IPlatformService>().GetPreference<long>(AppConstants.BookingExpiredTime);
                    Mvx.Resolve<IPlatformService>().SetPreference<long>(AppConstants.BookingExpiredTime, bookingExpiredTime + (int)(3600 * Mvx.Resolve<ICacheService>().ExtendHours));
                }

                Mvx.Resolve<ICacheService>().ExtendHours = 0;
            }

            //ViewModel.CheckTotalParkingTime();

			if (ViewModel.Status == ParkingStatus.Rented) {
				if (timer == null) {
					timer = new Timer (1000);
					timer.Elapsed += Timer_Elapsed;
					timer.Start ();
				}
			}

            ViewModel.RaisePropertyChanged("TextSource");
            ViewModel.RaisePropertyChanged("SharedTextSource");
            SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));

            mTextSourceToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TextSourceMessage>((TextSourceMessage message) =>
                {
                    ViewModel.RaisePropertyChanged("TextSource");
                    ViewModel.RaisePropertyChanged("SharedTextSource");
                    SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));
                });

            btnBarOverviewResume.Title = ViewModel.TextSource.GetText("OverviewText");
            btnBarStart.Title  = ViewModel.TextSource.GetText("StartText");
		}

		void Timer_Elapsed (object sender, ElapsedEventArgs e)
		{
			ViewModel.TotalParkingTime--;
		}

		public override void ViewWillDisappear (bool animated)
		{
			if (ViewModel.NeedRelease) 
            {
				if (timer != null) {
					timer.Stop ();
					timer.Elapsed -= Timer_Elapsed;
					timer.Dispose ();
					timer = null;
				}
			}

			base.ViewWillDisappear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (ViewModel == null)
				return;

			if (ViewModel.Status == ParkingStatus.Reserved) {
				SetTitle ("06/07");
			} else {
				SetTitle ("2:00:07");
			}
			
			btnBarExtend = new UIBarButtonItem (){ Image = UIImage.FromFile ("white_icon_clock_22.png")  };
			NavigationItem.RightBarButtonItem = btnBarExtend;

			btnBarMenu = new UIBarButtonItem (){ Image = UIImage.FromFile ("white_icon_menu_22.png")  };

            btnBarStart = new UIBarButtonItem (){ Title = ViewModel.TextSource.GetText("StartText") };
            btnBarOverviewResume = new UIBarButtonItem (){ Title = ViewModel.TextSource.GetText("OverviewText") };

			if (ViewModel.Status == ParkingStatus.Rented) {
				HideBackButton ();
				NavigationItem.LeftBarButtonItem = btnBarMenu;
			}

			ViewModel.View = this;
            ViewModel.BaseView = this;
			// Perform any additional setup after loading the view, typically from a nib.

			//tabbar background color
			TabBar.BarTintColor = UIColor.FromRGB (31, 31, 31);

			ViewControllers = new UIViewController[] {
                CreateTabFor (0, ViewModel.TextSource.GetText("SummaryText"), "icon_document_30.png", "", ViewModel.SummaryVM),
                //CreateTabFor (1, ViewModel.TextSource.GetText("MapText"), "icon_location_30.png", "", ViewModel.MapVM)
			};

			CustomizableViewControllers = new UIViewController[] { };
			SelectedViewController = ViewControllers [0];

			var set = this.CreateBindingSet<ParkingReservedView,ParkingReservedViewModel> ();
			set.Bind (btnBarExtend).To (vm => vm.GotoExtendParkingTimeCommand);
			set.Bind (btnBarMenu).To (vm => vm.GotoMenuCommand);

			set.Bind (btnBarOverviewResume).For (b => b.Title).To (vm => vm.OverviewResumeTitle);
			set.Bind (btnBarOverviewResume).To (vm => vm.ChangeNavigationModeCommand);
			set.Bind (btnBarStart).To (vm => vm.StartNavigationCommand);

            if (ViewModel.Status == ParkingStatus.Rented)
                set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.TotalParkingTime).WithConversion("ParkingTimer");
            else
            {
                set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.SummaryVM.StartTime).WithConversion("DateTimeToString", "Date");
            }

            #region Language Binding

            #endregion

			set.Apply ();

			#region UI Settings

			this.Delegate = this;

			#endregion
		}



		#region IParkingReservedView implementation

		public void StopTimer ()
		{
			if (timer != null) {
				timer.Stop ();
				timer.Elapsed -= Timer_Elapsed;
				timer.Dispose ();
				timer = null;
			}
		}

		public void ShowMapTab ()
		{
			SelectedViewController = ViewControllers [1];
            ResetBarButton(false);
		}

		void BuzzTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			// Generate the NSUrl to the sound file
			if (url == null)
				url = NSUrl.FromFilename ("door_bell.wav");

			// Generate the SystemSound instance with the NSUrl
			if (systemSound == null)
				systemSound = new SystemSound (url);

			systemSound.PlayAlertSound ();
		}

		public void Buzzing (bool isStart)
		{
			if (isStart) {
				if (buzzTimer == null) {
					buzzTimer = new Timer (2000);
					buzzTimer.Elapsed += BuzzTimer_Elapsed;
					buzzTimer.Start ();
				}
			} else {
				if (buzzTimer != null) {
					buzzTimer.Stop ();
					buzzTimer.Elapsed -= BuzzTimer_Elapsed;
					buzzTimer.Dispose ();
					buzzTimer = null;
				}
			}
		}

		public void ChangeBarButton (bool isNavigating)
		{
			ResetBarButton (!isNavigating);
		}

		#endregion


		#region UITabBarControllerDelegate

		[Foundation.Export ("tabBarController:didSelectViewController:")]
		public virtual void ViewControllerSelected (UITabBarController tabBarController, UIViewController viewController)
		{
			if (viewController is ParkingMapView) {
				Mvx.Resolve<IMvxMessenger> ().Publish (new NavigateMapMessage (this.ViewModel, 10.793947, 106.65862600000003, 10.768811, 106.63786500000003));
				//if (ViewModel.MapVM.IsNavigating)
					ResetBarButton (false);
			} else if (viewController is ParkingSummaryView) {				
				ResetBarButton (true);
			}
		}

		#endregion

		private void ResetBarButton (bool isParkingSummaryView)
		{
			if (isParkingSummaryView) {
				NavigationItem.RightBarButtonItem = btnBarExtend;
				if (ViewModel.Status == ParkingStatus.Rented) {
                    NavigationItem.SetHidesBackButton(true,false);
					NavigationItem.LeftBarButtonItem = btnBarMenu;
				}
			} else {
                if (ViewModel.MapVM.HasStaredNavigation)
                    NavigationItem.RightBarButtonItem = btnBarOverviewResume;
                else
                    NavigationItem.RightBarButtonItem = btnBarStart;
                
                NavigationItem.LeftBarButtonItem = null;
                NavigationItem.SetHidesBackButton(false,true);
			}
		}
	}
}

