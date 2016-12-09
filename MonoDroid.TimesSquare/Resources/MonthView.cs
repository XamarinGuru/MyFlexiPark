using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MonoDroid.TimesSquare
{
    public class MonthView : LinearLayout
    {
        private TextView _title;
        private CalendarGridView _grid;
        private ClickHandler _clickHandler;

        public MonthView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public static MonthView Create(ViewGroup parent, LayoutInflater inflater, string weekdayNameFormat,
            DateTime today, ClickHandler handler, int dividerColor, int dayBackgroundResId,
            int dayTextColorResId, int titleTextColor, int headerTextColor, int Language)
        {
            var view = (MonthView) inflater.Inflate(Resource.Layout.month, parent, false);
            view.setDividerColor(dividerColor);
            view.setDayTextColor(dayTextColorResId);
            view.setTitleTextColor(titleTextColor);
            view.setHeaderTextColor(headerTextColor);

            if (dayBackgroundResId != 0) {
                view.setDayBackground(dayBackgroundResId);
            }

            var originalDay = today;

            var firstDayOfWeek = (int) CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            var headerRow = (CalendarRowView) view._grid.GetChildAt(0);

            for (int i = 0; i < 7; i++) {
                var offset = firstDayOfWeek - (int) today.DayOfWeek + i;
                today = today.AddDays(offset);
                var textView = (TextView) headerRow.GetChildAt(i);
                // Sun Mon
               // textView.Text = today.ToString(weekdayNameFormat);
                string tmp = today.ToString(weekdayNameFormat);
                string SunMon = string.Empty;

                switch (tmp)
                {
                    case "Sun":
                        SunMon = (Language == 1) ? "Dim" : "Sun"; break;
                    case "Mon":
                        SunMon = (Language == 1) ? "Lun" : "Mon"; break;
                    case "Tue":
                        SunMon = (Language == 1) ? "Mar" : "Tue"; break;
                    case "Wed":
                        SunMon = (Language == 1) ? "Mer" : "Wed"; break;
                    case "Thu":
                        SunMon = (Language == 1) ? "Jeu" : "Thu"; break;
                    case "Fri":
                        SunMon = (Language == 1) ? "Ven" : "Fri"; break;
                    case "Sat":
                        SunMon = (Language == 1) ? "Sam" : "Sat"; break;
                    
                }

                textView.Text = SunMon;

                today = originalDay;
            }
            view._clickHandler = handler;
            return view;
        }

        public void Init(MonthDescriptor month, List<List<MonthCellDescriptor>> cells, int Language)
        {
            Logr.D("Initializing MonthView ({0:d}) for {1}", GetHashCode(), month);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            

         //  _title.Text = month.Label;

            _title.Text = GetMonthTitle(month, Language);

            int numOfRows = cells.Count;
            _grid.NumRows = numOfRows;
            for (int i = 0; i < 6; i++) {
                var weekRow = (CalendarRowView)_grid.GetChildAt(i + 1);
                weekRow.ClickHandler = _clickHandler;
                if (i < numOfRows) {
                    weekRow.Visibility = ViewStates.Visible;
                    var week = cells[i];
                    for (int c = 0; c < week.Count; c++) {
                        var cell = week[c];
                        var cellView = (CalendarCellView)weekRow.GetChildAt(c);
                        cellView.Text = cell.Value.ToString();
                        cellView.Enabled = cell.IsCurrentMonth;

                        cellView.Selectable = cell.IsSelectable;
                        cellView.Selected = cell.IsSelected;
                        cellView.IsCurrentMonth = cell.IsCurrentMonth;
                        cellView.IsToday = cell.IsToday;
                        cellView.IsHighlighted = cell.IsHighlighted;
                        cellView.RangeState = cell.RangeState;
                        cellView.Tag = cell;
                    }
                }
                else {
                    weekRow.Visibility = ViewStates.Gone;
                }
            }
            stopWatch.Stop();
            Logr.D("MonthView.Init took {0} ms", stopWatch.ElapsedMilliseconds);
        }

        public void setDividerColor(int color)
        {
            _grid.SetDividerColor(color);
        }

        public void setDayBackground(int resId)
        {
            _grid.SetDayBackground(resId);
        }

        public void setDayTextColor(int resId)
        {
            _grid.SetDayTextColor(resId);
        }

        public void setTitleTextColor(int color)
        {
            _title.SetTextColor(base.Resources.GetColor(color));
        }

        public void setHeaderTextColor(int color)
        {
            _grid.SetHeaderTextColor(color);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            _title = FindViewById<TextView>(Resource.Id.title);
            _grid = FindViewById<CalendarGridView>(Resource.Id.calendar_grid);
        }

        public string GetMonthTitle(MonthDescriptor monthDescriptor, int Language)
        {
            if (Language == 0)
            {
                //English
                switch (monthDescriptor.Month)
                {
                    case 1:
                        return "January" + " " + monthDescriptor.Year; 
                    case 2:
                        return "February" + " " + monthDescriptor.Year; 
                    case 3:
                        return "March" + " " + monthDescriptor.Year; 
                    case 4:
                        return "April" + " " + monthDescriptor.Year; 
                    case 5:
                        return "May" + " " + monthDescriptor.Year; 
                    case 6:
                        return "June" + " " + monthDescriptor.Year; 
                    case 7:
                        return "July" + " " + monthDescriptor.Year; 
                    case 8:
                        return "August" + " " + monthDescriptor.Year; 
                    case 9:
                        return "September" + " " + monthDescriptor.Year; 
                    case 10:
                        return "October" + " " + monthDescriptor.Year; 
                    case 11:
                        return "November" + " " + monthDescriptor.Year; 
                    case 12:
                        return "December" + " " + monthDescriptor.Year; 
                    default:
                        return monthDescriptor.Year.ToString();
                }
                
            }
            else
            {
                // French
                switch (monthDescriptor.Month)
                {
                    case 1:
                        return "Janvier" + " " + monthDescriptor.Year; 
                    case 2:
                        return "Février" + " " + monthDescriptor.Year; 
                    case 3:
                        return "Mars" + " " + monthDescriptor.Year; 
                    case 4:
                        return "Avril" + " " + monthDescriptor.Year; 
                    case 5:
                        return "Mai" + " " + monthDescriptor.Year; 
                    case 6:
                        return "Juin" + " " + monthDescriptor.Year; 
                    case 7:
                        return "Juillet" + " " + monthDescriptor.Year; 
                    case 8:
                        return "Août" + " " + monthDescriptor.Year; 
                    case 9:
                        return "Septembre" + " " + monthDescriptor.Year; 
                    case 10:
                        return "Octobre" + " " + monthDescriptor.Year; 
                    case 11:
                        return "Novembre" + " " + monthDescriptor.Year; 
                    case 12:
                        return "Décembre" + " " + monthDescriptor.Year; 
                    default:
                        return monthDescriptor.Year.ToString();
                }
            }
        }
    }
}