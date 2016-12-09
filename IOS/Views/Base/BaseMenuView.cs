using System;
using Cirrious.MvvmCross.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using BigTed;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Touch.Views
{
    public class BaseMenuView : MvxViewController, IBaseView
    {
        public BaseMenuView(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        private MvxSubscriptionToken mToastToken;
        private MvxSubscriptionToken mProgressToken;
        private MvxSubscriptionToken mAlertToken;
        private ProgressHUD mProgressHud;
        private UIAlertView mAlertView;

        protected void HideBackButton()
        {
            NavigationItem.SetHidesBackButton(true, true);
        }

        protected void SetTitle(string title)
        {
            NavigationItem.Title = title;
        }

        protected void SetBackButtonTitle(string title)
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.SetNavigationBarHidden(false,true);    

            #region UI Settings

            if (!DeviceHelper.IsPad)
                AdjustFontSize(this.View);

            //SetBackButtonTitle("Back");

            this.AutomaticallyAdjustsScrollViewInsets = false;

            #endregion
      

            mToastToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ToastMessage>((ToastMessage message) =>
                {
                    if (message.Sender != ViewModel)
                        return;

                    if (!string.IsNullOrEmpty(message.Message))
                    {
                        var progressHud = new ProgressHUD(){ ForceiOS6LookAndFeel = true };

                        progressHud.ShowToast(message.Message, ProgressHUD.MaskType.None, ProgressHUD.ToastPosition.Bottom, 1500);

                        progressHud.Dispose();
                        progressHud = null;
                    }

                });

            mProgressToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ProgressMessage>(message =>
                {
                    if (message.Sender != ViewModel)
                        return;

                    if (message.IsShow)
                    {
                        if (mProgressHud == null)
                            mProgressHud = new ProgressHUD() { ForceiOS6LookAndFeel = true };

						mProgressHud.Show(message.Message, -1, message.IsAllowInteraction ? ProgressHUD.MaskType.None : ProgressHUD.MaskType.Clear);
                    }
                    else
                    {
                        if (mProgressHud != null)
                        {
                            mProgressHud.Dismiss();
                            mProgressHud.Dispose();
                            mProgressHud = null;
                        }
                    }
                });

            mAlertToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<AlertMessage>(message =>
                {   
                    if (message.Sender != ViewModel)
                        return;

                    //TODO : use UIAlertView in iOS 7.x
                    if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                    {
                        //  show alert view with more than 1 buttons (e.g OK and Cancel )
                        if (message.OtherTitles != null && message.OtherActions != null)
                        {
                            if (mAlertView == null)
                                mAlertView = new UIAlertView(message.Title, message.Message, null, message.CancelTitle);

                            if (message.OtherTitles.Length == message.OtherActions.Length)
                            {
                                foreach (var title in message.OtherTitles)
                                {
                                    mAlertView.AddButton(title);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please check your message constructor");
                                return;
                            }

                            mAlertView.Clicked += (object sender, UIButtonEventArgs e) =>
                                {
                                    if (e.ButtonIndex != 0)
                                        message.OtherActions[(int)e.ButtonIndex - 1]();
                                    else
                                    {
                                        if (message.CancelAction != null)
                                            message.CancelAction();
                                    }

                                };

                            mAlertView.Dismissed += (object sender, UIButtonEventArgs e) => 
                                {
                                    if (mAlertView != null)
                                    {
                                        mAlertView.Dispose();
                                        mAlertView = null;
                                    }
                                };

                            mAlertView.Show();
                        }
                        //this is just a normal alert view :)
                        else
                        {                       
                            if (mAlertView == null)
                                mAlertView = new UIAlertView(message.Title, message.Message, null, message.CancelTitle);

                            mAlertView.Clicked += (object sender, UIButtonEventArgs e) =>
                                {
                                    if (message.CancelAction != null)
                                        message.CancelAction();

                                };

                            mAlertView.Dismissed += (object sender, UIButtonEventArgs e) => 
                                {
                                    if (mAlertView != null)
                                    {
                                        mAlertView.Dispose();
                                        mAlertView = null;
                                    }
                                };

                            mAlertView.Show();
                        }

                    }
                    else
                    {


                        //show alert view with more than 1 buttons (e.g OK and Cancel )
                        if (message.OtherTitles != null && message.OtherActions != null)
                        {
                            UIAlertController alertController = UIAlertController.Create(message.Title, message.Message, UIAlertControllerStyle.Alert);
                            UIAlertAction cancelAction = UIAlertAction.Create(message.CancelTitle, UIAlertActionStyle.Cancel, 
                                action =>
                                {
                                    if (message.CancelAction != null)
                                        message.CancelAction();
                                });
                            alertController.AddAction(cancelAction);

                            if (message.OtherTitles.Length == message.OtherActions.Length)
                            {
                                for (int i = 0; i < message.OtherTitles.Length; i++)
                                {
                                    UIAlertAction alertAction = UIAlertAction.Create(message.OtherTitles[i], UIAlertActionStyle.Default,  
                                        action =>
                                        {
                                            if ((i - 1 >= 0) && (message.OtherActions[i - 1] != null))
                                                message.OtherActions[i - 1]();
                                        });
                                    alertController.AddAction(alertAction);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please check your message constructor");
                                return;
                            }

                            PresentViewController(alertController, true, null);
                        }
                        //this is just a normal alert view :)
                        else
                        {
                            UIAlertController alertController = UIAlertController.Create(message.Title, message.Message, UIAlertControllerStyle.Alert);
                            UIAlertAction cancelAction = UIAlertAction.Create(message.CancelTitle, UIAlertActionStyle.Cancel, 
                                action =>
                                {
                                    if (message.CancelAction != null)
                                        message.CancelAction();
                                });

                            alertController.AddAction(cancelAction);

                            PresentViewController(alertController, true, null);
                        }
                    }
                });

        }

        public void AdjustFontSize(UIView view)
        {
            foreach (var subview in view.Subviews)
            {
                if (subview.Subviews != null && subview.Subviews.Length != 0)
                {
                    AdjustFontSize(subview);
                }
                else
                {
                    if (subview is UIButton)
                    {
                        (subview as UIButton).Font = FontHelper.AdjustFontSize((subview as UIButton).Font);
                    }
                    else if (subview is UILabel)
                    {
                        (subview as UILabel).Font = FontHelper.AdjustFontSize((subview as UILabel).Font);
                    }
                }
            }
        }

        #region IBaseView implementation

        public bool CheckInternetConnection()
        {
            if (Reachability.InternetConnectionStatus() == NetworkStatus.NotReachable)
                return false;
            else
                return true;
        }

        #endregion
    }
}