//
//  
//  TPKeyboardAvoidingScrollView.cs
//  Xamarin port from michaeltyson/TPKeyboardAvoidingScrollView
//
//  Created by Duy Nguyen & Trung Nguyen on 05/19/2015.
//  Copyright 2015 Beesightsoft. All rights reserved.
//

using System;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using ObjCRuntime;

namespace Beesightsoft.TPKeyboardAvoiding
{
	[Register("TPKeyboardAvoidingScrollView")]
	public class TPKeyboardAvoidingScrollView : UIScrollView
	{
		public UITextViewDelegate TextViewDelegate ;
		public UITextFieldDelegate TextFieldDelegate ;

		public TPKeyboardAvoidingScrollView () : base()
		{
		}

		public TPKeyboardAvoidingScrollView (CGRect frame) : base(frame)
		{
			Setup ();
		}

		public TPKeyboardAvoidingScrollView (IntPtr handle) : base(handle)
		{
		}

		public override void AwakeFromNib ()
		{
			Setup ();
			base.AwakeFromNib ();
		}

		protected override void Dispose (bool disposing)
		{
			if(Observers!=null)
				NSNotificationCenter.DefaultCenter.RemoveObservers(Observers);
			
			base.Dispose (disposing);
		}

		List<NSObject> Observers;

		void Setup()
		{
			Observers = new List<NSObject> ();

			var obser1 = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillChangeFrameNotification, (obj) => this.TPKeyboardAvoiding_keyboardWillShow (obj)  );
			var obser2 = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, (obj) => this.TPKeyboardAvoiding_keyboardWillHide (obj) );
			var obser3 = NSNotificationCenter.DefaultCenter.AddObserver (UITextView.TextDidBeginEditingNotification, ScrollToActiveTextField );
			var obser4 = NSNotificationCenter.DefaultCenter.AddObserver (UITextField.TextDidBeginEditingNotification, ScrollToActiveTextField );

			Observers.Add (obser1);
			Observers.Add (obser2);
			Observers.Add (obser3);
			Observers.Add (obser4);

			TextFieldDelegate = new CustomTextFieldDelegate (this);
		}

		void SetFrame(CGRect frame)
		{
			base.Frame = frame;
			(this as UIScrollView).TPKeyboardAvoiding_updateContentInset ();
		}

		void SetContentSize(CGSize contentSize)
		{
			base.ContentSize = contentSize;
			(this as UIScrollView). TPKeyboardAvoiding_updateFromContentSizeChange();
		}

		void ContentSizeToFit()
		{
			ContentSize = (this as UIScrollView). TPKeyboardAvoiding_calculatedContentSizeFromSubviewFrames();
		}

		bool FocusNextTextField()
		{
			return (this as UIScrollView).TPKeyboardAvoiding_focusNextTextField ();
		}

		void ScrollToActiveTextField(NSNotification notification)
		{
			(this as UIScrollView).TPKeyboardAvoiding_scrollToActiveTextField ();
		}

		//TODO : TextField Should return???


		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			var view = this.TPKeyboardAvoiding_findFirstResponderBeneathView (this);
			if (view != null)
				view.ResignFirstResponder ();
			
			base.TouchesEnded (touches, evt);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			NSObject.CancelPreviousPerformRequest (this,  new Selector ("assignTextDelegateForViewsBeneathView"), this);
			this.TPKeyboardAvoiding_assignTextDelegateForViewsBeneathView (this);
		}


		public override void WillMoveToSuperview (UIView newsuper)
		{
			base.WillMoveToSuperview (newsuper);
			if( newsuper == null)
			{
				NSObject.CancelPreviousPerformRequest (this,  new Selector ("assignTextDelegateForViewsBeneathView"), this);
			}
		}

		[Export ("assignTextDelegateForViewsBeneathView")]
		public void assignTextDelegateForViewsBeneathView ()
		{
			(this as UIScrollView).TPKeyboardAvoiding_assignTextDelegateForViewsBeneathView (this);
		}
	}

	public class CustomTextFieldDelegate : UITextFieldDelegate
	{
		private UIScrollView scrollView;
		public CustomTextFieldDelegate(UIScrollView scrollView):base()
		{
			this.scrollView = scrollView;
		}

		public override bool ShouldReturn (UITextField textField)
		{
			if (! scrollView.TPKeyboardAvoiding_focusNextTextField () )
				textField.ResignFirstResponder ();

			return true;
		}
	}
}

