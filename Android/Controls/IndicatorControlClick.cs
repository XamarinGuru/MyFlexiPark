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

namespace FlexyPark.UI.Droid.Controls
{
    public class IndicatorControlClick : TabPageIndicator
    {
        private bool mClickable = true;

        public IndicatorControlClick(Context context)
            : base(context)
        {
        }

        public IndicatorControlClick(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (mClickable)
            { return base.OnTouchEvent(e); }
            return true;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (mClickable)
            {
                return base.OnInterceptTouchEvent(ev);
            }
            else
            {
                return true;
            }
            
        }

        public void setClickable(bool mBool)
        {
            mClickable = mBool;
        }
    }
}