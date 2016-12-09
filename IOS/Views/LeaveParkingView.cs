
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views
{
    public partial class LeaveParkingView : BaseView
    {
        public LeaveParkingView()
            : base("LeaveParkingView", null)
        {
        }

        public new LeaveParkingViewModel ViewModel
        {
            get
            {
                return base.ViewModel as LeaveParkingViewModel;
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
			
            SetTitle("Leave");
            //HideBackButton();

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<LeaveParkingView, LeaveParkingViewModel>();

            set.Bind(btnDone).To(vm => vm.GoToMenuCommand);
            set.Bind(swLike).For(v => v.On).To(vm => vm.IsLikedThisSpot);
            set.Bind(tvComment).To(vm => vm.Comment);

            #region Language Binding

            set.Bind(btnDone).For("Title").To(vm => vm.TextSource).WithConversion("Language", "DoneText");
            set.Bind(lbILike).To(vm => vm.TextSource).WithConversion("Language", "ILikedThisSpotText");
            set.Bind(lbYourComment).To(vm => vm.TextSource).WithConversion("Language", "YourCommentText");

            #endregion

            set.Apply();
        }

    }
}

