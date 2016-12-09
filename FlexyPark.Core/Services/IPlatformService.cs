using System;

namespace FlexyPark.Core.Services
{
    public enum OS
    {
        Droid,
        Touch,
        WinPhone,
        WinStore,
        Mac,
        Wpf
    }

    public interface IPlatformService
    {
        OS OS { get; }

        string GetTimeZone();

        T GetPreference<T>(string key);

        void SetPreference<T>(string key, T value);

        bool IsLogin();
    }

}

