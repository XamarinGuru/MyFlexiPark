
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using BigTed;
using FlexyPark.UI.Touch.Helpers;
using System.Diagnostics;

namespace FlexyPark.UI.Touch.Views
{
    public partial class SignInView : BaseView
    {
        public SignInView()
            : base("SignInView", null)
        {
        }

        public new SignInViewModel ViewModel
        {
            get
            {
                return base.ViewModel as SignInViewModel;
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
            //HideBackButton();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //NSTimeZone timezone = NSTimeZone.LocalTimeZone;

            //SetTitle("MyProfile");

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<SignInView, SignInViewModel>();
            set.Bind(btnCreate).To(vm => vm.GotoSignUpCommand);
            set.Bind(btnSignIn).To(vm => vm.SignInCommand);
            set.Bind (btnForgotPassword).To (vm=>vm.LostPasswordCommand);

            set.Bind(tfEmail).To(vm => vm.Email);
            set.Bind(tfPassword).To(vm => vm.Password);

            #region Language Binding

            set.Bind(btnCreate).For("Title").To(vm => vm.TextSource).WithConversion("Language", "CreateAccountText");
            set.Bind(btnSignIn).For("Title").To(vm => vm.TextSource).WithConversion("Language", "SignInText");
			set.Bind(btnForgotPassword).For("Title").To(vm => vm.TextSource).WithConversion("Language", "ForgotPasswordText");

            set.Bind(lbEmail).To(vm => vm.TextSource).WithConversion("Language", "EmailText");
            set.Bind(lbPassword).To(vm => vm.TextSource).WithConversion("Language", "PasswordText");
            set.Bind(lbOr).To(vm => vm.TextSource).WithConversion("Language", "OrText");

            #endregion

            set.Apply();



        }
    }
}

