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
    public class ParkingSearchTitleConverter: MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Book for :":
                    return Mvx.Resolve<ICacheService>().TextSource.GetText("BookForText");
                default:
                    return Mvx.Resolve<ICacheService>().TextSource.GetText("SearchingForText");
            }
        }
    }
}
