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
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "AddPostCostView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AddSpotCostView : BaseView
    {
        #region UI Controls

     //   private SeekBar seekBar;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new AddSpotCostViewModel ViewModel
        {
            get { return base.ViewModel as AddSpotCostViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddSpotCostView);
            Init();
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvDone
            });

        }
        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            ViewModel.DoneCommand.Execute();
            Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate = ViewModel.SelectedPrice;
        }
        #endregion

        #region Implements

        #endregion

        #region Methods

        #region Init

        public void Init()
        {
            //seekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            //int progress = 1;
            //switch (int.Parse((ViewModel.SelectedValue * 10).ToString()))
            //{
            //    case 0:
            //        progress = 0; break;
            //    case 5:
            //        progress = 1; break;
            //    case 10:
            //        progress = 2; break;
            //    case 15:
            //        progress = 3; break;
            //    case 20:
            //        progress = 4; break;
            //    case 25:
            //        progress = 5; break;
            //    case 30:
            //        progress = 6; break;

            //}
            //seekBar.Progress = progress;

            //seekBar.ProgressChanged += (sender, args) =>
            //{
            //    float selectedValue = 0f;
            //    switch (args.Progress)
            //    {
            //        case 0:
            //            selectedValue = 0.0f; break;
            //        case 1:
            //            selectedValue = 0.5f; break;
            //        case 2:
            //            selectedValue = 1.0f; break;
            //        case 3:
            //            selectedValue = 1.5f; break;
            //        case 4:
            //            selectedValue = 2.0f; break;
            //        case 5:
            //            selectedValue = 2.5f; break;
            //        case 6:
            //            selectedValue = 3.0f; break;
            //    }

            //    ViewModel.SelectedValue = selectedValue;
            //};

        }

        #endregion

        #endregion
    }
}