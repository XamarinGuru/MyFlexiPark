
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Timers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ExtendParkingTimeConfirmView : BaseView
    {
        private MvxSubscriptionToken mTimeToken;

        public ExtendParkingTimeConfirmView()
            : base("ExtendParkingTimeConfirmView", null)
        {
        }

        public new ExtendParkingTimeConfirmViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ExtendParkingTimeConfirmViewModel;
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

        public override void ViewWillDisappear(bool animated)
        {
            if (ViewModel.NeedRelease)
            {
            }

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            mTimeToken = Mvx.Resolve<IMvxMessenger>().Subscribe<TimeMessage>(message => {
                if(message.Sender.GetType() != typeof(ParkingReservedViewModel))
                    return;

                ViewModel.TotalParkingTime = message.TimeLeft;
            });

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<ExtendParkingTimeConfirmView, ExtendParkingTimeConfirmViewModel>();

            set.Bind(btnPayWithCredits).To(vm=>vm.PayWithCreditsCommand);
            set.Bind(btnBuyCredits).To(vm => vm.BuyCreditsCommand);

            set.Bind(NavigationItem).For(v=>v.Title).To(vm=>vm.TotalParkingTime).WithConversion("ParkingTimer");

            set.Bind(lbMoney).To(vm => vm.Cost).WithConversion("Money");
            set.Bind(lbCredits).To(vm => vm.RemainingCredits).WithConversion("Money");

            set.Bind(btnBuyCredits).For(v => v.Hidden).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");
            set.Bind(btnPayWithCredits).For(v=>v.Enabled).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");
            set.Bind(btnPayWithCredits).For(v => v.UserInteractionEnabled).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");

            set.Bind(lbPlateNumber).To(vm => vm.Vehicle.PlateNumber);
            set.Bind(lbCarType).To(vm => vm.Vehicle.Type);

            #region Language Binding

            set.Bind(lbBeforeRemove).To(vm=>vm.TextSource).WithConversion("Language", "BeforeRemovingThisFormText");
            set.Bind(lbExtending).To(vm=>vm.TextSource).WithConversion("Language", "ExtendingText");
            set.Bind(lbCost).To(vm=>vm.TextSource).WithConversion("Language", "CostText");
            set.Bind(lbDuration).To(vm=>vm.TextSource).WithConversion("Language", "DurationText");
            set.Bind(lbEndOfReservation).To(vm=>vm.TextSource).WithConversion("Language", "EndOfReservationText");
            set.Bind(lbVehicle).To(vm=>vm.TextSource).WithConversion("Language", "VehicleText");
            set.Bind(btnPayWithCredits).For("Title").To(vm => vm.TextSource).WithConversion("Language", "PayWithCreditsText");
            set.Bind(lbRemainingCredits).To(vm=>vm.TextSource).WithConversion("Language", "RemainingCreditsText");
            set.Bind(btnBuyCredits).For("Title").To(vm => vm.TextSource).WithConversion("Language", "BuyCreditsText");

            set.Bind(lbHours).To(vm=>vm.Duration).WithConversion("Duration");
            set.Bind(lbEndTime).To(vm=>vm.EndTime).WithConversion("ValidTime");

            #endregion

            set.Apply();
        }

    }
}

