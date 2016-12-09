using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;

namespace FlexyPark.UI.Droid.Activity
{
    public class OwnFragment : BaseFragment
    {
        #region UI Controls


        #endregion

        #region Variables

        #endregion

        #region Constructors

        public OwnFragment()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().MyProfileViewModel.OwnVM;
           
        }

        public new OwnProfileViewModel ViewModel
        {
            get { return base.ViewModel as OwnProfileViewModel; }
            set
            {
                base.ViewModel = value;
            }
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.OwnFragment, container, false);
          
            SetButtonEffects(view, new List<int>()
            {
                Resource.Id.tvAddASpot
            });

            return view;
        }

        #endregion

        #region Implements

        #endregion

        #region Methods



        #endregion
       
    }
}