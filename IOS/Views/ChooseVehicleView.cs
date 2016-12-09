
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;
using Cirrious.MvvmCross.Binding.ValueConverters;
using Cirrious.CrossCore;
using System.Threading;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ChooseVehicleView : BaseView, IChooseVehicleView
    {
        private MvxSubscriptionToken mUpdateSuccessToken;

        public ChooseVehicleView()
            : base("ChooseVehicleView", null)
        {
        }

        public new ChooseVehicleViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ChooseVehicleViewModel;
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
            ViewModel.GetVehicles();
        }

        UIBarButtonItem btnBarEdit;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            //SetTitle("Choose vehicle");
            //SetBackButtonTitle("Back");

            ViewModel.View = this;

            mUpdateSuccessToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<UpdateSuccessMessage>(message =>
                {
                    if (message.Sender.GetType() == typeof(AddVehicleViewModel))
                        ViewModel.GetVehicles();
                });

            // Perform any additional setup after loading the view, typically from a nib.

            btnBarEdit = new UIBarButtonItem(){ Title = ViewModel.TextSource.GetText("EditTitle") };
            NavigationItem.RightBarButtonItem = btnBarEdit;
         
            var set = this.CreateBindingSet<ChooseVehicleView,ChooseVehicleViewModel>();
            set.Bind(btnBarEdit).To(vm => vm.SwitchModeCommand);
            set.Bind(btnAdd).To(vm => vm.GotoAddVehicleCommand);

            set.Bind(btnAdd).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ChooseVehicle");

            var source = new VehicleTableSource(tableVehicles, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Vehicles);

            set.Apply();

            tableVehicles.Source = source;
            tableVehicles.ReloadData();
        }


        #region IChooseVehicleView implementation

        public void SetModeTitle(string title)
        {
            //btnBarEdit.Title = title.Equals(ViewModel.TextSource.GetText("EditTitle")) ? ViewModel.TextSource.GetText("EditTitle") : ViewModel.TextSource.GetText("CancelTitle");
            btnBarEdit.Title = title.Equals(ViewModel.TextSource.GetText("EditTitle")) ? ViewModel.TextSource.GetText("EditTitle") : string.Empty;

            tableVehicles.ReloadData();
        }

        #endregion
    }
}

