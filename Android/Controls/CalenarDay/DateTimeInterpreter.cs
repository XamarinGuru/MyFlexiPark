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
using Java.Util;

namespace FlexyPark.UI.Droid.Controls.CalenarDay
{
    public interface DateTimeInterpreter
    {
        String interpretDate(Calendar date, int Language);
        String interpretTime(int hour);
    }

    
}