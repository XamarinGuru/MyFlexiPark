using System;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreFoundation;

namespace Beesightsoft.TPKeyboardAvoiding
{
	public  static class UIScrollViewExtensions
	{
		const float kCalculatedContentPadding = 10;
		const float kMinimumScrollOffsetPadding = 20;

		static TPKeyboardAvoidingState State = null;
		const double DBL_EPSILON = 2.2204460492503131E-16;

		static NSString _UIKeyboardFrameEndUserInfoKey()
		{
			return UIKeyboard.FrameEndUserInfoKey.Handle != null ? UIKeyboard.FrameEndUserInfoKey : new NSString("UIKeyboardFrameEndUserInfoKey");
		}

		static bool FEqual(float a, float b)
		{
			return Math.Abs (a-b) < DBL_EPSILON;
		}

		/*static TPKeyboardAvoidingState KeyboardAvoidingState()
		{
			TPKeyboardAvoidingState state = State;
			if(state == null)
				state = new TPKeyboardAvoidingState();

			return state;
		}*/

		public static void TPKeyboardAvoiding_keyboardWillShow(this UIScrollView scrollView, NSNotification notification)
		{
			CGRect keyboardRect = scrollView.ConvertRectFromView ((notification.UserInfo.ObjectForKey (UIKeyboard.FrameEndUserInfoKey) as NSValue).CGRectValue, null);

			if (keyboardRect == CGRect.Empty)
				return;

			//TPKeyboardAvoidingState state = KeyboardAvoidingState();
			if (State == null)
				State = new TPKeyboardAvoidingState ();

			UIView firstResponder = scrollView.TPKeyboardAvoiding_findFirstResponderBeneathView(scrollView);

			if ( firstResponder == null ) {
				return;
			}

			State.keyboardRect = keyboardRect;

			if(!State.keyboardVisible)
			{
				State.priorInset = scrollView.ContentInset;
				State.priorScrollIndicatorInsets = scrollView.ScrollIndicatorInsets;
				State.priorPagingEnabled = scrollView.PagingEnabled;
			}

			State.keyboardVisible = true;
			scrollView.PagingEnabled = false;

			if(scrollView is TPKeyboardAvoidingScrollView)
			{
				State.priorContentSize = scrollView.ContentSize;

				if(CGSize.Equals (scrollView.ContentSize, new CGSize(0,0)))
				{
					// Set the content size, if it's not set. Do not set content size explicitly if auto-layout
					// is being used to manage subviews
					scrollView.ContentSize = scrollView.TPKeyboardAvoiding_calculatedContentSizeFromSubviewFrames ();
				}
			}

			// Shrink view's inset by the keyboard's height, and scroll to show the text field/view being edited
			//TODO : need fix
			UIView.BeginAnimations (null);
			NSNumber number = notification.UserInfo.ValueForKey (UIKeyboard.AnimationCurveUserInfoKey) as NSNumber;
			UIView.SetAnimationCurve ((UIViewAnimationCurve)number.Int32Value);
			number = notification.UserInfo.ValueForKey (UIKeyboard.AnimationDurationUserInfoKey) as NSNumber;
			UIView.SetAnimationDuration (number.FloatValue);
//			UIView.SetAnimationCurve (UIViewAnimationCurve.EaseIn);
//			UIView.SetAnimationDuration (1.0);

			scrollView.ContentInset = scrollView.TPKeyboardAvoiding_contentInsetForKeyboard ();

			nfloat viewableHeight = scrollView.Bounds.Size.Height - scrollView.ContentInset.Top - scrollView.ContentInset.Bottom;

			scrollView.SetContentOffset (new CGPoint (scrollView.ContentOffset.X, (nfloat)scrollView.TPKeyboardAvoiding_idealOffsetForViewwithViewingAreaHeight(firstResponder, viewableHeight)), false);

			scrollView.ScrollIndicatorInsets = scrollView.ContentInset;
			scrollView.LayoutIfNeeded ();

			UIView.CommitAnimations ();
		}

		public static void TPKeyboardAvoiding_keyboardWillHide(this UIScrollView scrollView, NSNotification notification)
		{
			CGRect keyboardRect = scrollView.ConvertRectFromView ((notification.UserInfo.ObjectForKey (UIKeyboard.FrameEndUserInfoKey) as NSValue).CGRectValue, null);

			if (keyboardRect == CGRect.Empty)
				return;

			//TPKeyboardAvoidingState state = KeyboardAvoidingState();
			if (State == null)
				State = new TPKeyboardAvoidingState ();

			if ( !State.keyboardVisible ) {
				return;
			}

			State.keyboardRect = new CGRect (0, 0, 0, 0);
			State.keyboardVisible = false;

			// Restore dimensions to prior size
			//TODO : add animations
			UIView.BeginAnimations (null);
			UIView.SetAnimationCurve (UIViewAnimationCurve.EaseIn);
			UIView.SetAnimationDuration (1.0);

			if(scrollView is TPKeyboardAvoidingScrollView)
			{
				scrollView.ContentSize = State.priorContentSize;
			}

			scrollView.ContentInset = State.priorInset;
			scrollView.ScrollIndicatorInsets = State.priorScrollIndicatorInsets;
			scrollView.PagingEnabled = State.priorPagingEnabled;
			scrollView.LayoutIfNeeded ();

			UIView.CommitAnimations ();
		}

