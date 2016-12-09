using System;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch
{
    public class ChooseVehicleConverter: MvxValueConverter<ChooseVehicleMode,bool>
    {
        protected override bool Convert(ChooseVehicleMode value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == ChooseVehicleMode.NoAction)
            {
                return false;
            }
            return true;
        }
    }
}

