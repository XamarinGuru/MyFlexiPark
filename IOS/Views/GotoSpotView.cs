
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class GotoSpotView : BaseView
    {
        public GotoSpotView()
            : base("GotoSpotView", null)
        {
        }

        public new GotoSpotViewModel ViewModel
        {
            get
            {
                return base.ViewModel as GotoSpotViewModel;
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

            //SetTitle("Add a spot");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<GotoSpotView, GotoSpotViewModel>();
            set.Bind(btnOnSpot).To(vm => vm.ImOnMySpotCommand);

            #region Language Binding

            set.Bind(btnOnSpot).For("Title").To(vm => vm.TextSource).WithConversion("Language", "IAmOnMySpotText");
            set.Bind(lbPleasGo).To(vm=>vm.TextSource).WithConversion("Language", "PleaseGoToText");

            #endregion

            set.Apply();
        }
    }
}

