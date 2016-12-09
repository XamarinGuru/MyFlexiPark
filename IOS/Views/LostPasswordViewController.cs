using System;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class LostPasswordViewController : BaseView, ILostPasswordView
    {
        public LostPasswordViewController () : base ("LostPasswordViewController", null)
        {
        }

        public new LostPasswordViewModel ViewModel
        {
            get{
                return base.ViewModel as LostPasswordViewModel;
            }
            set{
                base.ViewModel = value;
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            //SetTitle("MyProfile");
            ViewModel.View = this;

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<LostPasswordViewController, LostPasswordViewModel> ();
            set.Bind (btnOk).To (vm => vm.OkCommand);

            set.Bind (tfEmail).To (vm => vm.Email);

            #region Language Binding

            set.Bind (btnOk).For ("Title").To (vm => vm.TextSource).WithConversion ("Language", "OkText");

            set.Bind (lbEmail).To (vm => vm.TextSource).WithConversion ("Language", "EmailText");

            #endregion

            set.Apply ();
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        public string GetDeviceIdentifier ()
        {
            var udid = UIDevice.CurrentDevice.IdentifierForVendor.ToString ();
            udid = udid.Substring (udid.IndexOf ('>') + 1).Trim ().Replace ("-", "");
            return udid;
        }

        public void CloseKeyboard()
        {
            this.View.EndEditing (true);
        }
    }
}


