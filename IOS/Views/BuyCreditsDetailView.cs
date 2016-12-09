using System;

using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class BuyCreditsDetailView : BaseView
    {
        public BuyCreditsDetailView()
            : base("BuyCreditsDetailView", null)
        {
        }

        public new BuyCreditsDetailsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as BuyCreditsDetailsViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            SetTitle("Buy Credits");

            var set = this.CreateBindingSet<BuyCreditsDetailView, BuyCreditsDetailsViewModel>();

            set.Bind(lbTitle).To(vm=>vm.DisplayName);
            set.Bind(lbDescription).To(vm=>vm.Description);
            set.Bind(lbPrice).To(vm=>vm.Price);

            set.Bind(btnBuy).To(vm=>vm.BuyCreditsCommand);

            set.Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.


        }
    }
}


