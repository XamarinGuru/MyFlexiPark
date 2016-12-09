using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "AddVehicle", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AddVehicleView : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Constructors

        #endregion

        #region Overrides

        public new AddVehicleViewModel ViewModel
        {
            get { return base.ViewModel as AddVehicleViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddVehicleView);

            TextView tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);
            TextView tvAdd = FindViewById<TextView>(Resource.Id.tvAdd);
            var rlBack = FindViewById<RelativeLayout>(Resource.Id.rlBack);


            Title = ViewModel.IsEditMode ? ViewModel.mCacheService.TextSource.GetText("EditPageTitle") : ViewModel.mCacheService.TextSource.GetText("PageTitle");
            var ButtonTitle = ViewModel.mCacheService.TextSource.GetText("AddText");

            tvTitle.Text = Title;
            tvAdd.Text = ButtonTitle;


            rlBack.Click += (sender, args) =>
            {
                if (ViewModel.IsEditMode)
                {
                    ViewModel.AddNewVehicleCommand.Execute(this);
                }
                else
                {
                    ViewModel.BackCommand.Execute(this);
                }
            };

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvAdd
            });

           

        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }


        #endregion

        #region Implements

        #endregion

        #region Methods

        #endregion
    }
}