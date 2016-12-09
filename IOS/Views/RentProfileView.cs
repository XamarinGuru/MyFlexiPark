
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using WYPopoverControllerBinding;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using System.Text;

namespace FlexyPark.UI.Touch.Views
{
	public partial class RentProfileView : BaseView, IRentProfileView, IUITextFieldDelegate
	{
		public RentProfileView()
			: base("RentProfileView", null)
		{
		}

		public new RentProfileViewModel ViewModel
		{
			get
			{
				return base.ViewModel as RentProfileViewModel;
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
			base.ViewWillDisappear(animated);
			//ViewModel.SaveCardInformations ();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.
			ViewModel.View = this;

			//tfNumber.Delegate = this;

			var set = this.CreateBindingSet<RentProfileView, RentProfileViewModel>();

			set.Bind(btnPick).To(vm => vm.PickValidityCommand);
			set.Bind(btnCreditPick).To(vm => vm.CreditPickCommand);
			set.Bind(btnAdd).For(v => v.Hidden).To(vm => vm.IsHaveCreditCard);
			set.Bind(btnRemove).For(v => v.Hidden).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");

			set.Bind(tfValidity).To(vm => vm.ValidityTime).WithConversion("ValidityTime");
			set.Bind(tfNumber).To(vm => vm.Number);
			set.Bind(tfHolderName).To(vm => vm.HolderName);
			set.Bind(tfCryptogram).To(vm => vm.Cryptogram);
			set.Bind(tfCredit).To(vm => vm.SelectedCreditStr);

			set.Bind(tfValidity).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");
			set.Bind(tfNumber).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");
			set.Bind(tfHolderName).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");
			set.Bind(tfCryptogram).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");
			set.Bind(tfCredit).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");
			set.Bind(btnPick).For(v => v.UserInteractionEnabled).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");

			set.Bind(btnBuyNow).To(vm => vm.BuyNowCommand);
			set.Bind(btnAdd).To(vm => vm.AddOrRemoveCardCommand);
			set.Bind(btnRemove).To(vm => vm.AddOrRemoveCardCommand);

			#region Language Binding

			set.Bind(btnPick).For("Title").To(vm => vm.TextSource).WithConversion("Language", "PickText");
			set.Bind(btnBuyNow).For("Title").To(vm => vm.TextSource).WithConversion("Language", "BuyText");

			set.Bind(lbCreditCardNumber).To(vm => vm.TextSource).WithConversion("Language", "CreditCardNumberText");
			set.Bind(lbCreditCardName).To(vm => vm.TextSource).WithConversion("Language", "CreditCardHolderNameText");
			set.Bind(lbCreditCardCrypto).To(vm => vm.TextSource).WithConversion("Language", "CreditCardCryptogramText");
			set.Bind(lbValidity).To(vm => vm.TextSource).WithConversion("Language", "ValidityText");
			set.Bind(lbCredits).To(vm => vm.TextSource).WithConversion("Language", "CreditsText");

			#endregion

			set.Apply();
		}

		#region PickerMonthYear

		WYPopoverController popoverPicker;
		UIViewController pickerView;
		UIPickerView picker;

		void InitPicker()
		{
			if (pickerView == null)
			{
				pickerView = new UIViewController();
				pickerView.View.Frame = new CGRect(0, 0, 320, 200);
			}

			if (picker == null)
			{
				picker = new UIPickerView(pickerView.View.Frame);
				picker.Model = new MyPickerViewModel(ViewModel);
				pickerView.View.AddSubview(picker);
			}

			if (popoverPicker == null)
			{
				popoverPicker = new WYPopoverController(pickerView);
				popoverPicker.PopoverContentSize = pickerView.View.Frame.Size;
			}
		}

		#endregion

		#region CreditPicker

		WYPopoverController popoverCreditPicker;
		UIViewController creditPickerView;
		UIPickerView creditPicker;

		void InitCreditPicker()
		{
			if (creditPickerView == null)
			{
				creditPickerView = new UIViewController();
				creditPickerView.View.Frame = new CGRect(0, 0, 320, 200);
			}

			if (creditPicker == null)
			{
				creditPicker = new UIPickerView(creditPickerView.View.Frame);
				creditPicker.Model = new MyCreditPickerViewModel(ViewModel);
				creditPickerView.View.AddSubview(creditPicker);
			}

			if (popoverCreditPicker == null)
			{
				popoverCreditPicker = new WYPopoverController(creditPickerView);
				popoverCreditPicker.PopoverContentSize = creditPickerView.View.Frame.Size;
			}
		}

		#endregion

		#region IRentProfileView implementation

		public void ShowPicker()
		{
			InitPicker();

			picker.Select((int)ViewModel.ValidityTime.Month - 1, 0, false);
			picker.Select((int)ViewModel.ValidityTime.Year - DateTime.Today.Year, 1, false);

			popoverPicker.PresentPopoverFromRect(btnPick.Frame, vContent, WYPopoverArrowDirection.Down, true);
		}

		public void ShowCreditPicker()
		{
			InitCreditPicker();
			creditPicker.Select(ViewModel.SelectedCredit, 0, false);
			popoverCreditPicker.PresentPopoverFromRect(btnCreditPick.Frame, vContent, WYPopoverArrowDirection.Down, true);
		}

		public void CloseKeyboard()
		{
			View.EndEditing(true);
		}

		#endregion


	}



	public class MyPickerViewModel : UIPickerViewModel
	{
		int minYear;
		RentProfileViewModel viewModel;

		public MyPickerViewModel(RentProfileViewModel viewModel)
		{
			this.viewModel = viewModel;
			minYear = DateTime.Today.Year;
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 2;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			if (component == 0)
				return 12;
			else
				return 20;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			string title = "";
			if (component == 0)
			{ // month
				title = string.Format("{0:00}", (int)row + 1);
			}
			else {
				title = (minYear + (int)row).ToString();
			}
			return title;
		}

		/*public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            UILabel label = view as UILabel;
            if(label == null)
            {
                label = new UILabel();
                label.Font = UIFont.SystemFontOfSize(17f);
                label.Lines = 0;
                label.TextAlignment = UITextAlignment.Center;
            }

            label.Text = GetTitle(pickerView, row, component);

            return label;
        }*/

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			int month = (int)pickerView.SelectedRowInComponent(0) + 1;
			int year = minYear + (int)pickerView.SelectedRowInComponent(1);

			viewModel.ValidityTime = new DateTime(year, month, DateTime.Today.Day);
		}
	}

	public class MyCreditPickerViewModel : UIPickerViewModel
	{
		RentProfileViewModel viewModel;

		public MyCreditPickerViewModel(RentProfileViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return viewModel.CreditsValues.Count;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			return string.Format("{0} €", viewModel.CreditsValues[(int)row]);
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			viewModel.SelectedCredit = viewModel.CreditsValues[(int)row];
		}
	}
}

