
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;
using FlexyPark.UI.Touch.Views.Base;
using CoreGraphics;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;

namespace FlexyPark.UI.Touch.Views
{
    public partial class MyProfileView : BaseTabBarView, IMyProfileView, IUITabBarDelegate, IUITabBarControllerDelegate
    {
        MvxSubscriptionToken mTextSourceToken;

        public MyProfileView()
        {
            ViewDidLoad();
        }

        public new MyProfileViewModel ViewModel
        {
            get
            {
                return base.ViewModel as MyProfileViewModel;
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

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            //change tab bar height
            CGRect rect = TabBar.Frame;
            rect.Y = rect.Y + (rect.Height -  UIScreen.MainScreen.Bounds.Height / 8f);
            rect.Height = UIScreen.MainScreen.Bounds.Height / 8f;//
            TabBar.Frame = rect;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.RaisePropertyChanged("TextSource");
            ViewModel.RaisePropertyChanged("SharedTextSource");
            SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));

            mTextSourceToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TextSourceMessage>((TextSourceMessage message) =>
                {
                    ViewModel.RaisePropertyChanged("TextSource");
                    ViewModel.RaisePropertyChanged("SharedTextSource");
                    SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));

                    for (int i = 0; i < 3; i++) {
                        ChangeTitle(i);
                    }
                });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            if (DeviceHelper.IsPad)
                HideBackButton();
            
            //SetTitle("My Profiles");
            //SetBackButtonTitle("Back");

            if (ViewModel == null)
                return;

            ViewModel.View = this;
            ViewModel.BaseView = this;
            // Perform any additional setup after loading the view, typically from a nib.

            //tabbar background color
            TabBar.BarTintColor = UIColor.FromRGB(31,31,31);
            TabBar.UserInteractionEnabled = ViewModel.IsAllowTabChange;

			ViewControllers = new UIViewController[]
				{
					CreateTabFor(0, ViewModel.TextSource.GetText("CommonText"), "icon_people_30.png", "" , ViewModel.CommonVM),
					CreateTabFor(1, ViewModel.TextSource.GetText("OwnText"), "icon_home_30.png" , "" , ViewModel.OwnVM)
				};

            CustomizableViewControllers = new UIViewController[] { };
            SelectedViewController = ViewControllers[0];

            // Perform any additional setup after loading the view, typically from a nib.
            var set = this.CreateBindingSet<MyProfileView, MyProfileViewModel>();

            set.Apply();

            TabBar.UserInteractionEnabled = ViewModel.IsAllowTabChange;

            this.Delegate = this;
        }

        private void ChangeTitle(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0: 
                    SetTitle(ViewModel.TextSource.GetText("MyProfileText"));
                    break;
                case 1: 
                    SetTitle(ViewModel.TextSource.GetText("OwnText"));
                    break;
                default:
                    break;
            }
        }

        #region IMyProfileView implementation

        public void ShowTab(int tabIndex)
        {
            if(tabIndex < ViewControllers.Length)
                SelectedViewController = ViewControllers[tabIndex];

            ChangeTitle(tabIndex);

            TabBar.UserInteractionEnabled = ViewModel.IsAllowTabChange;
        }

        #endregion

        #region UITabbarControllerDelegate
        [Foundation.Export("tabBarController:shouldSelectViewController:")]
        public virtual bool ShouldSelectViewController (UITabBarController tabBarController, UIViewController viewController)
        {
            return ViewModel.IsAllowTabChange;
        }
        #endregion


        #region UITabbarDelegate
        [Foundation.Export("tabBar:didSelectItem:")]
        public virtual void ItemSelected (UITabBar tabbar, UITabBarItem item)
        {
            /*int index = 0;
            switch (item.Tag)
            {
                case 0:
                    index = 0;
                    break;
                case 1:
                    index = 1;
                    break;
                case 2:
                    index = 2;
                    break;
                default:
                    break;
            }*/
            if(ViewModel.IsAllowTabChange)
                ChangeTitle((int)item.Tag); 
        }

        #endregion
    }
}

