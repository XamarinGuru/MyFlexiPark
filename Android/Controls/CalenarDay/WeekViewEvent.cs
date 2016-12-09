using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace FlexyPark.UI.Droid.Controls.CalenarDay
{
    public class WeekViewEvent
    {

        private long mId;
        private Calendar mStartTime;
        private Calendar mEndTime;
        private String mName;
        private Color mColor;

        public WeekViewEvent()
        {

        }
        public WeekViewEvent(long id, String name, int startYear, int startMonth, int startDay, int startHour, int startMinute, int endYear, int endMonth, int endDay, int endHour, int endMinute)
        {
            this.mId = id;

            this.mStartTime = Calendar.Instance;
            this.mStartTime.Set(Calendar.Year, startYear);
            this.mStartTime.Set(Calendar.Month, startMonth - 1);
            this.mStartTime.Set(Calendar.DayOfMonth, startDay);
            this.mStartTime.Set(Calendar.HourOfDay, startHour);
            this.mStartTime.Set(Calendar.Minute, startMinute);

            this.mEndTime = Calendar.Instance;
            this.mEndTime.Set(Calendar.Year, endYear);
            this.mEndTime.Set(Calendar.Month, endMonth - 1);
            this.mEndTime.Set(Calendar.DayOfMonth, endDay);
            this.mEndTime.Set(Calendar.HourOfDay, endHour);
            this.mEndTime.Set(Calendar.Minute, endMinute);

            this.mName = name;
        }
        public WeekViewEvent(long id, String name, Calendar startTime, Calendar endTime)
        {
            this.mId = id;
            this.mName = name;
            this.mStartTime = startTime;
            this.mEndTime = endTime;
        }
        public Calendar getStartTime()
        {
            return mStartTime;
        }

        public void setStartTime(Calendar startTime)
        {
            this.mStartTime = startTime;
        }

        public Calendar getEndTime()
        {
            return mEndTime;
        }

        public void setEndTime(Calendar endTime)
        {
            this.mEndTime = endTime;
        }

        public string getName()
        {
            return mName;
        }

        public void setName(String name)
        {
            this.mName = name;
        }

        public Color getColor()
        {
            return mColor;
        }

        public void setColor(Color color)
        {
            this.mColor = color;
        }

        public long getId()
        {
            return mId;
        }

        public void setId(long id)
        {
            this.mId = id;
        }
    }
}