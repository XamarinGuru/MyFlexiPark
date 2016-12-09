
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using WYPopoverControllerBinding;
using CoreGraphics;
using FlexyPark.UI.Touch.Helpers;
using Cirrious.CrossCore;
using FlexyPark.UI.Touch.Services;
using FlexyPark.Core.Messengers;
using FlexyPark.Core;
using System.Collections.Generic;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddEventView : BaseView, IAddEventView
    {
        public AddEventView()
            : base("AddEventView", null)
        {
        }

        public new AddEventViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddEventViewModel;
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
			
            //SetTitle(ViewModel.IsReadOnly ? );
            // Perform any additional setup after loading the view, typically from a nib.

            ViewModel.View = this;
            var set = this.CreateBindingSet<AddEventView, AddEventViewModel>();

            set.Bind(tfStartDate).To(vm => vm.StartDate).WithConversion("DateTimeToString", "Date");
            set.Bind(tfStartTime).To(vm => vm.StartDate).WithConversion("DateTimeToString", "Time");
            set.Bind(tfEndDate).To(vm => vm.EndDate).WithConversion("DateTimeToString", "Date");
            set.Bind(tfEndTime).To(vm => vm.EndDate).WithConversion("DateTimeToString", "Time");

            set.Bind(btnStartDate).To(vm => vm.SelectStartDateCommand);
            set.Bind(btnStartTime).To(vm => vm.SelectStartTimeCommand);
            set.Bind(btnEndDate).To(vm => vm.SelectEndDateCommand);
            set.Bind(btnEndTime).To(vm => vm.SelectEndTimeCommand);

            set.Bind(btnAdd).To(vm => vm.AddEventCommand);
            set.Bind(tfTitle).To(vm => vm.EventTitle);
            set.Bind (tfTitle).For (v=>v.UserInteractionEnabled).To (vm=>vm.IsReadOnly).WithConversion ("BooleanToHidden");

            set.Bind(btnRepeat).To(vm => vm.ShowRepeatPickerCommand);
            set.Bind(tfRepeat).To(vm => vm.SelectedRepeat).WithConversion("RepeatSpecialCase");
            set.Bind(tfTimes).To(vm => vm.Times);

            set.Bind(lbTimes).For(v => v.Hidden).To(vm => vm.IsShowTimes).WithConversion("BooleanToHidden");
            set.Bind(tfTimes).For(v => v.Hidden).To(vm => vm.IsShowTimes).WithConversion("BooleanToHidden");
            set.Bind(btnDelete).For(v => v.Hidden).To(vm => vm.IsEditMode).WithConversion("BooleanToHidden");
            set.Bind(btnDelete).To(vm => vm.DeleteCommand);

            set.Bind (btnAdd).For (v => v.Hidden).To (vm => vm.IsReadOnly);
            set.Bind (btnDelete).For (v => v.Hidden).To (vm => vm.IsReadOnly);


            #region Language Binding

            set.Bind(lbTitle).To(vm => vm.TextSource).WithConversion("Language", "TitleText");
            set.Bind(lbStartDate).To(vm => vm.TextSource).WithConversion("Language", "StartDateText");
            set.Bind(lbEndDate).To(vm => vm.TextSource).WithConversion("Language", "EndDateText");
            set.Bind(btnAdd).For("Title").To(vm => vm.ButtonTitle).WithConversion("AddEditButtonTitle", ViewModel.IsEditMode ? "Edit" : "Add");
            set.Bind(lbRepeat).To(vm => vm.TextSource).WithConversion("Language", "RepeatText");
            set.Bind(lbTimes).To(vm => vm.TextSource).WithConversion("Language", "TimesText");

            #endregion

            set.Apply();

            //mCalendarService = Mvx.Resolve<ICalendarService>().CachedCalendarService;
        }

        #region Repeat Picker

        WYPopoverController repeatPopoverPicker;
        UIViewController repeatPickerView;
        UIPickerView repeatPicker;

        void InitRepeatPicker()
        {
            if (repeatPickerView == null)
            {
                repeatPickerView = new UIViewController();
                repeatPickerView.View.Frame = new CGRect(0, 0, 320, 200);
            }

            if (repeatPicker == null)
            {
                repeatPicker = new UIPickerView(repeatPickerView.View.Frame);
                repeatPicker.Model = new RepeatPickerViewModel(this.ViewModel, this);
                repeatPickerView.View.AddSubview(repeatPicker);
            }

            if (repeatPopoverPicker == null)
            {
                repeatPopoverPicker = new WYPopoverController(repeatPickerView);
                repeatPopoverPicker.PopoverContentSize = repeatPickerView.View.Frame.Size;
            }
        }

        #endregion

        #region RepeatPickerViewModel

        public class RepeatPickerViewModel : UIPickerViewModel
        {
            AddEventViewModel mViewModel;
            AddEventView mView;

            public RepeatPickerViewModel(AddEventViewModel viewModel, AddEventView view)
            {
                mViewModel = viewModel;
                mView = view;
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                if (component == 0)
                    return AppConstants.Repeats.Length;
                else
                    return 0;
            }

            public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
            {
                UILabel label = view as UILabel;
                if (label == null)
                {
                    label = new UILabel(new CGRect(new CGPoint(0, 0), new CGSize(320, AppConstants.Repeats[(int)row].GetHeightForMultilineLabelWithString(320, UIFont.SystemFontOfSize(20f)))));
                    label.Lines = 0;
                    label.TextAlignment = UITextAlignment.Center;
                }

                label.Text = GetTitle(pickerView, row, component);

                return label;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                if (component == 0)
                {
                    switch (row)
                    {
                        case 0:
                            return "Never";
                        case 1:
                            return "Every Day";
                        case 2:
                            return "Every Week";
                        case 3:
                            return "Every 2 Week";
                        case 4:
                            return "Every Month";
                        case 5:
                            return "Every Year";
                        default:
                            return string.Empty;
                    }
                }

                return string.Empty;
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                if (component == 0)
                {
                    mViewModel.SelectedRepeat = AppConstants.Repeats[(int)row];

                    mViewModel.IsShowTimes = (int)row != 0;

                    if (mView.btnAddTopSpace != null)
                        mView.btnAddTopSpace.Constant = (int)row != 0 ? 100f : 20f;
                } 
            }
        }

        #endregion

        #region DateTime Picker

        WYPopoverController popoverPicker;
        UIViewController pickerView;
        UIDatePicker picker;

        void InitPicker(bool isDate, bool isStart)
        {
            if (pickerView == null)
            {
                pickerView = new UIViewController();
                pickerView.View.Frame = new CGRect(0, 0, 320, 200);
            }

            if (picker == null)
            {
                picker = new UIDatePicker();
                pickerView.View.AddSubview(picker);

                picker.ValueChanged += (sender, e) =>
                {		
                    if (picker.Tag == 0)
                    {
                        int compare = picker.Date.NSDateToDateTime().CompareTo(ViewModel.EndDate);
                        if (compare == 1)
                            ViewModel.EndDate = new DateTime(picker.Date.NSDateToDateTime().Year, picker.Date.NSDateToDateTime().Month, picker.Date.NSDateToDateTime().Day, 13, 0, 0);
                        
                        ViewModel.StartDate = picker.Date.NSDateToDateTime();
                    }
                    else
                        ViewModel.EndDate = picker.Date.NSDateToDateTime();
                    ;
                };
            }

            //set time format to 24h format
            picker.Locale = isDate ? new NSLocale("US") : new NSLocale("UK");

            picker.Mode = isDate ? UIDatePickerMode.Date : UIDatePickerMode.Time;

            picker.Tag = isStart ? 0 : 1;

            if (popoverPicker == null)
            {
                popoverPicker = new WYPopoverController(pickerView);
                popoverPicker.PopoverContentSize = pickerView.View.Frame.Size;
            }
        }

        #endregion

        #region IAddEventView implementation

        public void GetEvent()
        {
            //mCalendarService.Events.Get(ViewModel.CalendarId, ViewModel.EventId).FetchAsync(OnEventResponse);
        }

        public void ShowStartDatePicker()
        {
            View.EndEditing(true);

            InitPicker(true, true);

            picker.Date = ViewModel.StartDate.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnStartDate.Frame, vContent, WYPopoverArrowDirection.Up, true);
        }

        public void ShowStartTimePicker()
        {
            View.EndEditing(true);
            InitPicker(false, true);

            picker.Date = ViewModel.StartDate.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnStartTime.Frame, vContent, WYPopoverArrowDirection.Up, true);
        }

        public void ShowEndDatePicker()
        {
            View.EndEditing(true);
            InitPicker(true, false);

            picker.Date = ViewModel.EndDate.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnEndDate.Frame, vContent, WYPopoverArrowDirection.Up, true);
        }

        public void ShowEndTimePicker()
        {
            View.EndEditing(true);
            InitPicker(false, false);

            picker.Date = ViewModel.EndDate.DateTimeToNSDate();
            popoverPicker.PresentPopoverFromRect(btnEndTime.Frame, vContent, WYPopoverArrowDirection.Up, true);
        }

        public void ShowRepeatPicker()
        {
            View.EndEditing(true);
            InitRepeatPicker();

            var list = new List<string>(AppConstants.Repeats);

            repeatPicker.Select(list.IndexOf(ViewModel.SelectedRepeat), 0, true);
            repeatPopoverPicker.PresentPopoverFromRect(btnRepeat.Frame, vContent, WYPopoverArrowDirection.Down, true);
        }

        #endregion

    }
}

