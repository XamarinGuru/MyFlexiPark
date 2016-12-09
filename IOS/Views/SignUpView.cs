
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class SignUpView : BaseView
    {
        public SignUpView()
            : base("SignUpView", null)
        {
        }

        public new SignUpViewModel ViewModel
        {
            get
            {
                return base.ViewModel as SignUpViewModel;
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
			

            var set = this.CreateBindingSet<SignUpView, SignUpViewModel>();

            set.Bind(swAccept).For(v => v.On).To(vm => vm.IsTermsAccepted);

            set.Apply();
		}
    }
}

