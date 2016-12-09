//	New implementations, refactoring and restyling - FactoryMind || http://factorymind.com 
//  Converted to MonoTouch on 1/22/09 - Eduardo Scoz || http://escoz.com
//  Originally reated by Devin Ross on 7/28/09  - tapku.com || http://github.com/devinross/tapkulibrary
//
/*
 
 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:
 
 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

#if __UNIFIED__
	using Foundation;
	using UIKit;

	// Mappings Unified CoreGraphic classes to MonoTouch classes
	using CGRect = global::System.Drawing.RectangleF;
	using CGSize = global::System.Drawing.SizeF;
	using CGPoint = global::System.Drawing.PointF;

	// Mappings Unified types to MonoTouch types
	using nfloat = global::System.Single;
	using nint = global::System.Int32;
	using nuint = global::System.UInt32;
#else
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
#endif

namespace Factorymind.Components
{

	public delegate void DateSelected(DateTime date);
	public delegate void MonthChanged(DateTime monthSelected);

	public class FMCalendar : UIView
	{
		/// <summary>
		/// Fired when new date selected.
		/// </summary>
		public Action<DateTime> DateSelected;

		/// <summary>
		/// Fired when date selection finished
		/// </summary>
		public Action<DateTime> DateSelectionFinished;

		/// <summary>
		/// Fired when Selected month changed
		/// </summary>
		public Action<DateTime> MonthChanged;

		/// <summary>
		/// Mark with a dot dates that fulfill the predicate
		/// </summary>
		public Func<DateTime, bool> IsDayMarkedDelegate;

		/// <summary>
		/// Turn gray dates that fulfill the predicate
		/// </summary>
		public Func<DateTime, bool> IsDateAvailable;

		/// <summary>
		/// Gets the current selected Date.
		/// </summary>
		/// <value>The current selected date.</value>
		public DateTime CurrentSelectedDate { get { return SelectedDate; } }

		public DateTime CurrentMonthYear;
		protected DateTime CurrentDate { get; set; }
		internal DateTime SelectedDate { get; set; }

		private UIScrollView _scrollView;
		private bool calendarIsLoaded;

		private MonthGridView _monthGridView;
		private UIButton _leftButton, _rightButton;

		// User Customizations

		/// <summary>
		/// If true, Sunday will be showed as the first day of the week, otherwise the first one will be Monday
		/// </summary>
		/// <value><c>true</c> if sunday first; otherwise, <c>false</c>.</value>
		public Boolean SundayFirst { get; set; }

		/// <summary>
		/// Format string used to display the month's name
		/// </summary>
		/// <value>The month format string.</value>
		public String MonthFormatString { get; set; }

		/// <summary>
		/// Specify the color for the selected date
		/// </summary>
		/// <value>The color of the selection.</value>
		public UIColor SelectionColor { get; set; }

		/// <summary>
		/// Specify the color for the today circle
		/// </summary>
		/// <value>The color of the selection.</value>
		public UIColor TodayCircleColor { get; set; }

		/// <summary>
		/// Specify the background color for the calendar
		/// </summary>
		/// <value>The color of the selection.</value>
		public UIColor MonthBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the left arrow image.
		/// </summary>
		/// <value>The left arrow.</value>
		public UIImage LeftArrow { get; set; }

		/// <summary>
		/// Gets or sets the right arrow image.
		/// </summary>
		/// <value>The right arrow.</value>
		public UIImage RightArrow { get; set; }

		/// <summary>
		/// Gets or sets the top bar View.
		/// </summary>
		/// <value>The top bar.</value>
		public UIImage TopBar { get; set; }

		#if __UNIFIED__

		private CoreGraphics.CGRect HeaderViewSize { get; set; }
		private CoreGraphics.CGRect MainViewSize { get; set; }

		#else

		private RectangleF HeaderViewSize { get; set; }
		private RectangleF MainViewSize { get; set; }

		#endif

		private float HeaderMiddleY { get { return (float) (HeaderViewSize.Height / 2) - (HeaderElementSize / 2); } }
		private float HeaderElementSize { get { return (float) (HeaderViewSize.Height - (HeaderViewSize.Height * 0.10f)); } }

		private float HeaderBorderOffset { get { return 10f; } }
		private float DayNameHeight { get { return 10f; } }
		internal int DayCellWidth { get { return (int) MainViewSize.Width / 7; } }
		internal int DayCellHeight { get { return (int) MainViewSize.Height / 7; } }

		#if __UNIFIED__

		public FMCalendar() : this(new CoreGraphics.CGRect(0, 0, 320, 400))
		{
		}

		public FMCalendar(CoreGraphics.CGRect mainViewSize) : this(mainViewSize, new CoreGraphics.CGRect(0, 0, mainViewSize.Width, 60))
		{
		}

		public FMCalendar(CoreGraphics.CGRect mainViewSize, CoreGraphics.CGRect headerViewSize) : base (mainViewSize)
		{
			this.MainViewSize = mainViewSize;
			this.HeaderViewSize = headerViewSize;

			Initialize ();
		}

		#else

		public FMCalendar() : this(new RectangleF(0, 0, 320, 400))
		{
		}

		public FMCalendar(RectangleF mainViewSize) : this(mainViewSize, new RectangleF(0, 0, mainViewSize.Width, 60))
		{
		}

		public FMCalendar(RectangleF mainViewSize, RectangleF headerViewSize) : base (mainViewSize)
		{
			this.MainViewSize = mainViewSize;
			this.HeaderViewSize = headerViewSize;

			Initialize ();
		}

		#endif

		private void Initialize()
		{
			CurrentDate = DateTime.Now.Date;
			SelectedDate = CurrentDate;
			CurrentMonthYear = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);

			// Defaults

			SundayFirst = false;

			MonthFormatString = "MMMM yyyy";

			MonthBackgroundColor = UIColor.White;
			SelectionColor = UIColor.Red;
			TodayCircleColor = UIColor.Red;

			LeftArrow = UIImage.FromFile ("leftArrow.png");
			RightArrow = UIImage.FromFile ("rightArrow.png");
			BackgroundColor = MonthBackgroundColor;
		}

		public override void SetNeedsDisplay ()
		{
			base.SetNeedsDisplay();

			AdjustBackgroundColor ();

			if (_monthGridView!=null)
				_monthGridView.Update();
		}

		public override void LayoutSubviews ()
		{
			AdjustBackgroundColor ();

			if (calendarIsLoaded) return;

			#if __UNIFIED__

			_scrollView = new UIScrollView(new CoreGraphics.CGRect(0, HeaderViewSize.Height, MainViewSize.Width, MainViewSize.Height - HeaderViewSize.Height))
			{
				ContentSize = new CoreGraphics.CGSize(MainViewSize.Width, MainViewSize.Height),
				ScrollEnabled = false,
				Frame = new CoreGraphics.CGRect(0, HeaderViewSize.Height, MainViewSize.Width, MainViewSize.Height),
				BackgroundColor = MonthBackgroundColor,
			};

			#else

			_scrollView = new UIScrollView(new RectangleF(0, HeaderViewSize.Height, MainViewSize.Width, MainViewSize.Height - HeaderViewSize.Height))
			{
				ContentSize = new SizeF(MainViewSize.Width, MainViewSize.Height),
				ScrollEnabled = false,
				Frame = new RectangleF(0, HeaderViewSize.Height, MainViewSize.Width, MainViewSize.Height),
				BackgroundColor = MonthBackgroundColor,
			};

			#endif

			LoadButtons();

			LoadInitialGrids();

			AddSubview(_scrollView);
			_scrollView.AddSubview(_monthGridView);

			calendarIsLoaded = true;
		}

		public void DeselectDate(){
			if (_monthGridView!=null)
				_monthGridView.DeselectDayView();
		}

		private void LoadButtons()
		{
			_leftButton = UIButton.FromType(UIButtonType.Custom);
			_leftButton.TouchUpInside += HandlePreviousMonthTouch;
			_leftButton.ContentMode = UIViewContentMode.ScaleAspectFit;
			_leftButton.ContentEdgeInsets = new UIEdgeInsets (10, 10, 10, 10);
			_leftButton.SetImage(LeftArrow, UIControlState.Normal);
			AddSubview(_leftButton);

			#if __UNIFIED__

			_leftButton.Frame = new CoreGraphics.CGRect(HeaderBorderOffset, HeaderMiddleY, HeaderElementSize, HeaderElementSize);

			#else

			_leftButton.Frame = new RectangleF(HeaderBorderOffset, HeaderMiddleY, HeaderElementSize, HeaderElementSize);

			#endif

			_rightButton = UIButton.FromType(UIButtonType.Custom);
			_rightButton.TouchUpInside += HandleNextMonthTouch;
			_rightButton.ContentMode = UIViewContentMode.ScaleAspectFit;
			_rightButton.ContentEdgeInsets = new UIEdgeInsets (10, 10, 10, 10);
			_rightButton.SetImage(RightArrow, UIControlState.Normal);
			AddSubview(_rightButton);

			#if __UNIFIED__

			_rightButton.Frame = new CoreGraphics.CGRect(HeaderViewSize.Width - HeaderElementSize - HeaderBorderOffset, HeaderMiddleY, HeaderElementSize, HeaderElementSize);

			#else

			_rightButton.Frame = new RectangleF(HeaderViewSize.Width - HeaderElementSize - HeaderBorderOffset, HeaderMiddleY, HeaderElementSize, HeaderElementSize);

			#endif
		}

		private void HandlePreviousMonthTouch(object sender, EventArgs e)
		{
			MoveCalendarMonths(false, true);
		}
		private void HandleNextMonthTouch(object sender, EventArgs e)
		{
			MoveCalendarMonths(true, true);
		}

		/// <summary>
		/// Moves the calendar months.
		/// </summary>
		/// <param name="upwards">If set to <c>true</c> moves the month upwards.</param>
		/// <param name="animated">If set to <c>true</c> the transition will be animated.</param>
		public void MoveCalendarMonths(bool upwards, bool animated)
		{
			if (_monthGridView == null)
				return;

			CurrentMonthYear = CurrentMonthYear.AddMonths(upwards? 1 : -1);
			UserInteractionEnabled = false;

			// Dispatch event
			if (MonthChanged != null)
				MonthChanged(CurrentMonthYear);

			var gridToMove = CreateNewGrid(CurrentMonthYear);
			var pointsToMove = (upwards? 0 + _monthGridView.Lines : 0 - _monthGridView.Lines) * DayCellHeight;

			if (upwards && gridToMove.weekdayOfFirst==0)
				pointsToMove += DayCellHeight;
			if (!upwards && _monthGridView.weekdayOfFirst==0)
				pointsToMove -= DayCellHeight;

			#if __UNIFIED__

			gridToMove.Frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, pointsToMove), gridToMove.Frame.Size);

			#else

			gridToMove.Frame = new RectangleF(new PointF(0, pointsToMove), gridToMove.Frame.Size);

			#endif

			_scrollView.AddSubview(gridToMove);

			if (animated){
				UIView.BeginAnimations("changeMonth");
				UIView.SetAnimationDuration(0.4);
				UIView.SetAnimationDelay(0.1);
				UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			}

			_monthGridView.Alpha = 0;

			#if __UNIFIED__

			_monthGridView.Center = new CoreGraphics.CGPoint(_monthGridView.Center.X, _monthGridView.Center.Y - pointsToMove);
			gridToMove.Center = new CoreGraphics.CGPoint(gridToMove.Center.X, gridToMove.Center.Y - pointsToMove);

			_scrollView.Frame = new CoreGraphics.CGRect(
				_scrollView.Frame.Location,
				new CoreGraphics.CGSize(_scrollView.Frame.Width, (gridToMove.Lines) * DayCellHeight));

			#else

			_monthGridView.Center = new PointF(_monthGridView.Center.X, _monthGridView.Center.Y - pointsToMove);
			gridToMove.Center = new PointF(gridToMove.Center.X, gridToMove.Center.Y - pointsToMove);

			_scrollView.Frame = new RectangleF(
				_scrollView.Frame.Location,
				new SizeF(_scrollView.Frame.Width, (gridToMove.Lines) * DayCellHeight));

			#endif

			_scrollView.ContentSize = _scrollView.Frame.Size;
			SetNeedsDisplay();

			if (animated)
				UIView.CommitAnimations();

			_monthGridView = gridToMove;

			UserInteractionEnabled = true;

			AdjustBackgroundColor ();
		}

		/// <summary>
		/// Gos to the specified date date.
		/// </summary>
		/// <param name="targetDate">Target date.</param>
		public void GoToDate(DateTime targetDate)
		{
			if (_monthGridView == null)
				return;

			bool upwards = targetDate >= CurrentMonthYear;

			SelectedDate = targetDate.Date;
			CurrentMonthYear = new DateTime(targetDate.Year, targetDate.Month, 1);

			UserInteractionEnabled = false;

			// Dispatch event
			if (MonthChanged != null)
				MonthChanged(CurrentMonthYear);

			var gridToMove = CreateNewGrid(CurrentMonthYear);
			var pointsToMove = (upwards? 0 + _monthGridView.Lines : 0 - _monthGridView.Lines) * DayCellHeight;

			if (upwards && gridToMove.weekdayOfFirst==0)
				pointsToMove += DayCellHeight;
			if (!upwards && _monthGridView.weekdayOfFirst==0)
				pointsToMove -= DayCellHeight;

			#if __UNIFIED__

			gridToMove.Frame = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, pointsToMove), gridToMove.Frame.Size);

			#else

			gridToMove.Frame = new RectangleF(new PointF(0, pointsToMove), gridToMove.Frame.Size);

			#endif

			_scrollView.AddSubview(gridToMove);

			#if __UNIFIED__

			_monthGridView.Center = new CoreGraphics.CGPoint(_monthGridView.Center.X, _monthGridView.Center.Y - pointsToMove);
			gridToMove.Center = new CoreGraphics.CGPoint(gridToMove.Center.X, gridToMove.Center.Y - pointsToMove);

			_scrollView.Frame = new CoreGraphics.CGRect(
				_scrollView.Frame.Location,
				new CoreGraphics.CGSize(_scrollView.Frame.Width, (gridToMove.Lines) * DayCellHeight));

			#else

			_monthGridView.Center = new PointF(_monthGridView.Center.X, _monthGridView.Center.Y - pointsToMove);
			gridToMove.Center = new PointF(gridToMove.Center.X, gridToMove.Center.Y - pointsToMove);

			_scrollView.Frame = new RectangleF(
			_scrollView.Frame.Location,
			new SizeF(_scrollView.Frame.Width, (gridToMove.Lines) * DayCellHeight));

			#endif

			_monthGridView.Alpha = 0;

			_scrollView.ContentSize = _scrollView.Frame.Size;
			SetNeedsDisplay();

			_monthGridView = gridToMove;

			UserInteractionEnabled = true;

			AdjustBackgroundColor ();
		}

		private void AdjustBackgroundColor()
		{
			if (_scrollView != null)
				_scrollView.BackgroundColor = MonthBackgroundColor;
			BackgroundColor = MonthBackgroundColor;
		}

		private MonthGridView CreateNewGrid(DateTime date){
			var grid = new MonthGridView(this, date);
			grid.CurrentDate = CurrentDate;
			grid.BuildGrid();
			grid.Frame = MainViewSize;
			return grid;
		}

		private void LoadInitialGrids()
		{
			_monthGridView = CreateNewGrid(CurrentMonthYear);

			var rect = _scrollView.Frame;

			#if __UNIFIED__

			rect.Size = new CoreGraphics.CGSize { Height = MainViewSize.Height - HeaderViewSize.Height, Width = MainViewSize.Width };

			#else

			rect.Size = new SizeF { Height = MainViewSize.Height - HeaderViewSize.Height, Width = MainViewSize.Width };

			#endif

			_scrollView.Frame = rect;
		}

		#if __UNIFIED__

		public override void Draw (CoreGraphics.CGRect rect)
		{
			if(TopBar != null)
				TopBar.Draw(new CoreGraphics.CGPoint(0,0));

			DrawDayLabels(rect);
			DrawMonthLabel(rect);
		}

		private void DrawMonthLabel(CoreGraphics.CGRect rect)
		{
			var r = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, (HeaderViewSize.Height / 2) - 15), new CoreGraphics.CGSize { Width = HeaderViewSize.Width, Height = HeaderElementSize });
			UIColor.White.SetColor ();
			CurrentMonthYear.ToString (MonthFormatString, new CultureInfo (NSLocale.CurrentLocale.LanguageCode))
				.DrawString (r, UIFont.SystemFontOfSize (20), UILineBreakMode.WordWrap, UITextAlignment.Center);
		}

		private void DrawDayLabels(CoreGraphics.CGRect rect)
		{
			var font = UIFont.SystemFontOfSize(DayNameHeight);
            UIColor.White.SetColor ();
			var context = UIGraphics.GetCurrentContext();
			context.SaveState();
			var i = 0;

			var cultureInfo = new CultureInfo(NSLocale.CurrentLocale.LanguageCode);

			cultureInfo.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
			var dayNames = cultureInfo.DateTimeFormat.DayNames;

			if (!SundayFirst) 
			{
				// Shift Sunday to the end of the week
				var firstDay = dayNames [0];
				for (int count = 0; count < dayNames.Length - 1; count++)
					dayNames [count] = dayNames [count + 1];
				dayNames [dayNames.Length - 1] = firstDay;
			}

			foreach (var d in dayNames)
			{
				d.Substring (0, 3).DrawString (new CoreGraphics.CGRect (i * DayCellWidth, HeaderViewSize.Height - DayNameHeight - 2, DayCellWidth, DayNameHeight), font, UILineBreakMode.WordWrap, UITextAlignment.Center);
				i++;
			}
			context.RestoreState();
		}

		#else

		public override void Draw(RectangleF rect)
		{
			if(TopBar != null)
			TopBar.Draw(new PointF(0,0));

			DrawDayLabels(rect);
			DrawMonthLabel(rect);
		}

		private void DrawMonthLabel(RectangleF rect)
		{
			var r = new RectangleF(new PointF(0, (HeaderViewSize.Height / 2) - 15), new SizeF { Width = HeaderViewSize.Width, Height = HeaderElementSize });
            UIColor.White.SetColor ();
			DrawString(CurrentMonthYear.ToString(MonthFormatString, new CultureInfo(NSLocale.CurrentLocale.LanguageCode)), 
			r, UIFont.SystemFontOfSize(20),
			UILineBreakMode.WordWrap, UITextAlignment.Center);
		}

		private void DrawDayLabels(RectangleF rect)
		{
			var font = UIFont.SystemFontOfSize(DayNameHeight);
            UIColor.White.SetColor ();
			var context = UIGraphics.GetCurrentContext();
			context.SaveState();
			var i = 0;

			var cultureInfo = new CultureInfo(NSLocale.CurrentLocale.LanguageCode);

			cultureInfo.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
			var dayNames = cultureInfo.DateTimeFormat.DayNames;

			if (!SundayFirst) 
			{
			// Shift Sunday to the end of the week
			var firstDay = dayNames [0];
			for (int count = 0; count < dayNames.Length - 1; count++)
			dayNames [count] = dayNames [count + 1];
			dayNames [dayNames.Length - 1] = firstDay;
			}

			foreach (var d in dayNames)
			{
			DrawString(d.Substring(0, 3), new RectangleF(i*DayCellWidth, HeaderViewSize.Height - DayNameHeight - 2, DayCellWidth, DayNameHeight), font,
			UILineBreakMode.WordWrap, UITextAlignment.Center);
			i++;
			}
			context.RestoreState();
		}

		#endif
	}
}