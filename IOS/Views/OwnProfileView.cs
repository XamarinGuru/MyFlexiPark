
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using WYPopoverControllerBinding;
using CoreGraphics;

namespace FlexyPark.UI.Touch.Views
{
	public partial class OwnProfileView : BaseView, IOwnProfileView
    {
        public OwnProfileView()
            : base("OwnProfileView", null)
        {
        }

        public new OwnProfileViewModel ViewModel
        {
            get
            {
                return base.ViewModel as OwnProfileViewModel;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.
			ViewModel.View = this;

            var set = this.CreateBindingSet<OwnProfileView, OwnProfileViewModel>();

			#region bank account
			set.Bind(btnAdd).To(vm => vm.AddOrReplaceBankCommand);
			set.Bind(btnReplace).To(vm => vm.AddOrReplaceBankCommand);

			set.Bind(btnAdd).For(v => v.Hidden).To(vm => vm.IsHaveCreditCard);
			set.Bind(btnReplace).For(v => v.Hidden).To(vm => vm.IsHaveCreditCard).WithConversion("BooleanToHidden");

            set.Bind(tfBankAccount).To(vm => vm.BankAccount);
            set.Bind(tfHolderName).To(vm => vm.AccountName);
			#endregion

			#region user account

			set.Bind(btnPick).To(vm => vm.PickBirthdayCommand);
			set.Bind(btnOK).To(vm => vm.UpdateCommand);

			set.Bind(tfBirthday).To(vm => vm.User.DateOfBirth).WithConversion("DateOfBirth");
			set.Bind(tfStreet).To(vm => vm.User.Street);
			set.Bind(tfCity).To(vm => vm.User.City);
			set.Bind(tfPostalCode).To(vm => vm.User.PostalCode);

			#endregion


			#region Language Binding

			set.Bind(btnAdd).For("Title").To(vm => vm.TextSource).WithConversion("Language", "AddText");
			set.Bind(btnReplace).For("Title").To(vm => vm.TextSource).WithConversion("Language", "ReplaceText");
			
            set.Bind(lbBankAccount).To(vm=>vm.TextSource).WithConversion("Language", "BankAccountToBeCreditedText");
            set.Bind(lbAccountName).To(vm=>vm.TextSource).WithConversion("Language", "AccountHolderNameText");

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
				picker.Model = new MyBirthdayPickerViewModel(ViewModel, tfBirthday);
				pickerView.View.AddSubview(picker);
			}

			if (popoverPicker == null)
			{
				popoverPicker = new WYPopoverController(pickerView);
				popoverPicker.PopoverContentSize = pickerView.View.Frame.Size;
			}
		}

		#endregion

		#region IOwnProfileView implementation

		public void ShowPicker()
		{
			InitPicker();

			DateTime now = DateTime.UtcNow.ToLocalTime();
			DateTime birthday = DateTime.UtcNow.ToLocalTime();

			if (ViewModel.User.DateOfBirth != null && !ViewModel.User.DateOfBirth.Equals("na"))
				birthday = DateTime.Parse(ViewModel.User.DateOfBirth);

			picker.Select(99 - (now.Year - birthday.Year), 0, false);
			picker.Select((int)birthday.Month - 1, 1, false);
			picker.Select((int)birthday.Day - 1, 2, false);

			popoverPicker.PresentPopoverFromRect(btnPick.Frame, this.View, WYPopoverArrowDirection.Down, true);
		}

		public void CloseKeyboard()
		{
			View.EndEditing(true);
		}

		#endregion


	}



	public class MyBirthdayPickerViewModel : UIPickerViewModel
	{
		int maxYear;
		const int deltaYear = 100;
		OwnProfileViewModel viewModel;
		UITextField mTxtBirthday;

		public MyBirthdayPickerViewModel(OwnProfileViewModel viewModel, UITextField txtBirthday)
		{
			this.viewModel = viewModel;
			mTxtBirthday = txtBirthday;
			maxYear = DateTime.Today.Year;
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 3;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			if (component == 0)
				return deltaYear;
			else if (component == 1)
				return 12;
			else
				return 31;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			string title = "";
			if (component == 0)
			{ // year
				title = (maxYear - deltaYear + (int)row + 1).ToString();
			}
			else if (component == 1)
			{ // month
				title = string.Format("{0:00}", (int)row + 1);
			}
			else { // day
				title = string.Format("{0:00}", (int)row + 1);
			}
			return title;
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			int year = maxYear - (deltaYear - (int)pickerView.SelectedRowInComponent(0)) + 1;
			int month = (int)pickerView.SelectedRowInComponent(1) + 1;
			int day = (int)pickerView.SelectedRowInComponent(2) + 1;

			int daysInMonth = DateTime.DaysInMonth(year, month);

			if (day > daysInMonth)
			{
				pickerView.Select(daysInMonth - 1, 2, false);
				return;
			}
			var arrDates = new DateTime(year, month, day).GetDateTimeFormats();
			viewModel.User.DateOfBirth = arrDates[5];
			mTxtBirthday.Text = viewModel.User.DateOfBirth;
		}
	}

}

