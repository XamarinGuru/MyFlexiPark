using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlexyPark.Core;
using FlexyPark.Core.Services;

namespace FlexyPark.UI.Droid.Services
{
    public class PlatformService : IPlatformService
    {
        private string mSharedPreferences = "MyPrefs";
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor SharedPreferencesEditor;

        public PlatformService()
        {
            sharedPreferences = Application.Context.GetSharedPreferences(mSharedPreferences, FileCreationMode.Private);
        }


        public OS OS
        {
            get { return OS.Droid; }

        }

        public string GetTimeZone()
        {
            return TimeZone.CurrentTimeZone.ToString();
        }

        public T GetPreference<T>(string key)
        {
            string value = sharedPreferences.GetString(key, String.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            var t = default(T);
            return t;
        }

        public void SetPreference<T>(string key, T value)
        {
            SharedPreferencesEditor = sharedPreferences.Edit();
            SharedPreferencesEditor.PutString(key, value.ToString());
            SharedPreferencesEditor.Commit();
        }

        public bool IsLogin()
        {
            return GetPreference<bool>(AppConstants.IsLogin);
            ;
        }
    }
}