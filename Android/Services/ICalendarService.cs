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
using Google.Apis.Calendar.v3;

namespace FlexyPark.UI.Droid.Services
{
    public interface ICalendarService
    {
        Google.Apis.Calendar.v3.CalendarService CachedCalendarService { get; set; }
    }

    public class CalendarService : ICalendarService
    {
        public Google.Apis.Calendar.v3.CalendarService CachedCalendarService { get; set; }
    }
}