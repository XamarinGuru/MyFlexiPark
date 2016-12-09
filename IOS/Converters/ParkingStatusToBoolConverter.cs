using System;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Converters
{
    public class ParkingStatusToBoolConverter : MvxValueConverter<ParkingStatus,bool>
    {
        protected override bool Convert(ParkingStatus value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = parameter as string;

            if (param.Equals("Not"))
                return !(value == ParkingStatus.Rented);

            if (param.Equals("Reserved"))
            {
                return !(value == ParkingStatus.Reserved);
            }
            else
            {
                return !(value == ParkingStatus.Rented);
            }
        }
    }
}

