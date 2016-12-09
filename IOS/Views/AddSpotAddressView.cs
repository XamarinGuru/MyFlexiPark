
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotAddressView : BaseView
    {
        public AddSpotAddressView()
            : base("AddSpotAddressView", null)
        {
        }

        public new AddSpotAddressViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddSpotAddressViewModel;
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

            SetTitle(ViewModel.TextSource.GetText("PageTitle"));

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddSpotAddressView, AddSpotAddressViewModel>();
            //set.Bind(btnDone).To(vm => vm.DoneCommand);

            set.Bind(tfLocation).To(vm => vm.Address);
            //set.Bind(tfCity).To(vm => vm.City);
            //set.Bind(tfZipCode).To(vm => vm.ZipCode);

            #region Language Binding

            set.Bind(lbLocationLand).To(vm => vm.TextSource).WithConversion("Language", "LocationLandText");


            #endregion

            set.Apply();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel.DoneCommand.Execute();
        }
    }
}

