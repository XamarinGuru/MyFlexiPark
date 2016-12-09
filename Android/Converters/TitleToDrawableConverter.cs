using System;
using System.Collections.Generic;
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

namespace FlexyPark.UI.Droid.Converters
{
    public class TitleToDrawableConverter : MvxValueConverter<string, int>
    {
        protected override int Convert(string value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case "Please go to your spot":
                    return Resource.Drawable.blue_icon_home;
                    break;
                case "Placez-vous sur votre parking":
                    return Resource.Drawable.blue_icon_home;
                    break;
                case "GPS":
                    return Resource.Drawable.blue_icon_navigate;
                    break;
                case "GPS fr":
                    return Resource.Drawable.blue_icon_navigate;
                    break;
                case "Accuracy":
                    return Resource.Drawable.blue_icon_chart;
                    break;
                case "Précision":
                    return Resource.Drawable.blue_icon_chart;
                    break;
                case "Please set the spot address":
                    return Resource.Drawable.blue_icon_location;
                    break;
                case "Introduisez l’adresse du parking":
                    return Resource.Drawable.blue_icon_location;
                    break;
                case "Please set the spot size":
                    return Resource.Drawable.blue_icon_car;
                    break;
                case "Précisez la taille du parking":
                    return Resource.Drawable.blue_icon_car;
                    break;
                case "Please set the spot cost":
                    return Resource.Drawable.blue_icon_euro;
                    break;
                case "Précisez le prix du parking":
                    return Resource.Drawable.blue_icon_euro;
                    break;
                case "Please set the spot calendar":
                    return Resource.Drawable.blue_icon_calendar;
                    break;
                case "Précisez l’indisponibilité du parking":
                    return Resource.Drawable.blue_icon_calendar;
                    break;


            }

            return Android.Resource.Color.Transparent;
        }
    }
}