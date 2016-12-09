
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;
using System.Timers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class BookingView : BaseView
    {
        public BookingView()
            : base("BookingView", null)
        {
        }

        public new BookingViewModel ViewModel
        {
            get
            {
                return base.ViewModel as BookingViewModel;
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
			

            //SetTitle(ViewModel.TextSource.GetText("PageTitle")); 
            //SetBackButtonTitle(ViewModel.TextSource.GetText("BackTitle"));
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<BookingView, BookingViewModel>();
            set.Bind(btnContinue).To(vm => vm.GotoParkingReservedCommand);
            set.Bind(btnChooseVehicle).To(vm=>vm.GotoChooseVehicleCommand);
            set.Bind(btnChooseCreditCard).To(vm => vm.GotoPaymentConfigurationCommand);
            set.Bind(btnBuyCredits).To(vm => vm.BuyCreditsCommand);

            //set.Bind(lbBookingTime).To(vm=>vm.BookingTime).WithConversion("BookingTimer");
            set.Bind(lbMoney).To(vm => vm.Cost).WithConversion("Money");

            set.Bind(btnBuyCredits).For(v => v.Hidden).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");
            set.Bind(btnContinue).For(v=>v.Enabled).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");
            set.Bind(btnContinue).For(v => v.UserInteractionEnabled).To(vm => vm.IsShowBuyCredits).WithConversion("BooleanToHidden");

            set.Bind(lbPlateNumber).To(vm => vm.Vehicle.PlateNumber);
            set.Bind(lbCarType).To(vm => vm.Vehicle.Type);

            set.Bind(lbToWait).For(v => v.Hidden).To(vm => vm.IsShowClock).WithConversion("BooleanToHidden");
            set.Bind(ivClock).For(v => v.Hidden).To(vm => vm.IsShowClock).WithConversion("BooleanToHidden");

            #region Language Binding

            set.Bind(lbBeforeRemove).To(vm=>vm.TextSource).WithConversion("Language", "BeforeRemovingThisFormText");
            set.Bind(lbCost).To(vm=>vm.TextSource).WithConversion("Language", "CostText");
            set.Bind(lbDuration).To(vm=>vm.TextSource).WithConversion("Language", "DurationText");
            set.Bind(lbEndOfReservation).To(vm=>vm.TextSource).WithConversion("Language", "EndOfReservationText");
            set.Bind(lbVehicle).To(vm=>vm.TextSource).WithConversion("Language", "VehicleText");
            set.Bind(lbPayWithCredits).To(vm=>vm.TextSource).WithConversion("Language", "PayWithCreditCardText");
            set.Bind(btnContinue).For("Title").To(vm => vm.TextSource).WithConversion("Language", "PayNowText");
            set.Bind(lbRemaningCredits).To(vm=>vm.TextSource).WithConversion("Language", "RemainingCreditsText");
            set.Bind(btnBuyCredits).For("Title").To(vm => vm.TextSource).WithConversion("Language", "BuyCreditsText");

            set.Bind(lbToWait).To(vm=>vm.MinutesToWait).WithConversion("BookingToWait");
            set.Bind(lbHours).To(vm=>vm.Duration).WithConversion("Duration");
            set.Bind(lbEndTime).To(vm=>vm.EndTime).WithConversion("ValidTime");
            set.Bind(lbCredits).To(vm=>vm.RemainingCredits).WithConversion("Money");
            set.Bind(lbDistance).To(vm=>vm.Distance).WithConversion("Meter");

            #endregion

            set.Apply();
        }

    }
}
