using System;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.CrossCore;
using System.Reflection;
using System.Collections.Generic;
using Cirrious.CrossCore.Plugins;
using UIKit;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using FlexyPark.Core.Helpers;
using FlexyPark.Core;
using FlexyPark.Core.Services;
using FlexyPark.UI.Touch.Services;
using FlexyPark.UI.Touch.Views;

namespace FlexyPark.UI.Touch
{

	public class PadPresenter : MvxTouchViewPresenter
	{
        SplitViewController _splitView;
        UIWindow _window;
        UINavigationController _navigationController;
        public PadPresenter (UIApplicationDelegate applicationDelegate, UIWindow window) : base (applicationDelegate, window)
		{
            _splitView = new SplitViewController();
            _window = window;
            _navigationController = new UINavigationController();
            //_window.RootViewController = _splitView;
		}

		private IMvxTouchViewCreator _viewCreator;

		protected IMvxTouchViewCreator ViewCreator {
			get { return _viewCreator ?? (_viewCreator = Mvx.Resolve<IMvxTouchViewCreator> ()); }
		}

           

        public override void Show(IMvxTouchView view)
        {
            if (view is SignInView || view is SignUpView )
            {
                if(_navigationController != null)
                    _window.RootViewController = _navigationController;


                if (view is SignInView )
                    _navigationController.SetViewControllers(new UIViewController[] { view as UIViewController }, true);
                else
                    _navigationController.PushViewController(view as UIViewController , true);
                //TODO : maybe using MasterNavigationController to Push SignUpView into NavigationStack
                //or use base.Show(view) ???
                return;
            }
            else
            {
                if(_splitView != null)
                    _window.RootViewController = _splitView;
                
                if (view is BaseMenuView)
                {
                    _splitView.SetPrimaryView(view);
                }
                else
                {
                    _splitView.SetSecondaryView(view);
                }
            }
        }

        public override void Show(MvxViewModelRequest request)
        {
            var view = ViewCreator.CreateView(request);
            Show(view);
        }

//		public override void Show (MvxViewModelRequest request)
//		{
//			if (request.PresentationValues != null)
//			{
//				if (request.PresentationValues.ContainsKey (PresentationBundleFlagKeys.ClearStack)) {
//					var nextViewController = (UIViewController)ViewCreator.CreateView (request);
//
//					if (MasterNavigationController.TopViewController.GetType() != nextViewController.GetType())
//					{
//						//SideMenuView temp = MasterNavigationController.ViewControllers[1] as SideMenuView;
//						//temp.mainFeedView.ReleaseMemory ();
//						for (int i = MasterNavigationController.ViewControllers.Length - 1; i >= 0; i--)
//						{
//							var vc = MasterNavigationController.ViewControllers[i];
//							MasterNavigationController.PopViewController(false);
//							vc = null;
//						}
//						//MasterNavigationController.PopToRootViewController (false);
//						MasterNavigationController.PushViewController(nextViewController, true);
//					}
//
//					return;
//				}
//				else if (request.PresentationValues.ContainsKey (PresentationBundleFlagKeys.Child)) 
//				{
//					var nextViewController = (UIViewController)ViewCreator.CreateView (request);
//					if (MasterNavigationController.TopViewController.GetType() != nextViewController.GetType())
//					{
//						var vc = nextViewController as BaseChildView;
//						Console.WriteLine(vc);
//						var home = MasterNavigationController.VisibleViewController as BaseView;
//						if (home.currentChildView != null)
//							home.RemoveChildView(home.currentChildView);
//						if(home.currentChildView == null)
//							home.AddChildView(vc);
//					}
//
//					return;
//				}
//			}
//
//			base.Show (request);
//		}
//
//		public override void Close(IMvxViewModel toClose)
//		{
//			if(MasterNavigationController!=null)
//			{
//				if (toClose.GetType ().BaseType == typeof(BaseChildViewModel)) {
//					var visibleController = MasterNavigationController.VisibleViewController as BaseView;
//					if (visibleController.currentChildView != null)
//						visibleController.RemoveChildView (visibleController.currentChildView);
//				} else
//					base.Close (toClose);
//			}
//			else
//				base.Close(toClose);
//		}

	}
}
