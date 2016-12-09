using System;

using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotStatusView : BaseView
    {
        public AddSpotStatusView()
            : base("AddSpotStatusView", null)
        {
        }

        public new AddSpotStatusViewModel ViewModel
        {
            get{
                return base.ViewModel as AddSpotStatusViewModel;
            }
            set{
                base.ViewModel = value;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            SetTitle("Add spot staus");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddSpotStatusView, AddSpotStatusViewModel>();

            set.Bind(swStatus).For(v=>v.On).To(vm => vm.IsParkingDisabled).WithConversion("BooleanToHidden");

            #region Language Binding

//            set.Bind(btnDone).For("Title").To(vm=>vm.TextSource).WithConversion("Language", "DoneText");

            #endregion

            set.Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel.DoneCommand.Execute();

            Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Complete;
        }

    }
}