		public static void TPKeyboardAvoiding_updateContentInset(this UIScrollView scrollView)
		{
			//TPKeyboardAvoidingState state = KeyboardAvoidingState ();
			if (State == null)
				State = new TPKeyboardAvoidingState ();

			if(State.keyboardVisible)
			{
				scrollView.ContentInset = scrollView.TPKeyboardAvoiding_contentInsetForKeyboard ();
			}
		}

		public static void TPKeyboardAvoiding_updateFromContentSizeChange(this UIScrollView scrollView)
		{
			//TPKeyboardAvoidingState state = KeyboardAvoidingState ();
			if (State == null)
				State = new TPKeyboardAvoidingState ();

			if(State.keyboardVisible)
			{
				State.priorContentSize = scrollView.ContentSize;
				scrollView.ContentInset = scrollView.TPKeyboardAvoiding_contentInsetForKeyboard ();
			}
		}

		public static bool TPKeyboardAvoiding_focusNextTextField(this UIScrollView scrollView)
		{
			UIView firstResponder = scrollView.TPKeyboardAvoiding_findFirstResponderBeneathView(scrollView);
			if ( firstResponder == null ) {
				return false;
			}

			float minY = float.MaxValue;
			UIView view = null;
			scrollView.TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView(firstResponder,scrollView,ref minY, ref view);

			if ( view != null ) {
				view.BecomeFirstResponder ();
				return true;
			}

			return false;
		}

		public static void TPKeyboardAvoiding_scrollToActiveTextField(this UIScrollView scrollView)
		{
			//TPKeyboardAvoidingState state = KeyboardAvoidingState ();
			if (State == null)
				State = new TPKeyboardAvoidingState ();

			if(!State.keyboardVisible)
			{
				return;
			}

			nfloat visibleSpace = scrollView.Bounds.Size.Height - scrollView.ContentInset.Top - scrollView.ContentInset.Bottom;

			CGPoint idealOffset = new CGPoint (0, scrollView.TPKeyboardAvoiding_idealOffsetForViewwithViewingAreaHeight (scrollView.TPKeyboardAvoiding_findFirstResponderBeneathView (scrollView), visibleSpace));

			DispatchQueue.CurrentQueue.DispatchAfter (DispatchTime.Now, () => {
				scrollView.SetContentOffset (idealOffset,true);
			});
		}

		public static UIView TPKeyboardAvoiding_findFirstResponderBeneathView(this UIScrollView scrollView, UIView view)
		{
			foreach(UIView childView in view.Subviews)
			{
				if (childView.IsFirstResponder)
					return childView;
                
				UIView result = scrollView.TPKeyboardAvoiding_findFirstResponderBeneathView (childView);
				if (result != null)
					return result;
			}
			return null;
		}

		public static CGSize TPKeyboardAvoiding_calculatedContentSizeFromSubviewFrames(this UIScrollView scrollView)
		{
			bool wasShowingVerticalScrollIndicator = scrollView.ShowsVerticalScrollIndicator;
			bool wasShowingHorizontalScrollIndicator = scrollView.ShowsHorizontalScrollIndicator;

			scrollView.ShowsVerticalScrollIndicator = false;
			scrollView.ShowsHorizontalScrollIndicator = false;

			CGRect rect = new CGRect (0, 0, 0, 0);
			foreach (var subview in scrollView.Subviews)
			{
				rect = CGRect.Union (rect, subview.Frame);
			}

			var size = rect.Size;
			size.Height += kCalculatedContentPadding;

			rect.Size = size;

			scrollView.ShowsVerticalScrollIndicator = wasShowingVerticalScrollIndicator;
			scrollView.ShowsHorizontalScrollIndicator = wasShowingHorizontalScrollIndicator;

			return rect.Size;
		}

		public static UIEdgeInsets TPKeyboardAvoiding_contentInsetForKeyboard(this UIScrollView scrollView)
		{
			//TPKeyboardAvoidingState state = KeyboardAvoidingState();
			if (State == null)
				State = new TPKeyboardAvoidingState ();
			
			UIEdgeInsets newInset = scrollView.ContentInset;
			CGRect keyboardRect = State.keyboardRect;
			newInset.Bottom = (nfloat) (keyboardRect.Size.Height - Math.Max((keyboardRect.GetMaxY () - scrollView.Bounds.GetMaxY()), 0));
			return newInset;
		}

