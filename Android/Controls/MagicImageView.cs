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
using Android.Views.Animations;
using Android.Widget;

namespace FlexyPark.UI.Droid.Controls
{
   public class MagicImageView: ImageView, View.IOnTouchListener
   {
       private Animation alphaAnim;
       protected MagicImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
       {
           SetOnTouchListener(this);
       }

       public MagicImageView(Context context) : base(context)
       {
           SetOnTouchListener(this);
       }

       public MagicImageView(Context context, IAttributeSet attrs) : base(context, attrs)
       {
           SetOnTouchListener(this);
       }

       public MagicImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
       {
           SetOnTouchListener(this);
       }

       public MagicImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
       {
           SetOnTouchListener(this);
       }


       public bool OnTouch(View v, MotionEvent e)
       {
           switch (e.Action)
           {
               case MotionEventActions.Down:
                   alphaAnim = new AlphaAnimation(1F, 0.5F) {Duration = 0, FillAfter = true};
                   StartAnimation(alphaAnim);
                   break;

               case MotionEventActions.Up:
                   alphaAnim = new AlphaAnimation(0.5F, 1F) {Duration = 0, FillAfter = true};
                   StartAnimation(alphaAnim);
                   break;

               case MotionEventActions.Cancel:
                   alphaAnim = new AlphaAnimation(0.5F, 1F) {Duration = 0, FillAfter = true};
                   StartAnimation(alphaAnim);
                   break;
           }
           return false;
       }

      
    }
}