
using Foundation;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using KLCPopupBinding;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using System.Globalization;
using System.Diagnostics;

namespace FlexyPark.UI.Touch.Views
{
    public partial class MenuView : BaseMenuView, IMenuView
    {
        MvxSubscriptionToken mTextSourceToken;

        KLCPopup mParkMePopup;

        public MenuView()
            : base("MenuView", null)
        {
        }

        public new MenuViewModel ViewModel
        {
            get
            {
                return base.ViewModel as MenuViewModel;
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

            NavigationController.SetNavigationBarHidden(true, true);
            ViewModel.RaisePropertyChanged("TextSource");
            ViewModel.RaisePropertyChanged("SharedTextSource");
            SetTitle(ViewModel.TextSource.GetText("PageTitle"));
            SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));
            if (Mvx.Resolve<ICacheService>().CurrentUser != null)
                ViewModel.Credits = double.Parse(Mvx.Resolve<ICacheService>().CurrentUser.RemainingCredits, CultureInfo.InvariantCulture);
            
            ViewModel.RaisePropertyChanged("Credits");

            mTextSourceToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TextSourceMessage>((TextSourceMessage message) =>
                {
                    ViewModel.RaisePropertyChanged("TextSource");
                    ViewModel.RaisePropertyChanged("SharedTextSource");
                    SetTitle(ViewModel.TextSource.GetText("PageTitle"));
                    SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));
                });

            ViewModel.UpdateRemainCredit();

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetTitle("MyFlexyPark");
            HideBackButton();

            ViewModel.View = this;
            ViewModel.BaseView = this;

            /*var btnBarSettings = new UIBarButtonItem(){Image = UIImage.FromFile("white_icon_setting_22.png")};
            NavigationItem.RightBarButtonItem = btnBarSettings;*/
            // Perform any additional setup after loading the view, typically from a nib.


            var set = this.CreateBindingSet<MenuView, MenuViewModel>();
            //set.Bind(btnBarSettings).To(vm=>vm.GotoSettingsCommand);
            set.Bind(btnSettings).To(vm => vm.GotoSettingsCommand);
            set.Bind(btnParkMe).To(vm => vm.GotoParkMeCommand);
            set.Bind(btnMyReservations).To(vm => vm.GotoMyReservationsCommand);
            set.Bind(btnMyOwnParking).To(vm => vm.GotoMyOwnParkingCommand);
            set.Bind(btnCredits).To(vm => vm.GotoCreditsCommand);

            set.Bind(btnParkmeNow).To(vm => vm.ParkMeNowCommand);
            set.Bind(btnParkmeLater).To(vm => vm.ParkMeLaterCommand);

            #region Language Binding

            //set.Bind(lbCredits).To(vm => vm.Credits).WithConversion("Credits");
            set.Bind(lbMyOwnParking).For(v => v.Text).To(vm => vm.TextSource).WithConversion("Language", "MyOwnParkingText");
            set.Bind(lbParkMe).For(v => v.Text).To(vm => vm.TextSource).WithConversion("Language", "ParkMeText");
            set.Bind(lbMyReservations).For(v => v.Text).To(vm => vm.TextSource).WithConversion("Language", "MyReservationsText");

            set.Bind(btnParkmeNow).For("Title").To(vm => vm.TextSource).WithConversion("Language", "NowText");
            set.Bind(btnParkmeLater).For("Title").To(vm => vm.TextSource).WithConversion("Language", "LaterText");

            #endregion

            set.Apply();

            Debug.WriteLine(NSLocale.CurrentLocale.LocaleIdentifier);

            #region UI Settings

            mParkMePopup = KLCPopup.PopupWithContentView(vParkMePopup);
            mParkMePopup.ShouldDismissOnBackgroundTouch = true;
            mParkMePopup.DimmedMaskAlpha = 0.7f;

            #endregion
        }




        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!AppDelegate.IsPad)
            {
                cstHeightViewContent.Constant = vCredits.Frame.Y + vCredits.Frame.Height;
            }
        }


        #region IMenuView implementation

        public void ShowParkMePopup()
        {
            if (mParkMePopup != null)
                mParkMePopup.Show();
        }

        public void ClosePopup()
        {
            if (mParkMePopup.IsShowing)
                mParkMePopup.Dismiss(true);
        }

        #endregion
    }
}

