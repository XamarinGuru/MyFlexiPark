
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;
using FlexyPark.Core;
using WYPopoverControllerBinding;
using CoreGraphics;
using System.Linq;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AppSettingsView : BaseView, IAppSettingsView
    {
        public AppSettingsView()
            : base("AppSettingsView", null)
        {
        }

        public new AppSettingsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AppSettingsViewModel;
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

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.IsLoading = false;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel.SaveSettingsCommand.Execute();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.View = this;
			
            if (DeviceHelper.IsPad)
                HideBackButton();
            SetTitle("App Settings");
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AppSettingsView, AppSettingsViewModel>();

            set.Bind(lbAppVersion).To(vm => vm.AppVersion);

            set.Bind(swGoogle).To(vm => vm.IsGoogleMapsInstalled);
            set.Bind(swWaze).To(vm=>vm.IsWazeInstalled);
            set.Bind(swNavmii).To(vm => vm.IsNavmiiInstalled);

            set.Bind(tfLanguage).To(vm => vm.Language);
            set.Bind(btnPickLanguage).To(vm => vm.ShowLanguagePickerCommand);

            #region Language Binding

            set.Bind(lbAppVer).To(vm=>vm.TextSource).WithConversion("Language", "AppVersionText");
            set.Bind(lbAppLang).To(vm=>vm.TextSource).WithConversion("Language", "AppLanguageText");
            set.Bind(lbUseGG).To(vm=>vm.TextSource).WithConversion("Language", "UseGGText");
            set.Bind(lbUseNavmii).To(vm=>vm.TextSource).WithConversion("Language", "UseNavmiiText");
            set.Bind(lbUseWaze).To(vm=>vm.TextSource).WithConversion("Language", "UseWazeText");

            #endregion

            set.Apply();

        }

        #region LanguagePickerViewModel
        public class LanguagePickerViewModel : UIPickerViewModel
        {
            AppSettingsViewModel mViewModel;
            public LanguagePickerViewModel(AppSettingsViewModel viewModel)
            {
                mViewModel = viewModel;
            }
            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                if (component == 0)
                    return 3;
                else
                    return 0;
            }

            public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
            {
                UILabel label = view as UILabel;
                if(label == null)
                {
                    label = new UILabel(new CGRect(new CGPoint(0,0) , new CGSize(320, AppConstants.Languages[(int)row].GetHeightForMultilineLabelWithString( 320, UIFont.SystemFontOfSize(20f) ))));
                    label.Lines = 0;
                    label.TextAlignment = UITextAlignment.Center;
                }

                label.Text = GetTitle(pickerView, row, component);

                return label;
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                if(component == 0)
                {
                    return AppConstants.Languages[row];
                }

                return string.Empty;
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                if(component == 0)
                {
                    mViewModel.Language = AppConstants.Languages[(int)row];
                }
            }
        }
        #endregion

        #region Language Picker

        WYPopoverController popoverPicker;
        UIViewController pickerView;
        UIPickerView picker;

        void InitPicker ()
        {
            if (pickerView == null)
            {
                pickerView = new UIViewController();
                pickerView.View.Frame = new CGRect(0, 0, 320, 200);
            }

            if (picker == null)
            {
                picker = new UIPickerView(pickerView.View.Frame);
                picker.Model = new LanguagePickerViewModel(this.ViewModel);
                pickerView.View.AddSubview (picker);
            }

            if (popoverPicker == null)
            {
                popoverPicker = new WYPopoverController(pickerView);
                popoverPicker.PopoverContentSize = pickerView.View.Frame.Size;
            }
        }

        #endregion

        #region IAppSettingsView implementation

        public T GetPreference<T>(string key)
        {
            var value = NSUserDefaults.StandardUserDefaults.StringForKey(key);
            return value != null ? (T)Convert.ChangeType(value, typeof(T)) : default(T);
        }

        public void SetPreference<T>(string key, T value)
        {
            if (value != null)
            {
                NSUserDefaults.StandardUserDefaults.SetString(value.ToString(), key);
                NSUserDefaults.StandardUserDefaults.Synchronize();
            }
        }

        public bool CheckIfAppInstalled(string appName)
        {
            switch (appName)
            {
                case AppConstants.Waze:
                    return UIApplication.SharedApplication.CanOpenUrl(new NSUrl("waze://"));
                case AppConstants.GoogleMaps:
                    return UIApplication.SharedApplication.CanOpenUrl(new NSUrl("comgooglemaps://"));
                case AppConstants.Navmii:
                    return UIApplication.SharedApplication.CanOpenUrl(new NSUrl("navfree://"));
                default:
                    return false;
            }
        }

        public void GotoStoreToInstallApp(string appName)
        {
            switch(appName)
            {
                case AppConstants.Waze:
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id323229106"));
                    break;
                case AppConstants.GoogleMaps:
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id585027354"));
                    break;
                case AppConstants.Navmii:
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("http://itunes.apple.com/us/app/id434365587"));
                    break;
            }
        }

        public string GetAppVersion()
        {
            NSString version = (NSString)NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString");
            return version.ToString ();
        }

        public void ShowLanguagePicker()
        {
            InitPicker();

            var list = AppConstants.Languages.ToList();

            picker.Select(list.IndexOf(ViewModel.Language), 0, true);

            popoverPicker.PresentPopoverFromRect(btnPickLanguage.Frame, vContent, WYPopoverArrowDirection.Up, true);
        }

        #endregion
    }
}

