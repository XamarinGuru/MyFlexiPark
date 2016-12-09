
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class SettingsView : BaseMenuView
    {
        MvxSubscriptionToken mTextSourceToken;

        public SettingsView()
            : base("SettingsView", null)
        {
        }

        public new SettingsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as SettingsViewModel;
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

            ViewModel.RaisePropertyChanged("TextSource");
            ViewModel.RaisePropertyChanged("SharedTextSource");
            SetTitle(ViewModel.TextSource.GetText("PageTitle"));
            SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));

            mTextSourceToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TextSourceMessage>((TextSourceMessage message) =>
                {
                    ViewModel.RaisePropertyChanged("TextSource");
                    ViewModel.RaisePropertyChanged("SharedTextSource");
                    SetTitle(ViewModel.TextSource.GetText("PageTitle"));
                    SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));
                });


        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            SetTitle(ViewModel.TextSource.GetText("PageTitle"));
            //SetBackButtonTitle("Configuration");

            ViewModel.BaseView = this;

            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<SettingsView, SettingsViewModel>();
            set.Bind(btnMyProfile).To(vm => vm.GotoMyProfileCommand);
            set.Bind(btnAppSettings).To(vm => vm.GotoAppSettingsCommand);
            set.Bind (btnMyVehicles).To (vm => vm.GotoMyVehiclesCommand);

            #region Language Binding
            set.Bind(lbMyProfile).For(v=>v.Text).To(vm=>vm.TextSource).WithConversion("Language", "MyProfileText").OneWay();
            set.Bind(lbAppSettings).For(v=>v.Text).To(vm=>vm.TextSource).WithConversion("Language", "AppSettingsText").OneWay();
            set.Bind (lbMyVehicles).For (v => v.Text).To (vm => vm.TextSource).WithConversion ("Language", "MyVehicleText").OneWay ();
            #endregion

            set.Apply();
        }
    }
}

