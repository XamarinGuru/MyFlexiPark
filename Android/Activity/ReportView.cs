
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using cFlexyPark.UI.Droid.Extendsions;
using Cirrious.CrossCore;
using FlexyPark.Core;
using FlexyPark.Core.ViewModels;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using FileNotFoundException = Java.IO.FileNotFoundException;
using Orientation = Android.Media.Orientation;
using Uri = Android.Net.Uri;

namespace FlexyPark.UI.Droid.Activity
{

    [Activity(Label = "ReportView", MainLauncher = false, ScreenOrientation = ScreenOrientation.SensorPortrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class ReportView : BaseView, IReportView
    {
        #region UI Controls

        #endregion


        #region Variables
        public const string ImagesFolder = "FlexyParkImages";
        private int SelectPictureRequestCode = 1234;
        private int TakePictureRequestCode = 1233;
        private TaskCompletionSource<byte[]> Tcs;
        private File PhotoFile;

        public static File tmpPath;


        #endregion

        #region Constructors

        public new ReportViewModel ViewModel
        {
            get { return base.ViewModel as ReportViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReportView);
            ViewModel.View = this;
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvReport,
                Resource.Id.tvTakeAPicture
            });


        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (resultCode == Result.Ok)
                {
                    if (requestCode == SelectPictureRequestCode)
                    {

                        if (data == null)
                        {
                            Tcs.SetResult(null);
                            Tcs = null;
                            return;
                        }

                        try
                        {
                            var input = ContentResolver.OpenInputStream(data.Data);
                            var bitmap = BitmapFactory.DecodeStream(input);

                            if (bitmap == null)
                            {
                                Tcs.SetResult(null);
                                Tcs = null;
                                return;
                            }

                            var path = BitmapExtension.GetRealPathFromUri(this, data.Data);

                            var exif = new ExifInterface(path);

                            var rotation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Undefined);
                            var rotationInDegrees = BitmapExtension.ExifToDegrees(rotation);
                            var matrix = new Matrix();

                            if (rotation != Orientation.Normal && rotationInDegrees != 0)
                            {

                                var original = bitmap;
                                matrix.PreRotate(rotationInDegrees);
                                var rotatedBitmap = Bitmap.CreateBitmap(original, 0, 0, original.Width, original.Height, matrix, true);

                                bitmap = rotatedBitmap;

                                original.Recycle();
                                original.Dispose();

                            }

                            if (bitmap.Width > 720 || bitmap.Height > 1280)
                            {
                                bitmap = bitmap.ResizeBitmap720x1280();
                            }


                            var bytes = Bytes(bitmap);

                            bitmap.Recycle();

                            Tcs.SetResult(bytes);
                            Tcs = null;
                        }
                        catch (Exception e)
                        {
                            Tcs.SetResult(null);
                            Tcs = null;
                        }
                    }
                    else if (requestCode == TakePictureRequestCode)
                    {
                        if (PhotoFile != null)
                        {
                            var bitmap = LoadImage(this, ImagesFolder, PhotoFile.Name);

                            // Update it available in gallery
                            if (PhotoFile != null)
                            {
                                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                                Uri contentUri = Uri.FromFile(PhotoFile);
                                mediaScanIntent.SetData(contentUri);
                                SendBroadcast(mediaScanIntent);
                            }

                            //

                            if (bitmap != null)
                            {
                                var path = GetImagesPath(this, ImagesFolder) + "/";

                                var exif = new ExifInterface(path + PhotoFile.Name);
                                var matrix = new Matrix();

                                var orientation =
                                    (Orientation)
                                        exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

                                switch (orientation)
                                {
                                    case Orientation.Undefined: // Nexus 7 landscape...
                                        break;
                                    case Orientation.Normal: // landscape
                                        break;
                                    case Orientation.FlipHorizontal:
                                        break;
                                    case Orientation.Rotate180:
                                        matrix.PreRotate(180);
                                        bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix,
                                            false);
                                        matrix.Dispose();
                                        matrix = null;
                                        break;
                                    case Orientation.FlipVertical:
                                        break;
                                    case Orientation.Transpose:
                                        break;
                                    case Orientation.Rotate90: // portrait
                                        matrix.PreRotate(90);
                                        bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix,
                                            false);
                                        matrix.Dispose();
                                        matrix = null;
                                        break;
                                    case Orientation.Transverse:
                                        break;
                                    case Orientation.Rotate270: // might need to flip horizontally too...
                                        matrix.PreRotate(270);
                                        bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix,
                                            false);
                                        matrix.Dispose();
                                        matrix = null;
                                        break;
                                    default:
                                        matrix.PreRotate(90);
                                        bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix,
                                            false);
                                        matrix.Dispose();
                                        matrix = null;
                                        break;
                                }



                                if (bitmap.Width > 720 || bitmap.Height > 1280)
                                {
                                    bitmap = bitmap.ResizeBitmap720x1280();
                                }

                                var bytes = Bytes(bitmap);

                                bitmap.Recycle();
                                bitmap.Dispose();
                                bitmap = null;

                                //PhotoFile.Delete();
                                PhotoFile = null;

                                Tcs.SetResult(bytes);
                                Tcs = null;
                            }
                        }
                        else
                        {
                            Tcs.SetResult(null);
                            Tcs = null;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        #endregion

        #region Implements

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> TakePicture()
        {
            Tcs = new TaskCompletionSource<byte[]>();

            var takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            // Ensure that there's a camera activity to handle the intent
            if (takePictureIntent.ResolveActivity(PackageManager) != null)
            {
                // Create the File where the photo should go
                PhotoFile = GetOutputMediaFile(this, ImagesFolder);
                // Continue only if the File was successfully created
                if (PhotoFile != null)
                {
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput,
                            Uri.FromFile(PhotoFile));
                    StartActivityForResult(takePictureIntent, TakePictureRequestCode);
                }
            }

            return Tcs.Task;
        }

        public Task<byte[]> SelectFromLibrary()
        {
            Tcs = new TaskCompletionSource<byte[]>();

            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            intent.AddCategory(Intent.CategoryOpenable);
            StartActivityForResult(Intent.CreateChooser(intent, "Select From Library"),
                  SelectPictureRequestCode);
            return Tcs.Task;
        }

        public void SavePictureToLibrary(byte[] imgData)
        {
            //var bitmap = BitmapFactory.DecodeByteArray(imgData, 0, imgData.Length);
            //if (bitmap != null)
            //{
            //    MediaStore.Images.Media.InsertImage(ContentResolver, bitmap, string.Empty,
            //        string.Empty);
            //}
        }

        public void ShowPicker()
        {

        }

        public void CallOwner(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string uri = "tel:" + phoneNumber.Trim();
                StartActivity(new Intent(Intent.ActionCall, Uri.Parse(uri)));
            }
        }

        public void ConfigHeight(int index)
        {

        }

        #endregion

        #region Methods

        public static File GetOutputMediaFile(Context context, string imagesFolder, string fileExtension = "png")
        {
            if (Environment.ExternalStorageState != Environment.MediaMounted)
                return null;

            var mediaStorageDir = new File(GetImagesPath(context, imagesFolder));

            if (!mediaStorageDir.Exists())
            {
                try
                {
                    var x = mediaStorageDir.Mkdirs();
                }
                catch (Exception ex)
                {


                }
                if (!mediaStorageDir.Mkdirs())
                    return null;
            }

            // Create a media file name
            var timeStamp = DateTime.UtcNow.ToString("ddMMyyyy_HHmmss");
            var mImageName = timeStamp + "." + fileExtension;
            var mediaFile = new File(mediaStorageDir.Path + File.Separator + mImageName);
            tmpPath = mediaFile;
            return mediaFile;
        }

        public static string GetImagesPath(Context context, string imagesFolder)
        {
            if (Environment.ExternalStorageState == Environment.MediaMounted)
            {
                return Environment.ExternalStorageDirectory
                       + "/Android/data"
                + context.PackageName
                + "/"
                 + imagesFolder;
            }
            else
            {
                return string.Empty;
            }
        }

        public static byte[] Bytes(Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
            return stream.ToArray();
        }
        public static Bitmap LoadImage(Context context, string imagesFolder, string imageFileName)
        {
            if (Environment.ExternalStorageState != Environment.MediaMounted)
                return null;

            var mPath = GetImagesPath(context, imagesFolder) + File.Separator + imageFileName;
            return BitmapFactory.DecodeFile(mPath);
        }
        #endregion


    }
}