
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class MyReservationsView : BaseView
    {
        public MyReservationsView()
            : base("MyReservationsView", null)
        {
        }

        public new MyReservationsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as MyReservationsViewModel;
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

            ViewModel.GetMyReservations();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            //SetTitle("My reservations");
            //SetBackButtonTitle("Back");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<MyReservationsView, MyReservationsViewModel>();

            var source = new MyReservationTableSource(tableReservations, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Reservations);

            set.Bind(btnAdd).To(vm => vm.GotoParkingSearchCommand);

            set.Apply();

            this.AutomaticallyAdjustsScrollViewInsets = false;

            tableReservations.Source = source;
            tableReservations.ReloadData();


        }
    }
}

