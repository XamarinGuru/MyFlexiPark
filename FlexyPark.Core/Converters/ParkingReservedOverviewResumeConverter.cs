using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
    public class ParkingReservedOverviewResumeConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            string mText = String.Empty;
            switch (value)
            {
                case "Resume":
                    mText = Mvx.Resolve<ICacheService>().TextSource.GetText("ResumeText");
                    break;
                default:
                    mText = Mvx.Resolve<ICacheService>().TextSource.GetText("OverviewText");
                    break;
            }

            return mText;
        }
    }
}
