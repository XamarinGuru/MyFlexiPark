using System.Drawing;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using CoreGraphics;

namespace FlexyPark.UI.Touch.Views
{
    public sealed class SplitViewController : UISplitViewController
    {
        UINavigationController _primaryNav;
        UINavigationController _secondaryNav;

        public SplitViewController ()
        {
            View.Bounds = new CGRect(0,0,UIScreen.MainScreen.Bounds.Width,UIScreen.MainScreen.Bounds.Height);
            Delegate = new SplitViewDelegate();

            _primaryNav = new UINavigationController();
            _secondaryNav = new UINavigationController();

            this.ViewControllers = new UIViewController[] { _primaryNav, _secondaryNav };
            //this.ViewControllers = new UIViewController[] { new UIViewController(), new UIViewController() };
        }

        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

        public void SetPrimaryView(IMvxTouchView view)
        {
            var controller = view as UIViewController;
            _primaryNav.PushViewController(controller,true);

            //this.ViewControllers = new UIViewController[] { controller, ViewControllers[1] };
        }

        public void SetSecondaryView(IMvxTouchView view)
        {
            var controller = view as UIViewController;
			if(view is ExtendParkingTimeView || view is ExtendParkingTimeConfirmView || view is BookingView || view is DelayedParkingMapView)
                _secondaryNav.PushViewController(controller, true);
            else
                _secondaryNav.SetViewControllers(new UIViewController[]{ controller }, true);

            //this.ViewControllers = new UIViewController[] { ViewControllers[0], controller };
        }
    }

    public class SplitViewDelegate : UISplitViewControllerDelegate
    {
        //hide primary view controller if true (if hide - slide to see)
        public override bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation)
        {
            return false;
        }
    }
}