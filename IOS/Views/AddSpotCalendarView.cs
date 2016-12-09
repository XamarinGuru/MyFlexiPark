
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core;
using System.Collections.ObjectModel;
using FlexyPark.UI.Touch.Views.TableSource;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Linq;
using FlexyPark.UI.Touch.Services;
using FlexyPark.Core.Services;
using CoreGraphics;
using System.Collections.Generic;
using UICalendar;
using EventKit;
using System.Threading.Tasks;
using TweetStation;

namespace FlexyPark.UI.Touch.Views
{
	public partial class AddSpotCalendarView : BaseView, IAddSpotCalendarView
	{
		UIBarButtonItem btnBarEdit;

		EventTableSource source;

		RotatingCalendarView mCalendarView;

		public AddSpotCalendarView()
			: base("AddSpotCalendarView", null)
		{
		}

		public new AddSpotCalendarViewModel ViewModel
		{
			get
			{
				return base.ViewModel as AddSpotCalendarViewModel;
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

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			if (mCalendarView != null)
			{
				mCalendarView.Dispose();
				mCalendarView = null;
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

            ViewModel.GetUnavaiabilities(Core.Helpers.DateTimeHelpers.GetFirstDayOfMonthAndYear ( DateTime.UtcNow.Year, DateTime.UtcNow.Month));

			NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, ReGetUnavaiabilities);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			//TODO : Comment out if need
			Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Activation;

			NSNotificationCenter.DefaultCenter.RemoveObserver(UIApplication.DidBecomeActiveNotification);
		}

		private void ReGetUnavaiabilities(NSNotification notify)
		{
			Console.WriteLine("Re get unavaiabilities");
            ViewModel.GetUnavaiabilities(Core.Helpers.DateTimeHelpers.GetFirstDayOfMonthAndYear (DateTime.UtcNow.Year , DateTime.UtcNow.Month));
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			SetTitle(ViewModel.TextSource.GetText("PageTitle"));

			ViewModel.View = this;

			ViewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName.Equals("SelectedEvents") || e.PropertyName.Equals("Events"))
					tableEvents.ReloadData();
			};
			// Perform any additional setup after loading the view, typically from a nib.

			//            btnBarEdit = new UIBarButtonItem(){ Title = "Edit" };
			//            NavigationItem.RightBarButtonItem = btnBarEdit;

			var set = this.CreateBindingSet<AddSpotCalendarView, AddSpotCalendarViewModel>();
			set.Bind(btnEdit).To(vm => vm.SwitchModeCommand);

			set.Bind(btnDone).To(vm => vm.DoneCommand);
			set.Bind(btnAdd).To(vm => vm.AddNewEventCommand);

			set.Bind(lbDate).To(vm => vm.SelectedDate).WithConversion("DateTimeToString", "Date");

			source = new EventTableSource(tableEvents, this);
			set.Bind(source).To(vm => vm.Events);

			#region Language Binding

			set.Bind(btnDone).For("Title").To(vm => vm.TextSource).WithConversion("Language", "DoneText");

			#endregion

			set.Apply();

			tableEvents.Source = source;
			tableEvents.ReloadData();



		}


		#region IAddSpotCalendarView implementation

		public void SetModeTitle(string title)
		{
			InvokeOnMainThread(() =>
				{
					btnEdit.SetTitle(title, UIControlState.Normal);
					tableEvents.ReloadData();
				});
		}

		public void ReloadTable()
		{
			InvokeOnMainThread(() =>
				{
					tableEvents.ReloadData();
				});
		}

