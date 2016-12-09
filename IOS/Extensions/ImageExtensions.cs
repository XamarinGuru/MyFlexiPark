using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using Foundation;

namespace FlexyPark.UI.Touch.Extensions
{
	public static class ImageExtensions
	{

		public static byte[] ToNSData(this UIImage image)
		{
			if (image == null) {
				return null;
			}
			NSData data = null;

			try {
				data = image.AsPNG();
				return data.ToArray ();
			} catch (Exception ) {
				return null;
			}
			finally
			{
				if (image != null) {
					image.Dispose ();
					image = null;
				}
				if (data != null) {
					data.Dispose ();
					data = null;
				}
			}
		}

		public static UIImage ToImage(this byte[] data)
		{
			if (data==null) 
			{
				return null;
			}

			UIImage image = null;
			try 
			{
				var buffer = NSData.FromArray(data);
				image = new UIImage(buffer);

				data = null;
				buffer.Dispose ();
				buffer = null;
			} 
			catch (Exception ex) 
			{
				return null;
			}

			return image;
		}

        public static UIImage DrawImageOnImage(this UIImage belowImage, UIImage aboveImage, float finalSize)
        {
            //make a new square size, that is the resized imaged width
            CGSize size = new CGSize (finalSize, finalSize);

            UIGraphics.BeginImageContext(size);

            UIImage newImage = null;
            using ( var context = UIGraphics.GetCurrentContext())
            {
                belowImage.Draw(new CGRect(0,0,size.Width, size.Height));
                aboveImage.Draw(new CGRect(size.Width/2-aboveImage.Size.Width/2, size.Height/2-aboveImage.Size.Height/2, aboveImage.Size.Width, aboveImage.Size.Height));
                newImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            return newImage;
        }

		public static UIImage SquareImageAndScaleToSize(this UIImage image, float newSize)
		{
			double ratio;
			double delta;
			CGPoint offset;

			//make a new square size, that is the resized imaged width
			CGSize size = new CGSize (newSize, newSize);

			//figure out if the picture is landscape or portrait, then calculate scale factor and offset
			if(image.Size.Width > image.Size.Height)
			{
				ratio = newSize / image.Size.Width;
				delta = (ratio * image.Size.Width - ratio * image.Size.Height);
				offset = new CGPoint (delta / 2, 0);
			}
			else
			{
				ratio = newSize / image.Size.Height;
				delta = (ratio * image.Size.Height - ratio * image.Size.Width);
				offset = new CGPoint (0, delta / 2);
			}

			//make the final clipping rect based on the calculated values
            CGRect clipRect = new CGRect (-offset.X, -offset.Y/8 , (ratio * image.Size.Width) + delta, (ratio * image.Size.Height) + delta * 1.5   );

			//start a new context, with scale factor 0.0 so retina displays get high quality image
			if(UIScreen.MainScreen.RespondsToSelector (new ObjCRuntime.Selector("scale")))
			{
				UIGraphics.BeginImageContextWithOptions (size,true,0.0f);
			}
			else
			{
				UIGraphics.BeginImageContext (size);
			}

			UIImage newImage = null;
			using (CGContext context = UIGraphics.GetCurrentContext ()) 
			{
				context.ClipToRect (clipRect);
				image.Draw (clipRect);
				newImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
			}

			return newImage;
		}

		public static UIImage ScaleAndRotateImage (this UIImage image, int maxResolution)
		{
			//CGImage imgRef = image.CGImage;
			using(CGImage imgRef = image.CGImage){
				
			float width = imgRef.Width;
			float height = imgRef.Height;

			//int kMaxResolution = (int)Math.Max (width, height); 
            // use current size of picture (not scale or resize)
			int kMaxResolution = maxResolution; // based on customer's idea


			CGAffineTransform transform = CGAffineTransform.MakeIdentity ();
			CGRect bounds = new CGRect (0, 0, width, height);
			if (width > kMaxResolution || height > kMaxResolution)
			{
				float ratio = width / height;
				if (ratio > 1)
				{
//					bounds.Size = new CGSize (kMaxResolution, bounds.Size.Width / ratio);
					bounds.Size = new CGSize (kMaxResolution, kMaxResolution / ratio);
				}
				else
				{
//					bounds.Size = new CGSize (bounds.Size.Height * ratio, kMaxResolution);
					bounds.Size = new CGSize (kMaxResolution * ratio, kMaxResolution);
				}
			}

			float scaleRatio = (float)bounds.Size.Width / width;
			CGSize imageSize = new CGSize (imgRef.Width, imgRef.Height);
			float boundHeight;
				UIImageOrientation orient = image.Orientation;
			switch (orient)
			{

			case UIImageOrientation.Up: //EXIF = 1
				transform = CGAffineTransform.MakeIdentity ();
				break;

			case UIImageOrientation.UpMirrored: //EXIF = 2
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, 0.0f);
				transform = CGAffineTransform.Scale (transform, -1.0f, 1.0f);
				break;

			case UIImageOrientation.Down: //EXIF = 3
				transform = CGAffineTransform.MakeTranslation (imageSize.Width, imageSize.Height);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI);
				break;

			case UIImageOrientation.DownMirrored: //EXIF = 4
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Height);
				transform = CGAffineTransform.Scale (transform, 1.0f, -1.0f);
				break;

			case UIImageOrientation.LeftMirrored: //EXIF = 5
				boundHeight = (float)bounds.Size.Height;
				bounds.Size = new SizeF (boundHeight, (float)bounds.Size.Width);
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
				transform = CGAffineTransform.Scale (transform, -1.0f, 1.0f);
				transform = CGAffineTransform.Rotate (transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Left: //EXIF = 6
				boundHeight = (float)bounds.Size.Height;
				bounds.Size = new SizeF (boundHeight,(float) bounds.Size.Width);
				transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
				transform = CGAffineTransform.Rotate (transform, 3.0f * (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.RightMirrored: //EXIF = 7
				boundHeight = (float)bounds.Size.Height;
				bounds.Size = new SizeF (boundHeight, (float)bounds.Size.Width);
				transform = CGAffineTransform.MakeScale (-1.0f, 1.0f);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI / 2.0f);
				break;

			case UIImageOrientation.Right: //EXIF = 8
				boundHeight = (float)bounds.Size.Height;
				bounds.Size = new SizeF (boundHeight, (float)bounds.Size.Width);
				transform = CGAffineTransform.MakeTranslation (imageSize.Height, 0.0f);
				transform = CGAffineTransform.Rotate (transform, (float)Math.PI / 2.0f);
				break;

			default:
				break;

			}

			UIGraphics.BeginImageContext (bounds.Size);

				using (CGContext context = UIGraphics.GetCurrentContext ()) {

					if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left) {
						context.ScaleCTM (-scaleRatio, scaleRatio);
						context.TranslateCTM (-height, 0);
					} else {
						context.ScaleCTM (scaleRatio, -scaleRatio);
						context.TranslateCTM (0, -height);
					}

					context.ConcatCTM (transform);

					context.DrawImage (new CGRect (0, 0, width, height), imgRef);
					UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
					
						UIGraphics.EndImageContext ();
						return imageCopy;

				}
			}

		}

