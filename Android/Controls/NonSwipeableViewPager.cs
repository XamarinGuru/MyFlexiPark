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

namespace FlexyPark.UI.Droid.Controls
{
    public class NonSwipeableViewPager: ViewPager
    {

        public NonSwipeableViewPager(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {

        }

        public NonSwipeableViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.Enabled = true;
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            if (this.Enabled)
            {
                return base.OnTouchEvent(e);
            }
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (this.Enabled)
            {
                return base.OnInterceptTouchEvent(ev);
            }
            return false;
        }

        public void SetPagingEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }
    }
}