
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class CommonProfileView : BaseView, ICommonProfileView
    {
        public CommonProfileView()
            : base("CommonProfileView", null)
        {
        }

        public new CommonProfileViewModel ViewModel
        {
            get
            {
                return base.ViewModel as CommonProfileViewModel;
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

            ViewModel.View = this;
			
            //SetTitle("MyProfile");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<CommonProfileView, CommonProfileViewModel>();

            set.Bind(swAccept).For(v => v.On).To(vm => vm.IsAcceptTerms);
            set.Bind(vInfo).For(v => v.Hidden).To(vm => vm.IsShowInfo).WithConversion("BooleanToHidden");

            /*set.Bind(tfFirstName).To(vm => vm.FirstName);
            set.Bind(tfLastName).To(vm => vm.LastName);
            set.Bind(tfEmail).To(vm => vm.Email);
            set.Bind(tfPassword).To(vm => vm.Password);
            set.Bind(tfMobileNumber).To(vm => vm.MobileNumber);
            set.Bind(tfPromoCode).To(vm => vm.PromoCode);*/

            set.Bind(tfFirstName).To(vm => vm.User.FirstName);
            set.Bind(tfLastName).To(vm => vm.User.LastName);
            set.Bind(tfEmail).To(vm => vm.User.Email);
            set.Bind(tfPassword).To(vm => vm.Password);
            set.Bind(tfMobileNumber).To(vm => vm.User.PhoneNumber);
			set.Bind(tfPromoCode).To(vm => vm.User.PostalCode);

            set.Bind(btnLogout).To(vm => vm.LogoutCommand);
            set.Bind(btnLogout).For(v => v.Hidden).To(vm => vm.IsShowInfo).WithConversion("BooleanToHidden");

            set.Bind(btnOK).To(vm => vm.SignUpOrUpdateCommand);
            set.Bind (btnReadTerms).To (vm => vm.ReadTermsOfUseCommand);

            set.Bind(lbAcceptTOS).For(v => v.Hidden).To(vm => vm.IsShowInfo);
            set.Bind(swAccept).For(v => v.Hidden).To(vm => vm.IsShowInfo);


            #region Language Binding

            set.Bind(btnOK).For("Title").To(vm => vm.TextSource).WithConversion("Language", "OkText");
            set.Bind(btnReadTerms).For("Title").To(vm => vm.TextSource).WithConversion("Language", "ReadTermsOfUseText");

            set.Bind(lbFirstName).To(vm => vm.TextSource).WithConversion("Language", "FirstNameText");
            set.Bind(lbLastName).To(vm => vm.TextSource).WithConversion("Language", "LastNameText");
            set.Bind(lbEmail).To(vm => vm.TextSource).WithConversion("Language", "EmailText");
            set.Bind(lbPassword).To(vm => vm.TextSource).WithConversion("Language", "PasswordText");
            set.Bind(lbMobileNumber).To(vm => vm.TextSource).WithConversion("Language", "MobileNumberText");
            set.Bind(lbPromocode).To(vm => vm.TextSource).WithConversion("Language", "PromoCodeText");
            set.Bind(lbTOSVer).To(vm => vm.TextSource).WithConversion("Language", "TermsOfUseVersionText");
            set.Bind(lbCreatedDate).To(vm => vm.TextSource).WithConversion("Language", "CreatedDateText");
            set.Bind(lbAcceptTOS).To(vm => vm.TextSource).WithConversion("Language", "IAcceptYourTermsOfUseText");

            #endregion

            set.Apply();
        }

        #region ICommonProfileView implementation

        public string GetDeviceIdentifier()
        {
            var udid = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
            udid = udid.Substring(udid.IndexOf('>') + 1).Trim().Replace("-", "");
            return udid;
        }

        public void OpenURL (string url)
        {
            if (UIApplication.SharedApplication.CanOpenUrl (new NSUrl (url))) {
                UIApplication.SharedApplication.OpenUrl (new NSUrl (url));
            }
        }

		public void CloseKeyboard()
		{
			View.EndEditing(true);
		}

        #endregion
    }
}

