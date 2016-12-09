using FlexyPark.Core.Services;
using Foundation;
using System;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Services
{
    public class PlatformService : IPlatformService
    {
        #region IPlatformService implementation

        public OS OS
        {
            get
            {
                return OS.Touch;
            }
        }

        public string GetTimeZone()
        {
            return NSTimeZone.LocalTimeZone.Name;
        }

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

        public bool IsLogin()
        {
            return GetPreference<bool>(AppConstants.IsLogin);
        }

        #endregion


    }
}

