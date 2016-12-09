using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;
using Java.Util;
using Uri = Android.Net.Uri;

namespace FlexyPark.UI.Droid.Activity
{
    public class CommonFragment : BaseFragment, ICommonProfileView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public CommonFragment()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().MyProfileViewModel.CommonVM;
        }

        public new CommonProfileViewModel ViewModel
        {
            get { return base.ViewModel as CommonProfileViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.CommonFrament, container, false);
            ViewModel.View = this;
            SetButtonEffects(view, new List<int>()
            {
                Resource.Id.tvOk,
                Resource.Id.tvReadTermsOfUse,
                Resource.Id.tvLogout
            });
            return view;

        }

        #endregion

        #region Implements

        public string GetDeviceIdentifier()
        {
            return GetDeviceId();
        }

        public void OpenURL(string url)
        {
            try
            {
                Intent browserIntent = new Intent(Intent.ActionView, Uri.Parse(url));
                //var actionChoose = Intent.CreateChooser(browserIntent, "Please choose");
                StartActivity(browserIntent);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
            }
        }

		public void CloseKeyboard()
		{
			//throw new NotImplementedException();
		}

        #endregion

        #region Methods


        #region GetDeviceId

        public string GetDeviceId()
        {
            var telephonyDeviceID = string.Empty;
            var telephonySIMSerialNumber = string.Empty;
            TelephonyManager telephonyManager = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);
            if (telephonyManager != null)
            {
                if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
                    telephonyDeviceID = telephonyManager.DeviceId;
                if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
                    telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            }
            var androidID = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            var deviceID = deviceUuid.ToString();
            return deviceID;
        }

		#endregion

		#endregion


	}
}