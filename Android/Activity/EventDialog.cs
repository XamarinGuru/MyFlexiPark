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
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;

namespace FlexyPark.UI.Droid.Activity
{
    // [Activity(Theme = "@style/Theme.UserDialog", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize)]
    public class EventDialog : MvxDialogFragment
    {
        #region UI Controls

        private TextView tvEdit;
        private AddSpotCalendarViewModel AddSpotCalendarViewModelVM;

        #endregion

        public EventDialog()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel;

        }



        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            base.EnsureBindingContextSet(savedInstanceState);
            AddSpotCalendarViewModelVM = ViewModel as AddSpotCalendarViewModel;
        

            var view = this.BindingInflate(Resource.Layout.EventDialog, null);
            var dialog = new Dialog(Activity);


            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

            dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            dialog.SetContentView(view);
            tvEdit = dialog.FindViewById<TextView>(Resource.Id.tvEdit);

            if (AddSpotCalendarViewModelVM.IsEditMode)
            {
                tvEdit.Text = "Cancel";
            }
            else
            {
                tvEdit.Text = "Edit";
            }
          
            tvEdit.Click += (sender, args) =>
            {
                if (tvEdit.Text == "Edit")
                {
                    tvEdit.Text = "Cancel";
                }
                else
                {
                    tvEdit.Text = "Edit";
                }

            };

            return dialog;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
         
        }
        

        #region Implements



        #endregion
    }
}