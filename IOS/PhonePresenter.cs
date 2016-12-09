using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.CrossCore;
using UIKit;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.Helpers;
using FlexyPark.UI.Touch.Views;
using FlexyPark.Core.ViewModels;
using System;

namespace FlexyPark.UI.Touch
{

    public class PhonePresenter : MvxTouchViewPresenter
    {
        public PhonePresenter(UIApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        private IMvxTouchViewCreator _viewCreator;

        protected IMvxTouchViewCreator ViewCreator
        {
            get { return _viewCreator ?? (_viewCreator = Mvx.Resolve<IMvxTouchViewCreator>()); }
        }

        public override void Show(MvxViewModelRequest request)
        {
            if (request.PresentationValues != null)
            {
                if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.ParkingReserved))
                {
                    var vc = MasterNavigationController.TopViewController;
                    while (!(vc is ParkingReservedView))
                    {
                        ReleaseTimerIfNeeded(vc);

                        MasterNavigationController.PopViewController(false);
                        //vc.Dispose();
                        vc = MasterNavigationController.TopViewController;
                    }
                    return;
                }
                else if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.Menu))
                {
                    var vc = MasterNavigationController.TopViewController;
                    while (!(vc is MenuView))
                    {
                        Console.WriteLine(vc);
                        ReleaseTimerIfNeeded(vc);

                        if (vc is ParkingSearchView)
                        {
                            MasterNavigationController.PopViewController(true);
                            return;
                        }
                        else
                            MasterNavigationController.PopViewController(false);

                        //vc.Dispose();
                        vc = MasterNavigationController.TopViewController;
                    }
                    return;
                }
                else if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.Search))
                {
                    var vc = MasterNavigationController.TopViewController;
                    while (!(vc is ParkingSearchView))
                    {
                        ReleaseTimerIfNeeded(vc);

                        MasterNavigationController.PopViewController(false);
                        //vc.Dispose();
                        vc = MasterNavigationController.TopViewController;
                    }
                    return;
                }
                else if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.ParkingList))
                {
                    var vc = MasterNavigationController.TopViewController;
                    while (!(vc is ParkingListsView) && !(vc is MyReservationsView))
                    {
                        ReleaseTimerIfNeeded(vc);

                        MasterNavigationController.PopViewController(false);
                        //vc.Dispose();
                        vc = MasterNavigationController.TopViewController;
                    }
                       
                    return;
                }
                else if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.ClearStack))
                {
                    MasterNavigationController.PopToRootViewController(false);
                }

            }

            //prevent reshowing the current view controller (e.g 10 minutes before leaving )
            if (MasterNavigationController != null && MasterNavigationController.TopViewController != null)
            {
                var topvc = MasterNavigationController.TopViewController;
                if (topvc != null && topvc is MvxViewController && request.ViewModelType == (topvc as MvxViewController).ViewModel.GetType())
                    return;
            }

            base.Show(request);
        }



        public override void Close(IMvxViewModel toClose)
        {
            if (toClose is CommonProfileViewModel || toClose is RentProfileViewModel || toClose is OwnProfileViewModel)
            {
                MasterNavigationController.PopViewController(true);
            }
            else
                base.Close(toClose);

        }

        private void ReleaseTimerIfNeeded(UIViewController vc)
        {
            if (vc is ParkingReservedView)
            {
                if ((vc as ParkingReservedView).timer != null)
                {
                    (vc as ParkingReservedView).timer.Stop();
                    (vc as ParkingReservedView).timer.Dispose();
                    (vc as ParkingReservedView).timer = null;
                }
            }

        }

    }

    public class PhoneNavigationController : UINavigationController
    {
        public PhoneNavigationController(UIViewController rootViewController)
            : base(rootViewController)
        {
        }

        public override bool ShouldAutorotate()
        {
            return false;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }
    }
}