		public async void UpdateCalendar(bool forceReload = false)
		{
			await System.Threading.Tasks.Task.Delay(300);
            if (mCalendarView != null && forceReload)
			{
				CalendarDayTimelineView.monthEvents.Clear();
				if (ViewModel.Events != null && ViewModel.Events.Count > 0)
				{

					foreach (var _eventVM in ViewModel.Events)
					{
						if (_eventVM.Unavaiability != null)
							CalendarDayTimelineView.monthEvents.Add(new CalendarDayEventView(_eventVM.Unavaiability));
						else if (_eventVM.Booking != null)
							CalendarDayTimelineView.monthEvents.Add(new CalendarDayEventView(_eventVM.Booking));
					}
				}

				//HACK : To force the calendar to refresh after adding events
				//mCalendarView.SingleDayView.ShouldRefreshUI = true;
				//mCalendarView.SingleDayView.EventsNeedRefresh = false;
				mCalendarView.SingleDayView.ForceReloadView(true, false);
				return;
			}

			if (mCalendarView == null)
			{
				mCalendarView = new RotatingCalendarView(vCalendar.Frame);
				var rect = mCalendarView.View.Frame;
				rect.Y = 0;
				mCalendarView.View.Frame = new CGRect(0, 30, vCalendar.Frame.Width, vCalendar.Frame.Height + 40);

				mCalendarView.View.BackgroundColor = UIColor.Clear;

                mCalendarView.SingleDayView.dateChanged += (DateTime newDate) => 
                {
                    ViewModel.GetUnavaiabilities (Core.Helpers.DateTimeHelpers.GetFirstDayOfMonthAndYear (newDate.Year, newDate.Month));
                };

				CalendarDayTimelineView.monthEvents.Clear();
				if (ViewModel.Events != null && ViewModel.Events.Count > 0)
				{
					foreach (var _eventVM in ViewModel.Events)
					{
						if (_eventVM.Unavaiability != null)
							CalendarDayTimelineView.monthEvents.Add(new CalendarDayEventView(_eventVM.Unavaiability));
						else if (_eventVM.Booking != null)
							CalendarDayTimelineView.monthEvents.Add(new CalendarDayEventView(_eventVM.Booking));
					}
				}

				//HACK : To force the calendar to refresh after adding events
				//mCalendarView.SingleDayView.ShouldRefreshUI = true;
				//mCalendarView.SingleDayView.EventsNeedRefresh = false;
				mCalendarView.SingleDayView.ForceReloadView(true, false);

				mCalendarView.SingleDayView.OnEventClicked += (CalendarDayEventView theEvent) =>
				{
					Console.WriteLine(theEvent.Title);
					if (theEvent._Event != null)
					{
						var eventVM = ViewModel.Events.FirstOrDefault(x => x.Unavaiability != null && x.Unavaiability.Equals(theEvent._Event));
						if (eventVM != null)
							ViewModel.EventItemClickedCommand.Execute(eventVM);
					}
					else if (theEvent._Booking != null)
					{
						var eventVM = ViewModel.Events.FirstOrDefault(x => x.Booking != null && x.Booking.Equals(theEvent._Booking));
						if (eventVM != null)
							ViewModel.EventItemClickedCommand.Execute(eventVM);
					}
				};
				mCalendarView.SingleDayView.OnEditEventClicked += (CalendarDayEventView theEvent) =>
				{
					var eventVM = ViewModel.Events.FirstOrDefault(x => x.Unavaiability != null && x.Unavaiability.Equals(theEvent._Event));
					if (eventVM != null)
						ViewModel.EventItemClickedCommand.Execute(eventVM);
				};
				mCalendarView.SingleDayView.OnDeleteEventClicked += (CalendarDayEventView theEvent) =>
				{
					var eventVM = ViewModel.Events.FirstOrDefault(x => x.Unavaiability != null && x.Unavaiability.Equals(theEvent._Event));
					if (eventVM != null)
						ViewModel.DeleteEventCommand.Execute(eventVM);
				};

			}

			View.AddSubview(mCalendarView.View);
			if (ViewModel.SelectedEvent != null && mCalendarView != null && mCalendarView.SingleDayView != null)
			{
				mCalendarView.SingleDayView.SetDate(new DateTime(ViewModel.SelectedEvent.StartDate.Year, ViewModel.SelectedEvent.StartDate.Month, ViewModel.SelectedEvent.StartDate.Day, 0, 0, 0, 0, DateTimeKind.Local));
			}

			if (ViewModel.SelectedEvent != null && mCalendarView != null && CalendarDayTimelineView.calMonthView != null && CalendarDayTimelineView.calMonthView._monthGridView != null)
			{
				CalendarDayTimelineView.calMonthView._monthGridView.SelectedDate = new DateTime(ViewModel.SelectedEvent.StartDate.Year, ViewModel.SelectedEvent.StartDate.Month, ViewModel.SelectedEvent.StartDate.Day, 0, 0, 0, 0, DateTimeKind.Local);
				CalendarDayTimelineView.calMonthView._monthGridView.BuildGrid();

			}
			ViewModel.SelectedEvent = null;


			//make sure btnAdd is in front 
			View.BringSubviewToFront(btnAdd);

            mCalendarView.View.SizeToFit ();
            //mCalendarView.View.ClipsToBounds = true;
		}

        public override void TouchesEnded (Foundation.NSSet touches, UIKit.UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
        }

		public void HideFirstItem()
		{
		}

		public void GotoToday()
		{
		}

		#endregion
	}
}

