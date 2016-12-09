using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
    public class AddEditButtonTitleConverter : MvxValueConverter<string,string>
    {
        protected override string Convert(string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string toReturn = string.Empty;
            switch(parameter.ToString())
            {
                case "Edit":
                    toReturn = Mvx.Resolve<ICacheService>().TextSource.GetText("EditText");
                    break;
                default:
                    toReturn = Mvx.Resolve<ICacheService>().TextSource.GetText("AddText");
                    break;
            }
            return toReturn;
        }
    }
}

