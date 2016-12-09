using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "Menu View", MainLauncher = false, LaunchMode = LaunchMode.SingleTask,
        WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize,
        ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class MenuView : BaseView, IMenuView
    {
        #region UI Controls

        private LayoutInflater layoutInflater;
        private PopupWindow popupWindow;
        private View popupView;
        private FlexyPark.UI.Droid.Controls.TextRegular ParkMeNow, ParkMeLater;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new MenuViewModel ViewModel
        {
            get { return base.ViewModel as MenuViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MenuView);
            ViewModel.View = this;
            layoutInflater = (LayoutInflater)BaseContext.GetSystemService(LayoutInflaterService);
            popupView = layoutInflater.Inflate(Resource.Layout.ParkMeView, null);

            popupWindow = new PopupWindow(popupView, ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);

            popupWindow.AnimationStyle = Resource.Style.DialogAnimation;

            popupWindow.OutsideTouchable = true;
            popupWindow.Focusable = true;

            popupWindow.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            ParkMeNow = popupView.FindViewById<FlexyPark.UI.Droid.Controls.TextRegular>(Resource.Id.ParkMeNow);

            ParkMeLater = popupView.FindViewById<FlexyPark.UI.Droid.Controls.TextRegular>(Resource.Id.ParkMeLater);

            LinearLayout linearLayout = popupView.FindViewById<LinearLayout>(Resource.Id.llfullscreen);
            linearLayout.Click += (sender, args) => { popupWindow.Dismiss(); };

            SetButtonEffectsByViews(new List<View>()
            {
                ParkMeNow,
                ParkMeLater
            });

            ParkMeNow.Text = Mvx.Resolve<ICacheService>().TextSource.GetText("NowText");
            ParkMeLater.Text = Mvx.Resolve<ICacheService>().TextSource.GetText("LaterText");


            ParkMeNow.Click += (sender, args) => { ViewModel.ParkMeNow(); };
            ParkMeLater.Click += (sender, args) => { ViewModel.ParkMeLater(); };

            SetButtonEffects(new List<int>()
            {
                Resource.Id.llParkMe,
                Resource.Id.llMyOwnParking,
                Resource.Id.llCredits,
                Resource.Id.llReservations,
                Resource.Id.ivSetting,
            });
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (ParkMeNow != null && ParkMeLater != null)
            {
                ParkMeNow.Text = Mvx.Resolve<ICacheService>().TextSource.GetText("NowText");
                ParkMeLater.Text = Mvx.Resolve<ICacheService>().TextSource.GetText("LaterText");
            }

            ViewModel.RaisePropertyChanged("Credits");
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #endregion

        #region Implements

        /// <summary>
        /// </summary>
        public void ShowParkMePopup()
        {
            popupWindow.ShowAtLocation(popupView, GravityFlags.Center, 0, 0);
        }

        /// <summary>
        /// </summary>
        public void ClosePopup()
        {
            popupWindow.Dismiss();
        }

        #endregion

        #region Methods

        public void SetButtonEffectsByViews(List<View> Views)
        {
            foreach (var view in Views)
            {
                view.Touch += (sender, args) =>
                {
                    args.Handled = false;

                    switch (args.Event.Action)
                    {
                        case MotionEventActions.Down:
                            var alphaAnim = new AlphaAnimation(1F, 0.5F) { Duration = 0, FillAfter = true };
                            (sender as View).StartAnimation(alphaAnim);
                            break;

                        case MotionEventActions.Up:
                            alphaAnim = new AlphaAnimation(0.5F, 1F) { Duration = 0, FillAfter = true };
                            (sender as View).StartAnimation(alphaAnim);
                            break;

                        case MotionEventActions.Cancel:
                            alphaAnim = new AlphaAnimation(0.5F, 1F) { Duration = 0, FillAfter = true };
                            (sender as View).StartAnimation(alphaAnim);
                            break;
                    }
                };
            }
        }

        #endregion
    }
}