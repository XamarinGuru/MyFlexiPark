using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using FlexyPark.UI.Touch.Helpers;
using FlexyPark.Core;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using CrittercismIOS;
using System;
using FlexyPark.UI.Touch.Views;

using Google.Maps;

namespace FlexyPark.UI.Touch
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        // class-level declarations
        public IMvxTouchViewPresenter presenter;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // If you have defined a root view controller, set it here:
            // Window.RootViewController = myViewController;

            Stripe.StripeClient.DefaultPublishableKey = AppConstants.StripeAPIKey;

			MapServices.ProvideAPIKey("AIzaSyAiBwRUm_KZDv_sp3eI7F8hxkePqDTvY20");

            presenter = IsPad ? (IMvxTouchViewPresenter)new PadPresenter(this,Window) : (IMvxTouchViewPresenter)new PhonePresenter(this, Window);

            var setup = new Setup(this, presenter);
            setup.Initialize();

            //get last used language
            var language = NSUserDefaults.StandardUserDefaults.StringForKey(AppConstants.Language);
            if (string.IsNullOrEmpty(language))
                Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(string.Empty);
            else
                Mvx.Resolve<IMvxTextProviderBuilder>().LoadResources(language);

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start(); 

            // make the window visible
            Window.MakeKeyAndVisible();

            //customize navigation bar
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(35,137,203);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes(new NSDictionary(UIStringAttributeKey.ForegroundColor, UIColor.White));
            UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);


            //customize tab bar
            var textAttributes = new UITextAttributes();
            textAttributes.Font = UIFont.FromName("HelveticaNeue-Bold", 17f);
            textAttributes.Font = FontHelper.AdjustFontSize(textAttributes.Font);
            UITabBarItem.Appearance.SetTitleTextAttributes(textAttributes, UIControlState.Normal);

            //initialize Crittercism
            #if DEBUG
            #else
            Crittercism.Init("55fcbf338d4d8c0a00d07a12");
            #endif
           

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            Console.WriteLine("OnActivated");
            Console.WriteLine((presenter as PhonePresenter).MasterNavigationController.TopViewController);
            if((presenter as PhonePresenter).MasterNavigationController.TopViewController is ParkingReservedView)
            {
                var vc = (presenter as PhonePresenter).MasterNavigationController.TopViewController as ParkingReservedView;
                vc.ViewModel.CheckTotalParkingTime();
            }
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public static bool IsPad
        {
            get
            {
                return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
            }
        }
    }
}


