using System;
namespace FlexyPark.Core.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTime GetFirstDayOfMonthAndYear (int year, int month)
        {
            return new DateTime (year, month, 1, 0, 0, 0);
        }
    }
}

