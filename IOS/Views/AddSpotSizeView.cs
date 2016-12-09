
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotSizeView : BaseView
    {
        public AddSpotSizeView()
            : base("AddSpotSizeView", null)
        {
        }

        public new AddSpotSizeViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddSpotSizeViewModel;
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
			
            SetTitle("Add a spot");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddSpotSizeView, AddSpotSizeViewModel>();
            set.Bind(btnDone).To(vm => vm.DoneCommand);

            var source = new VehicleTypeTableSource(tableTypes, this);
            set.Bind(source).For(v=>v.ItemsSource).To(vm=>vm.VehicleTypes);

            #region Language Binding

            set.Bind(btnDone).For("Title").To(vm=>vm.TextSource).WithConversion("Language", "DoneText");

            #endregion

            set.Apply();

            tableTypes.Source = source;
            tableTypes.ReloadData();
        }
    }
}

