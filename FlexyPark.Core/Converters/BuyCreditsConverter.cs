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
    public class BuyCreditsConverter :MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0} {1} {2}", Mvx.Resolve<ICacheService>().TextSource.GetText("BuyText"), value, Mvx.Resolve<ICacheService>().TextSource.GetText("CreditsText"));
        }
    }
}
