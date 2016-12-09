using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
	public class ExpectedTimeConverter : MvxValueConverter<TimeSpan,string>
    {
		protected override string Convert(TimeSpan value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {	
			string info = null;
			if (value.TotalHours > 1) {
                info = string.Format ("{0:0.#} {1}", value.TotalHours, (int)value.TotalHours == 1 ? Mvx.Resolve<ICacheService>().SharedTextSource.GetText("HourText") : Mvx.Resolve<ICacheService>().SharedTextSource.GetText("HoursText"));
			} else if (value.TotalMinutes > 1) {
                info = string.Format ("{0:0} {1}", value.TotalMinutes, (int)value.TotalMinutes == 1 ? Mvx.Resolve<ICacheService>().SharedTextSource.GetText("MinuteText") : Mvx.Resolve<ICacheService>().SharedTextSource.GetText("MinutesText"));
			} else if (value.TotalSeconds > 0) {
                info = string.Format ("{0:0} {1}", value.TotalSeconds, (int)value.TotalSeconds == 1 ? Mvx.Resolve<ICacheService>().SharedTextSource.GetText("SecondText") : Mvx.Resolve<ICacheService>().SharedTextSource.GetText("SecondsText"));
			} else {
                info = Mvx.Resolve<ICacheService>().SharedTextSource.GetText("CalculatingText");
			}
			return info;
        }
    }
}

