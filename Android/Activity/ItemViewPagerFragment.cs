using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.UI.Droid.Activity;

namespace FlexyPark.UI.Droid.Resources.layout
{
    public class ItemViewPagerFragment : MvxFragment
    {
        #region UI Controls

        private TextView tvDistance, tvInstruction;

        string Distance = String.Empty;
        string Instruction = String.Empty;
        
        #endregion

        #region Variables

        #endregion

        #region Constructors

        public ItemViewPagerFragment(string distance, string instruction)
        {
            Distance = distance;
            Instruction = instruction;
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.ItemViewPager, container, false);
            tvDistance = view.FindViewById<TextView>(Resource.Id.distance);
            tvInstruction = view.FindViewById<TextView>(Resource.Id.instruction);

            if (!string.IsNullOrEmpty(Distance))
            {
                tvDistance.Text = Distance;
            }

            if (!string.IsNullOrEmpty(Instruction))
            {
                tvInstruction.Text = Instruction;
            }

            return view;
        }
        

        #endregion

        #region Implements

        #endregion

        #region Method

        #endregion
    }
}