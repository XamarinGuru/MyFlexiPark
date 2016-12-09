
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ParkMeView : BaseMenuView
    {
        public ParkMeView()
            : base("ParkMeView", null)
        {
        }

        public new ParkMeViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ParkMeViewModel;
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
            ViewModel.RaisePropertyChanged("TextSource");
            SetTitle(ViewModel.TextSource.GetText("PageTitle"));
            SetBackButtonTitle(ViewModel.TextSource.GetText("BackTitle"));
            base.ViewWillAppear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            SetTitle("MyFlexyPark");
            SetBackButtonTitle("Back");

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<ParkMeView, ParkMeViewModel>();
            set.Bind(btnParkMeNow).To(vm=>vm.ParkMeNowCommand);
            set.Bind(btnParkMeLater).To(vm=>vm.ParkMeLaterCommand);
            set.Apply();

        }
    }
}

