using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Converters
{
    public class ReportToBoolConverter : MvxValueConverter<ReportViewModel, bool>
    {
        protected override bool Convert(ReportViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (string)parameter;
            switch (param)
            {
                case "FlateNumber":
                    return value.Mode == ReportMode.PlateNumber || value.Mode == ReportMode.Unreachable;
                case "CallOwner":
                    return value.Mode == ReportMode.CallOwner;
                case "Full":
                    return value.Mode == ReportMode.Full;
                case "PictureLeave":
                    return value.Mode == ReportMode.PictureLeave;
                case "PictureRefuse":
                    return value.Mode == ReportMode.PictureRefuse;
                case "Refund":
                    return value.Mode == ReportMode.Refund || value.Mode == ReportMode.NotFound;
                default:
                    return false;
            }
        }
    }
}