		public static float TPKeyboardAvoiding_idealOffsetForViewwithViewingAreaHeight(this UIScrollView scrollView, UIView view, nfloat viewAreaHeight)
		{
			CGSize contentSize = scrollView.ContentSize;
			float offset = 0.0f;

            if (view == null)
                return offset;
			CGRect subviewRect = view.ConvertRectToView (view.Bounds, scrollView);


			// Attempt to center the subview in the visible space, but if that means there will be less than kMinimumScrollOffsetPadding
			// pixels above the view, then substitute kMinimumScrollOffsetPadding
			float padding = (float) (viewAreaHeight - subviewRect.Size.Height) / 2;
			if ( padding < kMinimumScrollOffsetPadding ) {
				padding = kMinimumScrollOffsetPadding;
			}

			// Ideal offset places the subview rectangle origin "padding" points from the top of the scrollview.
			// If there is a top contentInset, also compensate for this so that subviewRect will not be placed under
			// things like navigation bars.
			offset = (float) (subviewRect.Y - padding - scrollView.ContentInset.Top);

			// Constrain the new contentOffset so we can't scroll past the bottom. Note that we don't take the bottom
			// inset into account, as this is manipulated to make space for the keyboard.
			if ( offset > (contentSize.Height - viewAreaHeight) ) {
				offset = (float) (contentSize.Height - viewAreaHeight);
			}

			// Constrain the new contentOffset so we can't scroll past the top, taking contentInsets into account
			if ( offset < -scrollView.ContentInset.Top ) {
				offset =(float) -scrollView.ContentInset.Top;
			}

			return offset;
		}

		public static void TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView(this UIScrollView scrollView,UIView firstResponder, UIView beneathView, ref float minY, ref UIView foundView)
		{
			// Search recursively for text field or text view below firstResponder
			nfloat priorFieldOffset = scrollView.ConvertRectFromView (firstResponder.Frame, firstResponder.Superview).GetMinY ();

			foreach (UIView childView  in beneathView.Subviews)
			{
				if (childView.Hidden)
					continue;

				if((childView is UITextView || childView is UITextField) && childView.UserInteractionEnabled )
				{
					CGRect frame = scrollView.ConvertRectFromView (childView.Frame, beneathView);
					if(childView != firstResponder && frame.GetMinY () >= priorFieldOffset && frame.GetMinY () < (nfloat)minY && !(FEqual ((float)frame.Y, (float)firstResponder.Frame.Y)) && frame.X <= firstResponder.Frame.X)
					{
						minY = (float)frame.GetMinY ();
						foundView = childView;
					}
				}
				else
				{
					scrollView.TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView (firstResponder, childView, ref minY,  ref foundView);
				}
			}
		}

		public static void TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView(this UIScrollView scrollView,UIView firstResponder, UIView beneathView, float minY, UIView foundView)
		{
			// Search recursively for text field or text view below firstResponder
			nfloat priorFieldOffset = scrollView.ConvertRectFromView (firstResponder.Frame, firstResponder.Superview).GetMinY ();

			foreach (UIView childView  in beneathView.Subviews)
			{
				if (childView.Hidden)
					continue;

				if((childView is UITextView || childView is UITextField) && childView.UserInteractionEnabled )
				{
					CGRect frame = scrollView.ConvertRectFromView (childView.Frame, beneathView);
					if(childView != firstResponder && frame.GetMinY () >= priorFieldOffset && frame.GetMinY () < minY && !(FEqual ((float)frame.Y, (float)firstResponder.Frame.Y)) && frame.X < firstResponder.Frame.X)
					{
						minY = (float)frame.GetMinY ();
						foundView = childView;
					}
				}
				else
				{
					scrollView.TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView (firstResponder, childView, minY,  foundView);
				}
			}
		}

		public static void TPKeyboardAvoiding_assignTextDelegateForViewsBeneathView(this UIScrollView scrollView, UIView view)
		{
			foreach (var childView in view.Subviews)
			{
				if (childView is UITextView || childView is UITextField)
				{
					scrollView.TPKeyboardAvoiding_initializeView (childView);
				}
				else
					scrollView.TPKeyboardAvoiding_assignTextDelegateForViewsBeneathView (childView);
			}
		}

		public static void TPKeyboardAvoiding_initializeView(this UIScrollView scrollView, UIView view)
		{
			if ( view is UITextField && ((UITextField)view).ReturnKeyType == UIReturnKeyType.Default && (((UITextField)view).Delegate == null ||  ((UITextField)view).Delegate == (scrollView as TPKeyboardAvoidingScrollView).TextFieldDelegate))
			{
				((UITextField)view).Delegate = (scrollView as TPKeyboardAvoidingScrollView).TextFieldDelegate;
				UIView otherView = null;
				float minY = float.MaxValue;
				scrollView.TPKeyboardAvoiding_findTextFieldAfterTextFieldbeneathViewminYfoundView (view, scrollView,ref minY, ref otherView);

				if (otherView != null)
				{
					((UITextField)view).ReturnKeyType = UIReturnKeyType.Next;
				}
				else
					((UITextField)view).ReturnKeyType = UIReturnKeyType.Done;
			}
		}
	}

	public class TPKeyboardAvoidingState : NSObject
	{
		public UIEdgeInsets priorInset;
		public  UIEdgeInsets priorScrollIndicatorInsets;
		public bool        keyboardVisible;
		public  CGRect       keyboardRect;
		public  CGSize       priorContentSize;
		public bool priorPagingEnabled;
	}



}

