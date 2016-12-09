
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ExtendParkingTimeView : BaseView
    {
        private MvxSubscriptionToken mTimeToken;

        public ExtendParkingTimeView()
            : base("ExtendParkingTimeView", null)
        {
        }

        public new ExtendParkingTimeViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ExtendParkingTimeViewModel;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            SetTitle("Extend");
            // Perform any additional setup after loading the view, typically from a nib.

            mTimeToken = Mvx.Resolve<IMvxMessenger>().Subscribe<TimeMessage>(message => {
                if(message.Sender.GetType() != typeof(ParkingReservedViewModel))
                    return;

                ViewModel.TotalParkingTime = message.TimeLeft;
            });

            var label = new UILabel(new CoreGraphics.CGRect(0, 0, 70, 30)){ Text = "aaaaa" };
            label.TextAlignment = UITextAlignment.Center;
            label.TextColor = UIColor.White;

            if (ViewModel.TotalParkingTime != 0)
            {
                var btnBarTime = new UIBarButtonItem(label);
                NavigationItem.RightBarButtonItem = btnBarTime;
            }

            var set = this.CreateBindingSet<ExtendParkingTimeView, ExtendParkingTimeViewModel>();

            var source = new ExtendParkingTimeTableSource(tableExtendTime, this);
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.ExtendTimes);

            set.Bind(label).For(v => v.Text).To(vm => vm.TotalParkingTime).WithConversion("ParkingTimer");

            set.Apply();

			tableExtendTime.TableFooterView = new UIView ();

            tableExtendTime.Source = source;
            tableExtendTime.ReloadData();
        }
    }
}

