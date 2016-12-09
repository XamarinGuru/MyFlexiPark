using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Splat;
using UrlImageViewHelper;

namespace FlexyPark.UI.Droid.Controls
{
    public class BindableImageView : ImageView
    {
        #region Constructors
        protected BindableImageView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public BindableImageView(Context context)
            : base(context)
        {
        }

        public BindableImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public BindableImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
        }
        #endregion

        #region Bindings

        #region ImageResource


        private int mImageResource;

        public int ImageResource
        {
            get
            {
                return mImageResource;
            }
            set
            {
                mImageResource = value;
                SetImageResource(value);
            }
        }

        #endregion

        #region Drawable





        private Drawable mDrawable;
        public Drawable Drawable
        {
            get { return mDrawable; }
            set
            {
                mDrawable = value;

                if (mDrawable != null)
                {
                    SetImageDrawable(mDrawable);
                }
            }
        }

        #endregion

        #region ImageUrl
        private string mImageUrl;
        public string ImageUrl
        {
            get { return mImageUrl; }
            set
            {
                mImageUrl = value;
                SetImage();
            }
        }
        #endregion

        #region Image Bitmap
        private IBitmap mImageBitmap;
        public IBitmap ImageBitmap
        {
            get { return mImageBitmap; }
            set
            {
                mImageBitmap = value;
                SetImage();
            }
        }
        #endregion

        #region Image Path
        private string mPathImage;
        public string PathImage
        {
            get { return mPathImage; }
            set
            {
                mPathImage = value;
                SetImage();
            }
        }
        #endregion

        #region Bytes


        private byte[] mBytes;

        public byte[] Bytes
        {
            get
            {
                return mBytes;
            }
            set
            {
                mBytes = value;
                if (mBytes != null)
                {
                    Bitmap bm = BitmapFactory.DecodeByteArray(mBytes, 0, mBytes.Length);

                    if (bm != null)
                    {
                        SetImageBitmap(bm);
                    }
                }
            }
        }

        #endregion

        private void SetImage()
        {
            if (ImageBitmap != null)
            {
                SetImageDrawable(mImageBitmap.ToNative());
                SetScaleType(ScaleType.FitXy);
            }
            else if (!String.IsNullOrEmpty(mImageUrl))
            {
                DestroyDrawingCache();
                this.SetUrlDrawable(mImageUrl, Resources.GetDrawable(Android.Resource.Color.Transparent));

                //SetScaleType(ScaleType.FitXy);

            }
            else if (!string.IsNullOrEmpty(PathImage))
            {
                Drawable drawable = Drawable.CreateFromPath(PathImage);
                SetImageDrawable(drawable);
                //SetScaleType(ScaleType.FitXy);

            }
            else
            {
                SetImageResource(Android.Resource.Color.Transparent);

            }
        }


        #endregion
    }
}