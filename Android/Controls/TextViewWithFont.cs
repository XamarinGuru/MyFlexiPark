using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Text;

namespace FlexyPark.UI.Droid.Controls
{
    public class TextViewWithFont: TextView
    {
        #region Constructors
        
       

        protected TextViewWithFont(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TextViewWithFont(Context context) : base(context)
        {
        }

        public TextViewWithFont(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public TextViewWithFont(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        #endregion

        #region Binding

        #region Font


        private string mFont;

        public string Font
        {
            get
            {
                return mFont;
            }
            set
            {
                mFont = value;
                if (!string.IsNullOrEmpty(mFont))
                {
                    SetFont();
                }
            }
        }

        #region TextColor


        private Android.Graphics.Color mTextColor;

        public Android.Graphics.Color TextColor
        {
            get
            {
                return mTextColor;
            }
            set
            {
                mTextColor = value;

                SetTextColor(value);
                
            }
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        public void SetFont()
        {
            Typeface mTypeface = Typeface.CreateFromAsset(this.Context.Assets, Font);
            SetTypeface(mTypeface,TypefaceStyle.Normal);
        }

        #endregion


    }
}