		public static UIImage ImageFromColorAndSize(UIColor color, CGSize size)
		{
			UIImage img = null;
				CGRect rect = new CGRect (0, 0, size.Width, size.Height);
				UIGraphics.BeginImageContextWithOptions (rect.Size,false,0);
				CGContext context = UIGraphics.GetCurrentContext ();
				context.SetFillColor (color.CGColor);
				context.FillRect (rect);
				img = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();

				context.Dispose ();
				context = null;

				return img;
		}

		public static UIColor AverageColor(this UIImage image)
		{
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			
			byte[] rgba = new byte[4];
			CGContext context = new CGBitmapContext(rgba, 1, 1, 8, 4, colorSpace, CGImageAlphaInfo.PremultipliedLast );

			context.DrawImage( new CGRect(0, 0, 1, 1), image.CGImage);
			colorSpace.Dispose ();
			colorSpace = null;
			context.Dispose ();
			context = null;

			if(rgba[3] > 0) {
				float alpha = ((float)rgba[3])/255.0f;
				float multiplier = alpha/255.0f;
				UIColor color = new UIColor(((nfloat)rgba[0])*multiplier,((nfloat)rgba[1])*multiplier,((nfloat)rgba[2])*multiplier,alpha);
				return color;
			}
			else {
				UIColor color = new UIColor(((nfloat)rgba[0])*255.0f,((nfloat)rgba[1])*255.0f,((nfloat)rgba[2])*255.0f,((nfloat)rgba[3])*255.0f);
				return color;
			}
		}


	}
}

