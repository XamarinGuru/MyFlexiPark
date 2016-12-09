using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.Binding.ExtensionMethods;
using FlexyPark.Core.ViewModels;
using LayoutDirection = Android.Views.LayoutDirection;

namespace FlexyPark.UI.Droid.Controls
{
    public class ListViewWithoutScrollView : MvxListView
    {
        private int totalHeight;


        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {
            base.AddView(child, index, @params);
        }


        public ListViewWithoutScrollView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public ListViewWithoutScrollView(Context context, IAttributeSet attrs, IMvxAdapter adapter)
            : base(context, attrs, adapter)
        {
        }

        protected ListViewWithoutScrollView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public void ReCalculateHeight()
        {
            if (ItemsSource != null && ItemsSource.Count() != 0)
            {
                totalHeight = 0;
                for (int i = 0; i < ItemsSource.Count(); i++)
                {
                    var item = Adapter.GetView(i, null, this);
                    item.Measure(MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                    totalHeight += item.MeasuredHeight;
                }
            }
        }

        public override ViewGroup.LayoutParams LayoutParameters
        {
            get { return base.LayoutParameters; }
            set
            {
                base.LayoutParameters = value;
                if (LayoutParameters != null && totalHeight!=0)
                {
                    LayoutParameters.Height = totalHeight;
                }
            }
        }
        

       
       
    }
}