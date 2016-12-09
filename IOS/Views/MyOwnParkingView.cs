
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class MyOwnParkingView : BaseView, IMyOwnParkingView
    {
        public MyOwnParkingView()
            : base("MyOwnParkingView", null)
        {
        }

        public new MyOwnParkingViewModel ViewModel
        {
            get
            {
                return base.ViewModel as MyOwnParkingViewModel;
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

            ViewModel.GetMyOwnParking();
        }

        UIBarButtonItem btnBarDelete;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            //SetTitle("Owner Spots");
            //SetBackButtonTitle("Back");

            ViewModel.View = this;

            // Perform any additional setup after loading the view, typically from a nib.

//            btnBarDelete = new UIBarButtonItem(){ Title = ViewModel.TextSource.GetText("DeleteText") };
//            NavigationItem.RightBarButtonItem = btnBarDelete;


            var set = this.CreateBindingSet<MyOwnParkingView, MyOwnParkingViewModel>();

            var source = new OwnerParkingTableSource(tableParkings, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Parkings);

            set.Bind(btnAddSpot).To(vm => vm.GotoAddSpotCommand);
//            set.Bind(btnBarDelete).To(vm => vm.SwitchModeCommand);

            set.Apply();

            tableParkings.Source = source;
            tableParkings.ReloadData();
        }

        #region IMyOwnParkingView implementation

        public void SetModeTitle(string title)
        {
            btnBarDelete.Title = title.Equals(ViewModel.TextSource.GetText("DeleteText")) ? ViewModel.TextSource.GetText("DeleteText") : ViewModel.TextSource.GetText("CancelText");

            tableParkings.ReloadData();
        }

        #endregion
    }
}

