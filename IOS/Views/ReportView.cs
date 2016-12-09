
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using System.Threading.Tasks;
using WYPopoverControllerBinding;
using CoreGraphics;
using FlexyPark.Core;
using FlexyPark.UI.Touch.Helpers;
using System.Collections.Generic;
using AssetsLibrary;
using FlexyPark.UI.Touch.Extensions;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ReportView : BaseView, IReportView
    {
        public ReportView()
            : base("ReportView", null)
        {
        }

        public new ReportViewModel ViewModel
        {
            get
            {
                return base.ViewModel as ReportViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (ivPic.Image != null)
            {
                ivPic.Image.Dispose();
                ivPic.Image = null;
            }

            if (ivPic2.Image != null)
            {
                ivPic2.Image.Dispose();
                ivPic2.Image = null;
            }

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            //HideBackButton();
            SetTitle("Report");
            // Perform any additional setup after loading the view, typically from a nib.

            ViewModel.View = this;

            var set = this.CreateBindingSet<ReportView, ReportViewModel>();

            set.Bind(btnTakePic).To(vm => vm.AddPictureCommand);
            set.Bind(ivPic).To(vm => vm.Image).WithConversion("BytesToUIImage");
            set.Bind(tvProblem).To(vm => vm.Problem);

            set.Bind(btnTakePic2).To(vm => vm.AddPictureCommand);
            set.Bind(ivPic2).To(vm => vm.Image).WithConversion("BytesToUIImage");
            set.Bind(tvProblem2).To(vm => vm.Problem);

            set.Bind(tfPlateNumber).To(vm => vm.PlateNumber);

            set.Bind(btnOkFull).To(vm => vm.SendReportCommand);
            set.Bind(btnOkLeave).To(vm => vm.SendReportCommand);
            set.Bind(btnOkRefuse).To(vm => vm.SendReportCommand);
            set.Bind(btnOkPlateNumber).To(vm => vm.SendReportCommand);
            set.Bind(btnOkRefund).To(vm => vm.SendReportCommand);

            set.Bind(btnCallOwner).To(vm => vm.CallOwnerCommand);


            set.Bind(vPictureRefuse).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "PictureRefuse");
            set.Bind(vPictureLeave).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "PictureLeave");
            set.Bind(vPlateNumber).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "PlateNumber");
            set.Bind(vCallOwner).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "CallOwner");
            set.Bind(vFull).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "Full");
            set.Bind(vRefund).For(v => v.Hidden).To(vm => vm.Mode).WithConversion("ReportModeToBoolean", "Refund");


            #region Language Binding

            set.Bind(btnTakePic).For("Title").To(vm => vm.TextSource).WithConversion("Language", "TakeAPictureText");
            set.Bind(btnTakePic2).For("Title").To(vm => vm.TextSource).WithConversion("Language", "TakeAPictureText");

            set.Bind(lbReportProblem).To(vm => vm.TextSource).WithConversion("Language", "ReportAProblemText");
            set.Bind(lbPlateNumber).To(vm => vm.TextSource).WithConversion("Language", "PlateNumberText");
            set.Bind(lbProblem).To(vm => vm.TextSource).WithConversion("Language", "ProblemText");
            set.Bind(lbProblem2).To(vm => vm.TextSource).WithConversion("Language", "ProblemText");

            set.Bind(lbPleaseContact).To(vm => vm.TextSource).WithConversion("Language", "PleaseContactText");
            set.Bind(lbNewParkingFull).To(vm => vm.TextSource).WithConversion("Language", "NewParkingSpotText");
            set.Bind(lbNewParkingRefuse).To(vm => vm.TextSource).WithConversion("Language", "NewParkingSpotText");
            set.Bind(lbRefund).To(vm => vm.TextSource).WithConversion("Language", "YouWillBeRefundText");
            set.Bind(lbRefundPlateNumber).To(vm => vm.TextSource).WithConversion("Language", "YouWillBeRefundText");

            #endregion

            set.Apply();


            #region UI Settings

            this.AutomaticallyAdjustsScrollViewInsets = false;

            #endregion
        }



        #region IReportView implementation

        public Task<byte[]> TakePicture()
        {
            var tcs = new TaskCompletionSource<byte[]>();

            TweetStation.Camera.TakePicture(this, (obj) =>
                {
                    UIImage originalImage = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                    if (originalImage != null)
                    {
                        // do something with the image
                        Console.WriteLine("got the original image");

                        //TODO : take care of the image size

                        var jpegData = originalImage.AsJPEG(0.5f);

                        originalImage.Dispose();
                        originalImage = null;

                        var buffer = jpegData.ToArray();
                        jpegData.Dispose();
                        jpegData = null;

                        tcs.TrySetResult(buffer);
                    }
                });

            return tcs.Task;
        }

        public Task<byte[]> SelectFromLibrary()
        {
            var tcs = new TaskCompletionSource<byte[]>();

            TweetStation.Camera.SelectPicture(this, (obj) =>
                {
                    UIImage originalImage = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                    if (originalImage != null)
                    {
                        // do something with the image
                        Console.WriteLine("got the original image");

                        //TODO : take care of the image size

                        var jpegData = originalImage.AsJPEG(0.5f);

                        originalImage.Dispose();
                        originalImage = null;

                        var buffer = jpegData.ToArray();
                        jpegData.Dispose();
                        jpegData = null;

                        tcs.TrySetResult(buffer);
                    }
                });

            return tcs.Task;
        }

        public void SavePictureToLibrary(byte[] imgData)
        {
            ALAssetsLibrary library = new ALAssetsLibrary();
            library.WriteImageToSavedPhotosAlbum(imgData.ToImage().CGImage, new NSDictionary(), (assetUrl, error) =>
                {
                    Console.WriteLine("assetUrl:" + assetUrl);
                });
        }

        public void ConfigHeight(int index)
        {
            cstContentHeight.Constant = (index == 7 || index == 9) ? 640f : UIScreen.MainScreen.Bounds.Height;   
        }

        public void CallOwner(string phoneNumber)
        {
            var url = new NSUrl("tel:" + phoneNumber);
            if (UIApplication.SharedApplication.CanOpenUrl(url))
                UIApplication.SharedApplication.OpenUrl(url);
            else
            {
                var av = new UIAlertView("Not supported",
                             "Scheme 'tel:' is not supported on this device",
                             null,
                             "OK",
                             null);
                av.Show();
            }
        }

        #endregion
    }


}

