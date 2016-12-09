
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddVehicleView : BaseView
    {
        public AddVehicleView()
            : base("AddVehicleView", null)
        {
        }

        public new AddVehicleViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddVehicleViewModel;
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

        public override void ViewWillDisappear(bool animated)
        {
            ViewModel.AddNewVehicleCommand.Execute();

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.PropertyChanged += (sender, e) => 
                {
                    if(e.PropertyName.Equals("VehicleTypes"))
                        tableCarType.ReloadData();
                };
			
            SetTitle(ViewModel.IsEditMode ? ViewModel.TextSource.GetText("EditPageTitle") : ViewModel.TextSource.GetText("PageTitle"));
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddVehicleView,AddVehicleViewModel>();

            set.Bind(tfPlateNumber).To(vm => vm.PlateNumber);

            set.Bind(btnAdd).To(vm => vm.AddNewVehicleCommand);
            set.Bind(btnAdd).For(v=>v.Hidden).To(vm => vm.IsEditMode);

            var source = new VehicleTypeTableSource(tableCarType, this);
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.VehicleTypes);

            #region Language Binding

            set.Bind(btnAdd).For("Title").To(vm => vm.ButtonTitle).WithConversion("AddEditButtonTitle", ViewModel.IsEditMode ? "Edit" : "Add");
            set.Bind(lbPlateNumber).To(vm=>vm.TextSource).WithConversion("Language", "PlateNumberText");

            #endregion

            set.Apply();

            tableCarType.Source = source;
            tableCarType.ReloadData();

            #region UI Settings

            this.AutomaticallyAdjustsScrollViewInsets = false;

            #endregion
        }
    }
}

