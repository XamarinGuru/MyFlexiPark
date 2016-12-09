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
    public class TextNeueBold: TextView
    {
        #region Constructors
        
       

        protected TextNeueBold(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Font = "helvetica-neue-bold.ttf";
        }

        public TextNeueBold(Context context) : base(context)
        {
            Font = "helvetica-neue-bold.ttf";
        }

        public TextNeueBold(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Font = "helvetica-neue-bold.ttf";
        }

        public TextNeueBold(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Font = "helvetica-neue-bold.ttf";
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