using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Uri = Android.Net.Uri;


namespace cFlexyPark.UI.Droid.Extendsions
{
    public static class BitmapExtension
    {
        public static Bitmap Square(this Bitmap bitmap, bool fromTop = false)
        {
            if (!fromTop)
            {
                if (bitmap.Width >= bitmap.Height)
                {
                    return Bitmap.CreateBitmap(
                        bitmap,
                        bitmap.Width / 2 - bitmap.Height / 2,
                        0,
                        bitmap.Height,
                        bitmap.Height
                        );
                }
                else
                {
                    return Bitmap.CreateBitmap(
                        bitmap,
                        0,
                        bitmap.Height / 2 - bitmap.Width / 2,
                        bitmap.Width,
                        bitmap.Width
                        );
                }
            }
            else
            {
                return bitmap.Width >= bitmap.Height
                    ? Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Height, bitmap.Height)
                    : Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Width);
            }
        }

        public static Bitmap Resize(this Bitmap bitmap, int newWidth, int newHeight)
        {
            Bitmap scaledBitmap = Bitmap.CreateBitmap(newWidth, newHeight, Bitmap.Config.Argb8888);

            float ratioX = newWidth / (float)bitmap.Width;
            float ratioY = newHeight / (float)bitmap.Height;
            float middleX = newWidth / 2.0f;
            float middleY = newHeight / 2.0f;

            Matrix scaleMatrix = new Matrix();
            scaleMatrix.SetScale(ratioX, ratioY, middleX, middleY);

            Canvas canvas = new Canvas(scaledBitmap);
            canvas.Matrix = scaleMatrix;
            canvas.DrawBitmap(bitmap, middleX - bitmap.Width / 2, middleY - bitmap.Height / 2, new Paint(PaintFlags.FilterBitmap));
            return scaledBitmap;
        }

        public static Bitmap ResizeBitmap720x1280(this Bitmap bitmap)
        {

            if (bitmap.Width > 720 || bitmap.Height > 1280)
            {
                if (bitmap.Width > bitmap.Height)
                {
                    var newWidth = 720;
                    var newHeight = bitmap.Height / (bitmap.Width / 720f);
                    bitmap = bitmap.Resize(newWidth, (int)newHeight);
                }
                else
                {
                    var newHeight = 1280;
                    var newWidth = bitmap.Width / (bitmap.Height / 1280f);
                    bitmap = bitmap.Resize((int)newWidth, newHeight);
                }
            }

            return bitmap;
        }
        public static Bitmap ResizeBitmap1280x720(this Bitmap bitmap)
        {

            if (bitmap.Width > 1280 || bitmap.Height > 720)
            {
                if (bitmap.Width > bitmap.Height)
                {
                    var newWidth = 1280;
                    var newHeight = bitmap.Height / (bitmap.Width / 1280f);
                    bitmap = bitmap.Resize(newWidth, (int)newHeight);
                }
                else
                {
                    var newHeight = 720;
                    var newWidth = bitmap.Width / (bitmap.Height / 720f);
                    bitmap = bitmap.Resize((int)newWidth, newHeight);
                }
            }

            return bitmap;
        }

        public static Bitmap ResizeBitmap(this Bitmap bitmap, int size)
        {

            if (bitmap.Width > size || bitmap.Height > size)
            {
                if (bitmap.Width > bitmap.Height)
                {
                    var newWidth = size;
                    var newHeight = bitmap.Height / (bitmap.Width / size);
                    bitmap = bitmap.Resize(newWidth, (int)newHeight);
                }
                else
                {
                    var newHeight = size;
                    var newWidth = bitmap.Width / (bitmap.Height / size);
                    bitmap = bitmap.Resize((int)newWidth, newHeight);
                }
            }

            return bitmap;
        }

        public static int ExifToDegrees(Orientation exifOrientation)
        {

            if (exifOrientation == Orientation.Rotate90) { return 90; }
            else if (exifOrientation == Orientation.Rotate180) { return 180; }
            else if (exifOrientation == Orientation.Rotate270) { return 270; }



            return 0;
        }

        public static string GetRealPathFromUri(Context context, Android.Net.Uri contentUri)
        {
            var uriString = Java.Lang.String.ValueOf(contentUri);
            var goForKitKat = uriString.Contains("com.android.providers");

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat && goForKitKat)
            {
                return GetPathForV19AndUp(context, contentUri);
            }
            else
            {

                return GetPathForPreV19(context, contentUri);
            }
        }

        public static string GetPathForPreV19(Context context, Android.Net.Uri contentUri)
        {
            var proj = new[] { MediaStore.Images.Media.InterfaceConsts.Data };
            var cursor = context.ContentResolver.Query(contentUri, proj, null, null, null);

            if (cursor != null)
            {
                string res = null;

                if (cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                    res = cursor.GetString(column_index);
                }

                cursor.Close();

                if (string.IsNullOrEmpty(res))
                {
                    return contentUri.Path;
                }
                else
                {
                    return res;
                }
            }
            else
            {
                return contentUri.Path;
            }
        }
        public static string GetPathForV19AndUp(Context context, Uri contentUri)
        {
            var wholeId = DocumentsContract.GetDocumentId(contentUri);

            // Split at colon, use second item in the array
            var id = wholeId.Split(':')[1];
            var column = new[] { MediaStore.Images.Media.InterfaceConsts.Data };

            // where id is equal to
            var sel = MediaStore.Images.Media.InterfaceConsts.Id + "=?";
            var cursor = context.ContentResolver.
                Query(MediaStore.Images.Media.ExternalContentUri,
                    column, sel, new[] { id }, null);

            if (cursor != null)
            {
                var filePath = "";
                var columnIndex = cursor.GetColumnIndex(column[0]);

                if (cursor.MoveToFirst())
                {
                    filePath = cursor.GetString(columnIndex);
                }

                cursor.Close();

                if (string.IsNullOrEmpty(filePath))
                {
                    return contentUri.Path;
                }
                else
                {
                    return filePath;
                }
            }
            else
            {
                return contentUri.Path;
            }
        }

        public static Bitmap DecodeSampledBitmapFromResource(string path, int reqWidth, int reqHeight)
        {
            using (var options = new BitmapFactory.Options())
            {
                options.InJustDecodeBounds = true;

                BitmapFactory.DecodeFile(path, options);

                options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

                options.InJustDecodeBounds = false;

                return BitmapFactory.DecodeFile(path, options);
            }
        }

        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > reqHeight
                       && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }
    }



}