using System;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Converters
{
    public class ReportModeToBooleanConverter : MvxValueConverter<ReportMode,bool>
    {
        protected override bool Convert(ReportMode value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = (string)parameter;
            switch (param)
            {
                case "CallOwner":
                    return value != ReportMode.CallOwner;
                case "PlateNumber":
                    return value != ReportMode.PlateNumber && value != ReportMode.Unreachable;
                case "PictureLeave":
                    return value != ReportMode.PictureLeave;
                case "PictureRefuse":
                    return value != ReportMode.PictureRefuse;
                case "Refund":
                    return value != ReportMode.Refund && value != ReportMode.NotFound;
                case "Full":
                    return value != ReportMode.Full;
            }

            return false;
        }
    }
}

