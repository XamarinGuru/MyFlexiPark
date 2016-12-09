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
    public class LinearLayoutClickable:LinearLayout
    {
        protected LinearLayoutClickable(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LinearLayoutClickable(Context context) : base(context)
        {
        }

        public LinearLayoutClickable(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public LinearLayoutClickable(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }


        #region IsClickable


        private bool mIsClickable;

        public bool IsClickable
        {
            get
            {
                return mIsClickable;
            }
            set
            {
                mIsClickable = value;
                setClickAble(value);
            }
        }

        #endregion

        public void setClickAble(bool IsClickAble)
        {
            Clickable = !IsClickAble;
        }
    }
}