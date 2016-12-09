using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Support.V4.View;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Text;
using Java.Util;
using Exception = System.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;
using String = System.String;

namespace FlexyPark.UI.Droid.Controls.CalenarDay
{
    public class WeekView : View
    {
        #region Variables
        public int Language = 0;
        public static int LENGTH_SHORT = 1;
        public static int LENGTH_LONG = 2;
        private Context mContext;
        private Calendar mToday;
        private Calendar mStartDate;
        private Paint mTimeTextPaint;
        private float mTimeTextWidth;
        private float mTimeTextHeight;
        private Paint mHeaderTextPaint;
        private float mHeaderTextHeight;
        private GestureDetectorCompat mGestureDetector;

        private myGestureListener mGestureListener;

        private static OverScroller mScroller;
        private PointF mCurrentOrigin = new PointF(0f, 0f);
        private Direction mCurrentScrollDirection = Direction.NONE;
        private Paint mHeaderBackgroundPaint;
        private float mWidthPerDay;
        private Paint mDayBackgroundPaint;
        private Paint mHourSeparatorPaint;
        private float mHeaderMarginBottom;
        private Paint mTodayBackgroundPaint;
        private Paint mTodayHeaderTextPaint;
        private Paint mEventBackgroundPaint;
        private float mHeaderColumnWidth;
        private List<EventRect> mEventRects;
        private TextPaint mEventTextPaint;
        private Paint mHeaderColumnBackgroundPaint;
        private Scroller mStickyScroller;
        private int[] mFetchedMonths = new int[3];
        private bool mRefreshEvents = false;
        private float mDistanceY = 0;
        private float mDistanceX = 0;
        private Direction mCurrentFlingDirection = Direction.NONE;

        // Attributes and their default values.
        private int mHourHeight = 50;
        private int mColumnGap = 10;
        private int mFirstDayOfWeek = Calendar.Monday;
        private int mTextSize = 12;
        private int mHeaderColumnPadding = 10;
        private Color mHeaderColumnTextColor = Color.Black;
        private int mNumberOfVisibleDays = 3;
        private int mHeaderRowPadding = 10;
        private Color mHeaderRowBackgRoundColor = Color.White;
        private Color mDayBackgRoundColor = Color.Rgb(245, 245, 245);
        private Color mHourSeparatorColor = Color.Rgb(230, 230, 230);
        private Color mTodayBackgRoundColor = Color.Rgb(239, 247, 254);
        private int mHourSeparatorHeight = 2;
        private Color mTodayHeaderTextColor = Color.Rgb(39, 137, 228);
        private int mEventTextSize = 12;
        private Color mEventTextColor = Color.Black;
        private int mEventPadding = 8;
        private Color mHeaderColumnBackgRoundColor = Color.White;
        private Color mDefaultEventColor;
        private bool mIsFirstDraw = true;
        private bool mAreDimensionsInvalid = true;
        private int mDayNameLength = LENGTH_LONG;
        private int mOverlappingEventGap = 0;
        private int mEventMarginVertical = 0;
        private float mXScrollingSpeed = 1f;
        private Calendar mFirstVisibleDay;
        private Calendar mLastVisibleDay;
        private Calendar mScrollToDay = null;
        private double mScrollToHour = -1;

        // Listeners.
        private EventClickListener mEventClickListener;
        private EventLongPressListener mEventLongPressListener;
        private MonthChangeListener mMonthChangeListener;
        private EmptyViewClickListener mEmptyViewClickListener;
        private EmptyViewLongPressListener mEmptyViewLongPressListener;
        private DateTimeInterpreter mDateTimeInterpreter;
        private ScrollListener mScrollListener;

        #endregion

        #region Constructors


        protected WeekView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {



            init();
        }

        public WeekView(Context context)
            : base(context)
        {




            init();
        }

        public WeekView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            mContext = context;


            // Get the attribute values (if any).
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.WeekView, 0, 0);
            try
            {
                mFirstDayOfWeek = a.GetInteger(Resource.Styleable.WeekView_firstDayOfWeek, mFirstDayOfWeek);
                mHourHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourHeight, mHourHeight);
                //mTextSize = (int)Resources.GetDimension(Resource.Dimension.textMediumLarge);
                mTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_textSizeX, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mTextSize, context.Resources.DisplayMetrics));
                mHeaderColumnPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerColumnPadding, mHeaderColumnPadding);
                mColumnGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_columnGap, mColumnGap);
                mHeaderColumnTextColor = a.GetColor(Resource.Styleable.WeekView_headerColumnTextColor, mHeaderColumnTextColor);
                mNumberOfVisibleDays = a.GetInteger(Resource.Styleable.WeekView_noOfVisibleDays, mNumberOfVisibleDays);
                mHeaderRowPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerRowPadding, mHeaderRowPadding);
                mHeaderRowBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerRowBackgroundColor, mHeaderRowBackgRoundColor);
                mDayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_dayBackgroundColor, mDayBackgRoundColor);
                mHourSeparatorColor = a.GetColor(Resource.Styleable.WeekView_hourSeparatorColor, mHourSeparatorColor);
                mTodayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_todayBackgroundColor, mTodayBackgRoundColor);
                mHourSeparatorHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mHourSeparatorHeight);
                mTodayHeaderTextColor = a.GetColor(Resource.Styleable.WeekView_todayHeaderTextColor, mTodayHeaderTextColor);
                mEventTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventTextSize, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mEventTextSize, context.Resources.DisplayMetrics));
                mEventTextColor = a.GetColor(Resource.Styleable.WeekView_eventTextColor, mEventTextColor);
                mEventPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mEventPadding);
                mHeaderColumnBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerColumnBackground, mHeaderColumnBackgRoundColor);
                mDayNameLength = a.GetInteger(Resource.Styleable.WeekView_dayNameLength, mDayNameLength);
                mOverlappingEventGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_overlappingEventGap, mOverlappingEventGap);
                mEventMarginVertical = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventMarginVertical, mEventMarginVertical);
                mXScrollingSpeed = a.GetFloat(Resource.Styleable.WeekView_xScrollingSpeed, mXScrollingSpeed);
            }
            finally
            {
                a.Recycle();
            }

            init();
        }

        public WeekView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {

            // Hold references.
            mContext = context;


            // Get the attribute values (if any).
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.WeekView, 0, 0);
            try
            {
                mFirstDayOfWeek = a.GetInteger(Resource.Styleable.WeekView_firstDayOfWeek, mFirstDayOfWeek);
                mHourHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourHeight, mHourHeight);
                mTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_textSizeX, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mTextSize, context.Resources.DisplayMetrics));
                //mTextSize = (int) Resources.GetDimension(Resource.Dimension.textMediumLarge);
                mHeaderColumnPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerColumnPadding, mHeaderColumnPadding);
                mColumnGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_columnGap, mColumnGap);
                mHeaderColumnTextColor = a.GetColor(Resource.Styleable.WeekView_headerColumnTextColor, mHeaderColumnTextColor);
                mNumberOfVisibleDays = a.GetInteger(Resource.Styleable.WeekView_noOfVisibleDays, mNumberOfVisibleDays);
                mHeaderRowPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerRowPadding, mHeaderRowPadding);
                mHeaderRowBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerRowBackgroundColor, mHeaderRowBackgRoundColor);
                mDayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_dayBackgroundColor, mDayBackgRoundColor);
                mHourSeparatorColor = a.GetColor(Resource.Styleable.WeekView_hourSeparatorColor, mHourSeparatorColor);
                mTodayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_todayBackgroundColor, mTodayBackgRoundColor);
                mHourSeparatorHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mHourSeparatorHeight);
                mTodayHeaderTextColor = a.GetColor(Resource.Styleable.WeekView_todayHeaderTextColor, mTodayHeaderTextColor);
                mEventTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventTextSize, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mEventTextSize, context.Resources.DisplayMetrics));
                mEventTextColor = a.GetColor(Resource.Styleable.WeekView_eventTextColor, mEventTextColor);
                mEventPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mEventPadding);
                mHeaderColumnBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerColumnBackground, mHeaderColumnBackgRoundColor);
                mDayNameLength = a.GetInteger(Resource.Styleable.WeekView_dayNameLength, mDayNameLength);
                mOverlappingEventGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_overlappingEventGap, mOverlappingEventGap);
                mEventMarginVertical = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventMarginVertical, mEventMarginVertical);
                mXScrollingSpeed = a.GetFloat(Resource.Styleable.WeekView_xScrollingSpeed, mXScrollingSpeed);
            }
            finally
            {
                a.Recycle();
            }

            init();
        }

        public WeekView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            // Hold references.
            mContext = context;


            // Get the attribute values (if any).
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.WeekView, 0, 0);
            try
            {
                mFirstDayOfWeek = a.GetInteger(Resource.Styleable.WeekView_firstDayOfWeek, mFirstDayOfWeek);
                mHourHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourHeight, mHourHeight);
                //mTextSize = (int)Resources.GetDimension(Resource.Dimension.textMediumLarge);
                mTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_textSizeX, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mTextSize, context.Resources.DisplayMetrics));
                mHeaderColumnPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerColumnPadding, mHeaderColumnPadding);
                mColumnGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_columnGap, mColumnGap);
                mHeaderColumnTextColor = a.GetColor(Resource.Styleable.WeekView_headerColumnTextColor, mHeaderColumnTextColor);
                mNumberOfVisibleDays = a.GetInteger(Resource.Styleable.WeekView_noOfVisibleDays, mNumberOfVisibleDays);
                mHeaderRowPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_headerRowPadding, mHeaderRowPadding);
                mHeaderRowBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerRowBackgroundColor, mHeaderRowBackgRoundColor);
                mDayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_dayBackgroundColor, mDayBackgRoundColor);
                mHourSeparatorColor = a.GetColor(Resource.Styleable.WeekView_hourSeparatorColor, mHourSeparatorColor);
                mTodayBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_todayBackgroundColor, mTodayBackgRoundColor);
                mHourSeparatorHeight = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mHourSeparatorHeight);
                mTodayHeaderTextColor = a.GetColor(Resource.Styleable.WeekView_todayHeaderTextColor, mTodayHeaderTextColor);
                mEventTextSize = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventTextSize, (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, mEventTextSize, context.Resources.DisplayMetrics));
                mEventTextColor = a.GetColor(Resource.Styleable.WeekView_eventTextColor, mEventTextColor);
                mEventPadding = a.GetDimensionPixelSize(Resource.Styleable.WeekView_hourSeparatorHeight, mEventPadding);
                mHeaderColumnBackgRoundColor = a.GetColor(Resource.Styleable.WeekView_headerColumnBackground, mHeaderColumnBackgRoundColor);
                mDayNameLength = a.GetInteger(Resource.Styleable.WeekView_dayNameLength, mDayNameLength);
                mOverlappingEventGap = a.GetDimensionPixelSize(Resource.Styleable.WeekView_overlappingEventGap, mOverlappingEventGap);
                mEventMarginVertical = a.GetDimensionPixelSize(Resource.Styleable.WeekView_eventMarginVertical, mEventMarginVertical);
                mXScrollingSpeed = a.GetFloat(Resource.Styleable.WeekView_xScrollingSpeed, mXScrollingSpeed);
            }
            finally
            {
                a.Recycle();
            }

            init();

        }

        #endregion

        #region Main

        #region MyGestureListener

        class myGestureListener : Java.Lang.Object, GestureDetector.IOnGestureListener
        {
            private WeekView mParent;
            public OverScroller mOverScroller;
            public Scroller mStickyScroller;

            public float mDistanceX, mDistanceY, mXScrollingSpeed;
            private PointF mCurrentOrigin;
            int mHourHeight;
            private float mHeaderTextHeight;
            int mHeaderRowPadding;
            List<EventRect> mEventRects;
            private EventClickListener mEventClickListener;

            private View view;
            private float mHeaderMarginBottom;
            private float mHeaderColumnWidth;
            private EmptyViewClickListener mEmptyViewClickListener;
            EventLongPressListener mEventLongPressListener;
            private EmptyViewLongPressListener mEmptyViewLongPressListener;


            public myGestureListener(WeekView parent, OverScroller scroller, Scroller stickyScroller, Direction currentScrollDirection, Direction currentFlingDirection, float distanceX, float distanceY, float xScrollingSpeed, PointF currentOrigin,
                 int hourHeight,
                float headerTextHeight,
                int headerRowPadding,
               EmptyViewClickListener emptyViewClickListener,

                EventClickListener eventClickListener, View mView,
                float headerMarginBottom,
                float headerColumnWidth,
                EventLongPressListener eventLongPressListener,
                 EmptyViewLongPressListener emptyViewLongPressListener)
            {
                mParent = parent;
                mScroller = scroller;
                mStickyScroller = stickyScroller;

                mDistanceX = distanceX;
                mDistanceY = distanceY;
                mXScrollingSpeed = xScrollingSpeed;
                mCurrentOrigin = currentOrigin;
                mHourHeight = hourHeight;
                mHeaderTextHeight = headerTextHeight;
                mHeaderRowPadding = headerRowPadding;

                mEventClickListener = eventClickListener;

                view = mView;
                mHeaderMarginBottom = headerMarginBottom;
                mHeaderColumnWidth = headerColumnWidth;
                mEmptyViewClickListener = emptyViewClickListener;
                mEventLongPressListener = eventLongPressListener;
                mEmptyViewLongPressListener = emptyViewLongPressListener;
            }


            public bool OnDown(MotionEvent e)
            {
                mScroller.ForceFinished(true);
                mStickyScroller.ForceFinished(true);
                return true;
            }

            public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                mScroller.ForceFinished(true);
                mStickyScroller.ForceFinished(true);

                if (mParent.mCurrentFlingDirection == Direction.HORIZONTAL)
                {
                    mScroller.Fling((int)mCurrentOrigin.X, 0, (int)(velocityX * mXScrollingSpeed), 0, Integer.MinValue, Integer.MaxValue, 0, 0);
                }
                else if (mParent.mCurrentFlingDirection == Direction.VERTICAL)
                {
                    //  mScroller.fling(0, (int)mCurrentOrigin.y, 0, (int)velocityY, 0, 0, (int)-(mHourHeight * 24 + mHeaderTextHeight + mHeaderRowPadding * 2 - getHeight()), 0);
                    mScroller.Fling(0, (int)mCurrentOrigin.Y, 0, (int)velocityY, 0, 0, (int)-(mHourHeight * 24 + mHeaderTextHeight + mHeaderRowPadding * 2 - mParent.Height), 0);
                }

                ViewCompat.PostInvalidateOnAnimation(view);

                return true;
            }

            public void OnLongPress(MotionEvent e)
            {

                if (mEventLongPressListener != null && mEventRects != null)
                {
                    List<EventRect> reversedEventRects = mEventRects;
                    //   Collections.reverse(reversedEventRects);

                    reversedEventRects.Reverse(0, reversedEventRects.Count);


                    foreach (var eventRect in reversedEventRects)
                    {
                        if (eventRect.rectF != null && e.GetX() > eventRect.rectF.Left && e.GetX() < eventRect.rectF.Right && e.GetY() > eventRect.rectF.Top && e.GetY() < eventRect.rectF.Bottom)
                        {
                            mEventLongPressListener.onEventLongPress(eventRect.originalEvent, eventRect.rectF);
                            mParent.PerformHapticFeedback(FeedbackConstants.LongPress);
                            return;
                        }
                    }
                }

                // If the tap was on in an empty space, then trigger the callback.
                if (mEmptyViewLongPressListener != null && e.GetX() > mHeaderColumnWidth && e.GetY() > (mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom))
                {
                    Calendar selectedTime = mParent.getTimeFromPoint(e.GetX(), e.GetY());
                    if (selectedTime != null)
                    {
                        mParent.PerformHapticFeedback(FeedbackConstants.LongPress);
                        mEmptyViewLongPressListener.onEmptyViewLongPress(selectedTime);
                    }
                }

            }

            public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                if (mParent.mCurrentScrollDirection == Direction.NONE)
                {
                    if (Math.Abs(distanceX) > Math.Abs(distanceY))
                    {
                        mParent.mCurrentScrollDirection = Direction.HORIZONTAL;
                        mParent.mCurrentFlingDirection = Direction.HORIZONTAL;

                    }
                    else
                    {
                        mParent.mCurrentFlingDirection = Direction.VERTICAL;
                        mParent.mCurrentScrollDirection = Direction.VERTICAL;


                    }
                }
                mDistanceX = distanceX * mXScrollingSpeed;
                mDistanceY = distanceY;

                mParent.Invalidate();

                return true;
            }

            public void OnShowPress(MotionEvent e)
            {

            }

            public bool OnSingleTapUp(MotionEvent e)
            {
                // If the tap was on an event then trigger the callback.
                if (mEventRects != null && mEventClickListener != null)
                {
                    List<EventRect> reversedEventRects = new List<EventRect>(mEventRects);
                    // Collections.Reverse(reversedEventRects);

                    reversedEventRects.Reverse(0, reversedEventRects.Count);

                    foreach (var reversedEventRect in reversedEventRects)
                    {
                        if (reversedEventRect.rectF != null && e.GetX() > reversedEventRect.rectF.Left && e.GetX() < reversedEventRect.rectF.Right && e.GetY() > reversedEventRect.rectF.Top && e.GetY() < reversedEventRect.rectF.Bottom)
                        {
                            mEventClickListener.onEventClick(reversedEventRect.originalEvent, reversedEventRect.rectF);
                            mParent.PlaySoundEffect(SoundEffects.Click);
                            return OnSingleTapUp(e);
                        }
                    }
                }

                // If the tap was on in an empty space, then trigger the callback.
                if (mEmptyViewClickListener != null && e.GetX() > mHeaderColumnWidth && e.GetY() > (mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom))
                {
                    Calendar selectedTime = mParent.getTimeFromPoint(e.GetX(), e.GetY());
                    if (selectedTime != null)
                    {

                        mParent.PlaySoundEffect(SoundEffects.Click);
                        mEmptyViewClickListener.onEmptyViewClicked(selectedTime);
                    }
                }

                //return OnSingleTapUp(e);
                return true;
            }

        }

        #endregion

        #endregion

        #region Enum

        private enum Direction
        {
            NONE,
            HORIZONTAL,
            VERTICAL
        }

        #endregion

        #region Overrides

        public override void Invalidate()
        {
            base.Invalidate();
            mAreDimensionsInvalid = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {

            if (e.Action == MotionEventActions.Up)
            {

                if (mCurrentScrollDirection == Direction.HORIZONTAL)
                {
                    float leftDays = (float)System.Math.Round(mCurrentOrigin.X / (mWidthPerDay + mColumnGap));
                    int nearestOrigin = (int)(mCurrentOrigin.X - leftDays * (mWidthPerDay + mColumnGap));
                    mStickyScroller.StartScroll((int)mCurrentOrigin.X, 0, -nearestOrigin, 0);
                    ViewCompat.PostInvalidateOnAnimation(this);
                }
                mCurrentScrollDirection = Direction.NONE;
            }
            return mGestureDetector.OnTouchEvent(e);


        }

        public override void ComputeScroll()
        {
            base.ComputeScroll();

            if (mScroller.ComputeScrollOffset())
            {
                if (System.Math.Abs(mScroller.FinalX - mScroller.CurrX) < mWidthPerDay + mColumnGap &&
                    System.Math.Abs(mScroller.FinalX - mScroller.StartX) != 0)
                {
                    mScroller.ForceFinished(true);
                    float leftDays = (float)System.Math.Round(mCurrentOrigin.X / (mWidthPerDay + mColumnGap));
                    if (mScroller.FinalX < mScroller.CurrX)
                        leftDays--;
                    else
                        leftDays++;
                    int nearestOrigin = (int)(mCurrentOrigin.X - leftDays * (mWidthPerDay + mColumnGap));
                    mStickyScroller.StartScroll((int)mCurrentOrigin.X, 0, -nearestOrigin, 0);
                    ViewCompat.PostInvalidateOnAnimation(this);
                }
                else
                {
                    if (mCurrentFlingDirection == Direction.VERTICAL) mCurrentOrigin.Y = mScroller.CurrY;
                    else mCurrentOrigin.X = mScroller.CurrX;
                    ViewCompat.PostInvalidateOnAnimation(this);
                }
            }
            if (mStickyScroller.ComputeScrollOffset())
            {
                mCurrentOrigin.X = mStickyScroller.CurrX;
                ViewCompat.PostInvalidateOnAnimation(this);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            // Draw the header row.
            drawHeaderRowAndEvents(canvas);

            // Draw the time column and all the axes/separators.
            drawTimeColumnAndAxes(canvas);

            // Hide everything in the first cell (top left corner).
            canvas.DrawRect(0, 0, mTimeTextWidth + mHeaderColumnPadding * 2, mHeaderTextHeight + mHeaderRowPadding * 2,
                mHeaderBackgroundPaint);

            // Hide anything that is in the bottom margin of the header row.
            canvas.DrawRect(mHeaderColumnWidth, mHeaderTextHeight + mHeaderRowPadding * 2, Width,
                mHeaderRowPadding * 2 + mHeaderTextHeight + mHeaderMarginBottom + mTimeTextHeight / 2 -
                mHourSeparatorHeight / 2, mHeaderColumnBackgroundPaint);

        }

        #endregion

        #region Init()

        private void init()
        {
            // Get the date today.
            mToday = Calendar.Instance;
            mToday.Set(Calendar.HourOfDay, 0);
            mToday.Set(Calendar.Minute, 0);
            mToday.Set(Calendar.Second, 0);

            // Scrolling initialization.



            // Measure settings for time column.
            mTimeTextPaint = new Paint(PaintFlags.AntiAlias);
            mTimeTextPaint.TextAlign = (Paint.Align.Right);
            mTimeTextPaint.TextSize = (mTextSize);
            mTimeTextPaint.SetTypeface(Typeface.CreateFromAsset(this.Context.Assets, "HelveticaNeue.ttf"));

            //mTimeTextPaint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Px,
            //    Resources.GetDimension(Resource.Dimension.textExtraLarge), Resources.DisplayMetrics);

            mTimeTextPaint.Color = (mHeaderColumnTextColor);
            Rect rect = new Rect();
            mTimeTextPaint.GetTextBounds("00 PM", 0, "00 PM".Length, rect);
            mTimeTextWidth = mTimeTextPaint.MeasureText("00 PM");
            mTimeTextHeight = rect.Height();
            mHeaderMarginBottom = mTimeTextHeight / 2;

            // Measure settings for header row.
            mHeaderTextPaint = new Paint(PaintFlags.AntiAlias);
            mHeaderTextPaint.Color = (mHeaderColumnTextColor);
            mHeaderTextPaint.TextAlign = (Paint.Align.Center);
            mHeaderTextPaint.TextSize = (mTextSize);
            mHeaderTextPaint.GetTextBounds("00 PM", 0, "00 PM".Length, rect);
            mHeaderTextHeight = rect.Height();
            mHeaderTextPaint.SetTypeface(Typeface.DefaultBold);

            // Prepare header backgRound paint.
            mHeaderBackgroundPaint = new Paint();
            mHeaderBackgroundPaint.Color = (mHeaderRowBackgRoundColor);

            // Prepare day backgRound color paint.
            mDayBackgroundPaint = new Paint();
            mDayBackgroundPaint.Color = (mDayBackgRoundColor);

            // Prepare hour separator color paint.
            mHourSeparatorPaint = new Paint();
            mHourSeparatorPaint.SetStyle(Paint.Style.Stroke);
            mHourSeparatorPaint.StrokeWidth = (mHourSeparatorHeight);
            mHourSeparatorPaint.Color = (mHourSeparatorColor);

            // Prepare today backgRound color paint.
            mTodayBackgroundPaint = new Paint();
            mTodayBackgroundPaint.Color = (mTodayBackgRoundColor);

            // Prepare today header text color paint.
            mTodayHeaderTextPaint = new Paint(PaintFlags.AntiAlias);
            mTodayHeaderTextPaint.TextAlign = (Paint.Align.Center);
            mTodayHeaderTextPaint.TextSize = (mTextSize);

            mTodayHeaderTextPaint.SetTypeface(Typeface.DefaultBold);
            mTodayHeaderTextPaint.Color = (mTodayHeaderTextColor);

            // Prepare event backgRound color.
            mEventBackgroundPaint = new Paint();
            mEventBackgroundPaint.Color = (Color.Rgb(174, 208, 238));

            // Prepare header column backgRound color.
            mHeaderColumnBackgroundPaint = new Paint();
            mHeaderColumnBackgroundPaint.Color = (mHeaderColumnBackgRoundColor);

            // Prepare event text size and color.
            mEventTextPaint = new TextPaint(PaintFlags.AntiAlias | PaintFlags.LinearText);
            mEventTextPaint.SetStyle(Paint.Style.Fill);
            mEventTextPaint.Color = (mEventTextColor);
            mEventTextPaint.TextSize = (mEventTextSize);
            mStartDate = (Calendar)mToday.Clone();

            // Set default event color.
            mDefaultEventColor = Color.ParseColor("#9fc6e7");


            mScroller = new OverScroller(mContext);
            mStickyScroller = new Scroller(mContext);

            //mGestureListener = new myGestureListener(this, mScroller, mStickyScroller, mCurrentScrollDirection, mCurrentFlingDirection, mDistanceX, mDistanceY, mXScrollingSpeed, mCurrentOrigin,
            //     mHourHeight,
            //    mHeaderTextHeight,
            //    mHeaderRowPadding,
            //   mEmptyViewClickListener,
            //    Height,
            //     mEventClickListener, this,
            //    mHeaderMarginBottom,
            //    mHeaderColumnWidth,
            //    mEventLongPressListener,
            //     mEmptyViewLongPressListener);



            //mGestureDetector = new GestureDetectorCompat(mContext, mGestureListener);
        }

        #endregion

        #region Setting and Getting Properties

        public void setOnEventClickListener(EventClickListener listener)
        {
            this.mEventClickListener = listener;
        }

        public EventClickListener getEventClickListener()
        {
            return mEventClickListener;
        }

        public MonthChangeListener getMonthChangeListener()
        {
            return mMonthChangeListener;
        }

        public void setMonthChangeListener(MonthChangeListener monthChangeListener)
        {
            this.mMonthChangeListener = monthChangeListener;
        }

        public EventLongPressListener getEventLongPressListener()
        {
            return mEventLongPressListener;
        }

        public void setEventLongPressListener(EventLongPressListener eventLongPressListener)
        {
            this.mEventLongPressListener = eventLongPressListener;
        }

        public void setEmptyViewClickListener(EmptyViewClickListener emptyViewClickListener)
        {
            this.mEmptyViewClickListener = emptyViewClickListener;
        }

        public EmptyViewClickListener getEmptyViewClickListener()
        {
            return mEmptyViewClickListener;
        }

        public void setEmptyViewLongPressListener(EmptyViewLongPressListener emptyViewLongPressListener)
        {
            this.mEmptyViewLongPressListener = emptyViewLongPressListener;
        }

        public EmptyViewLongPressListener getEmptyViewLongPressListener()
        {
            return mEmptyViewLongPressListener;
        }

        public void setScrollListener(ScrollListener scrolledListener)
        {
            this.mScrollListener = scrolledListener;
        }

        public ScrollListener getScrollListener()
        {
            return mScrollListener;
        }


        public DateTimeInterpreter getDateTimeInterpreter()
        {
            if (mDateTimeInterpreter == null)
            {
                mDateTimeInterpreter = new NewDateTimeInterpreter(mDayNameLength, LENGTH_SHORT);
            }
            return mDateTimeInterpreter;
        }



        public void setDateTimeInterpreter(DateTimeInterpreter dateTimeInterpreter)
        {
            this.mDateTimeInterpreter = dateTimeInterpreter;
        }

        public int getNumberOfVisibleDays()
        {
            return mNumberOfVisibleDays;
        }

        public void setNumberOfVisibleDays(int numberOfVisibleDays)
        {
            this.mNumberOfVisibleDays = numberOfVisibleDays;
            mCurrentOrigin.X = 0;
            mCurrentOrigin.Y = 0;
            Invalidate();
        }

        public int getHourHeight()
        {
            return mHourHeight;
        }

        public void setHourHeight(int hourHeight)
        {
            mHourHeight = hourHeight;
            Invalidate();
        }

        public int getColumnGap()
        {
            return mColumnGap;
        }

        public void setColumnGap(int columnGap)
        {
            mColumnGap = columnGap;
            Invalidate();
        }

        public int getFirstDayOfWeek()
        {
            return mFirstDayOfWeek;
        }

        public void setFirstDayOfWeek(int firstDayOfWeek)
        {
            mFirstDayOfWeek = firstDayOfWeek;
            Invalidate();
        }

        public int getTextSize()
        {
            return mTextSize;
        }

        public void setTextSize(int textSize)
        {
            mTextSize = textSize;
            mTodayHeaderTextPaint.TextSize = (mTextSize);
            mHeaderTextPaint.TextSize = (mTextSize);
            mTimeTextPaint.TextSize = (mTextSize);
            Invalidate();
        }

        public int getHeaderColumnPadding()
        {
            return mHeaderColumnPadding;
        }

        public void setHeaderColumnPadding(int headerColumnPadding)
        {
            mHeaderColumnPadding = headerColumnPadding;
            Invalidate();
        }

        public int getHeaderColumnTextColor()
        {
            return mHeaderColumnTextColor;
        }

        public void setHeaderColumnTextColor(Color headerColumnTextColor)
        {
            // mHeaderColumnTextColor = headerColumnTextColor;
            mHeaderColumnTextColor = headerColumnTextColor;
            Invalidate();
        }

        public int getHeaderRowPadding()
        {
            return mHeaderRowPadding;
        }

        public void setHeaderRowPadding(int headerRowPadding)
        {
            mHeaderRowPadding = headerRowPadding;
            Invalidate();
        }

        public int getHeaderRowBackgRoundColor()
        {
            return mHeaderRowBackgRoundColor;
        }

        public void setHeaderRowBackgRoundColor(Color headerRowBackgRoundColor)
        {
            mHeaderRowBackgRoundColor = headerRowBackgRoundColor;
            Invalidate();
        }

        public int getDayBackgRoundColor()
        {
            return mDayBackgRoundColor;
        }

        public void setDayBackgRoundColor(Color dayBackgRoundColor)
        {
            mDayBackgRoundColor = dayBackgRoundColor;
            Invalidate();
        }

        public int getHourSeparatorColor()
        {
            return mHourSeparatorColor;
        }

        public void setHourSeparatorColor(Color hourSeparatorColor)
        {
            mHourSeparatorColor = hourSeparatorColor;
            Invalidate();
        }

        public int getTodayBackgRoundColor()
        {
            return mTodayBackgRoundColor;
        }

        public void setTodayBackgRoundColor(Color todayBackgRoundColor)
        {
            mTodayBackgRoundColor = todayBackgRoundColor;
            Invalidate();
        }

        public int getHourSeparatorHeight()
        {
            return mHourSeparatorHeight;
        }

        public void setHourSeparatorHeight(int hourSeparatorHeight)
        {
            mHourSeparatorHeight = hourSeparatorHeight;
            Invalidate();
        }

        public int getTodayHeaderTextColor()
        {
            return mTodayHeaderTextColor;
        }

        public void setTodayHeaderTextColor(Color todayHeaderTextColor)
        {
            mTodayHeaderTextColor = todayHeaderTextColor;
            Invalidate();
        }

        public int getEventTextSize()
        {
            return mEventTextSize;
        }

        public void setEventTextSize(int eventTextSize)
        {
            mEventTextSize = eventTextSize;
            mEventTextPaint.TextSize = (mEventTextSize);
            Invalidate();
        }

        public int getEventTextColor()
        {
            return mEventTextColor;
        }

        public void setEventTextColor(Color eventTextColor)
        {
            mEventTextColor = eventTextColor;
            Invalidate();
        }

        public int getEventPadding()
        {
            return mEventPadding;
        }

        public void setEventPadding(int eventPadding)
        {
            mEventPadding = eventPadding;
            Invalidate();
        }

        public int getHeaderColumnBackgRoundColor()
        {
            return mHeaderColumnBackgRoundColor;
        }

        public void setHeaderColumnBackgRoundColor(Color headerColumnBackgRoundColor)
        {
            // mHeaderColumnBackgRoundColor = headerColumnBackgRoundColor;
            mHeaderColumnBackgRoundColor = headerColumnBackgRoundColor;
            Invalidate();
        }

        public int getDefaultEventColor()
        {
            return mDefaultEventColor;
        }

        public void setDefaultEventColor(Color defaultEventColor)
        {
            mDefaultEventColor = defaultEventColor;
            Invalidate();
        }

        public int getDayNameLength()
        {
            return mDayNameLength;
        }

        public void setDayNameLength(int length)
        {
            if (length != LENGTH_LONG && length != LENGTH_SHORT)
            {
                throw new IllegalArgumentException("length parameter must be either LENGTH_LONG or LENGTH_SHORT");
            }
            this.mDayNameLength = length;
        }

        public int getOverlappingEventGap()
        {
            return mOverlappingEventGap;
        }


        public void setOverlappingEventGap(int overlappingEventGap)
        {
            this.mOverlappingEventGap = overlappingEventGap;
            Invalidate();
        }

        public int getEventMarginVertical()
        {
            return mEventMarginVertical;
        }

        public void setEventMarginVertical(int eventMarginVertical)
        {
            this.mEventMarginVertical = eventMarginVertical;
            Invalidate();
        }

        public Calendar getFirstVisibleDay()
        {
            return mFirstVisibleDay;
        }

        public Calendar getLastVisibleDay()
        {
            return mLastVisibleDay;
        }

        public float getXScrollingSpeed()
        {
            return mXScrollingSpeed;
        }

        public void setXScrollingSpeed(float xScrollingSpeed)
        {
            this.mXScrollingSpeed = xScrollingSpeed;
        }

        #endregion

        #region Methods


        public void Init()
        {
            mGestureListener = new myGestureListener(this, mScroller, mStickyScroller, mCurrentScrollDirection, mCurrentFlingDirection, mDistanceX, mDistanceY, mXScrollingSpeed, mCurrentOrigin,
                mHourHeight,
               mHeaderTextHeight,
               mHeaderRowPadding,
               mEmptyViewClickListener,
               mEventClickListener, this,
               mHeaderMarginBottom,
               mHeaderColumnWidth,
               mEventLongPressListener,
               mEmptyViewLongPressListener);



            mGestureDetector = new GestureDetectorCompat(mContext, mGestureListener);
        }


        private void drawTimeColumnAndAxes(Canvas canvas)
        {
            // Do not let the view go above/below the limit due to scrolling. Set the max and min limit of the scroll.
            if (mCurrentScrollDirection == Direction.VERTICAL)
            {
                if (mCurrentOrigin.Y - mDistanceY > 0) mCurrentOrigin.Y = 0;
                else if (mCurrentOrigin.Y - mDistanceY <
                         -(mHourHeight * 24 + mHeaderTextHeight + mHeaderRowPadding * 2 - Height))
                    mCurrentOrigin.Y = -(mHourHeight * 24 + mHeaderTextHeight + mHeaderRowPadding * 2 - Height);
                else mCurrentOrigin.Y -= mDistanceY;
            }

            // Draw the backgRound color for the header column.
            canvas.DrawRect(0, mHeaderTextHeight + mHeaderRowPadding * 2, mHeaderColumnWidth, Height,
                mHeaderColumnBackgroundPaint);

            for (int i = 0; i < 24; i++)
            {
                float top = mHeaderTextHeight + mHeaderRowPadding * 2 + mCurrentOrigin.Y + mHourHeight * i +
                            mHeaderMarginBottom;

                // Draw the text if its y position is not outside of the visible area. The pivot point of the text is the point at the bottom-right corner.
                string time = getDateTimeInterpreter().interpretTime(i);
                if (time == null)
                    throw new IllegalStateException("A DateTimeInterpreter must not return null time");
                if (top < Height)
                    canvas.DrawText(time, mTimeTextWidth + mHeaderColumnPadding, top + mTimeTextHeight, mTimeTextPaint);
            }
        }


        public void drawHeaderRowAndEvents(Canvas canvas)
        {
            // Calculate the available width for each day.
            mHeaderColumnWidth = mTimeTextWidth + mHeaderColumnPadding * 2;
            mWidthPerDay = Width - mHeaderColumnWidth - mColumnGap * (mNumberOfVisibleDays - 1);
            mWidthPerDay = mWidthPerDay / mNumberOfVisibleDays;

            if (mAreDimensionsInvalid)
            {
                mAreDimensionsInvalid = false;
                if (mScrollToDay != null)
                    goToDate(mScrollToDay);

                mAreDimensionsInvalid = false;
                if (mScrollToHour >= 0)
                    goToHour(mScrollToHour);

                mScrollToDay = null;
                mScrollToHour = -1;
                mAreDimensionsInvalid = false;
            }
            if (mIsFirstDraw)
            {
                mIsFirstDraw = false;

                // If the week view is being drawn for the first time, then consider the first day of the week.
                if (mNumberOfVisibleDays >= 7 && mToday.Get(Calendar.DayOfWeek) != mFirstDayOfWeek)
                {
                    int difference = (7 + (mToday.Get(Calendar.DayOfWeek) - mFirstDayOfWeek)) % 7;
                    mCurrentOrigin.X += (mWidthPerDay + mColumnGap) * difference;
                }
            }

            // Consider scroll offset.
            if (mCurrentScrollDirection == Direction.HORIZONTAL) mCurrentOrigin.X -= mDistanceX;
            int leftDaysWithGaps = (int)-(System.Math.Ceiling(mCurrentOrigin.X / (mWidthPerDay + mColumnGap)));
            float startFromPixel = mCurrentOrigin.X + (mWidthPerDay + mColumnGap) * leftDaysWithGaps +
                    mHeaderColumnWidth;
            float startPixel = startFromPixel;

            // Prepare to iterate for each day.
            Calendar day = (Calendar)mToday.Clone();
            day.Add(Calendar.Hour, 6);

            // Prepare to iterate for each hour to draw the hour lines.
            int lineCount = (int)((Height - mHeaderTextHeight - mHeaderRowPadding * 2 -
                    mHeaderMarginBottom) / mHourHeight) + 1;
            lineCount = (lineCount) * (mNumberOfVisibleDays + 1);
            float[] hourLines = new float[lineCount * 4];

            // Clear the cache for event rectangles.
            if (mEventRects != null)
            {

                foreach (var mEventRect in mEventRects)
                {
                    mEventRect.rectF = null;
                }
            }

            // Iterate through each day.
            Calendar oldFirstVisibleDay = mFirstVisibleDay;
            mFirstVisibleDay = (Calendar)mToday.Clone();
            mFirstVisibleDay.Add(Calendar.Date, leftDaysWithGaps);
            if (!mFirstVisibleDay.Equals(oldFirstVisibleDay) && mScrollListener != null)
            {
                mScrollListener.onFirstVisibleDayChanged(mFirstVisibleDay, oldFirstVisibleDay);
            }
            for (int dayNumber = leftDaysWithGaps + 1;
                 dayNumber <= leftDaysWithGaps + mNumberOfVisibleDays + 1;
                 dayNumber++)
            {

                // Check if the day is today.
                day = (Calendar)mToday.Clone();
                mLastVisibleDay = (Calendar)day.Clone();
                day.Add(Calendar.Date, dayNumber - 1);
                mLastVisibleDay.Add(Calendar.Date, dayNumber - 2);
                bool sameDay = isSameDay(day, mToday);

                // Get more events if necessary. We want to store the events 3 months beforehand. Get
                // events only when it is the first iteration of the loop.
                if (mEventRects == null || mRefreshEvents || (dayNumber == leftDaysWithGaps + 1 && mFetchedMonths[1] != day.Get(Calendar.Month) + 1 && day.Get(Calendar.DayOfMonth) == 15))
                {
                    getMoreEvents(day);
                    mRefreshEvents = false;
                }

                // Draw backgRound color for each day.
                float start = (startPixel < mHeaderColumnWidth ? mHeaderColumnWidth : startPixel);
                if (mWidthPerDay + startPixel - start > 0)
                    canvas.DrawRect(start, mHeaderTextHeight + mHeaderRowPadding * 2 + mTimeTextHeight / 2 + mHeaderMarginBottom, startPixel + mWidthPerDay, Height, sameDay ? mTodayBackgroundPaint : mDayBackgroundPaint);

                // Prepare the separator lines for hours.
                int i = 0;
                for (int hourNumber = 0; hourNumber < 24; hourNumber++)
                {
                    float top = mHeaderTextHeight + mHeaderRowPadding * 2 + mCurrentOrigin.Y + mHourHeight * hourNumber + mTimeTextHeight / 2 + mHeaderMarginBottom;
                    if (top > mHeaderTextHeight + mHeaderRowPadding * 2 + mTimeTextHeight / 2 + mHeaderMarginBottom - mHourSeparatorHeight && top < Height && startPixel + mWidthPerDay - start > 0)
                    {
                        hourLines[i * 4] = start;
                        hourLines[i * 4 + 1] = top;
                        hourLines[i * 4 + 2] = startPixel + mWidthPerDay;
                        hourLines[i * 4 + 3] = top;
                        i++;
                    }
                }

                // Draw the lines for hours.
                canvas.DrawLines(hourLines, mHourSeparatorPaint);

                // Draw the events.
                drawEvents(day, startPixel, canvas);

                // In the next iteration, start from the next day.
                startPixel += mWidthPerDay + mColumnGap;
            }

            // Draw the header backgRound.
            canvas.DrawRect(0, 0, Width, mHeaderTextHeight + mHeaderRowPadding * 2, mHeaderBackgroundPaint);

            // Draw the header row texts.
            startPixel = startFromPixel;
            for (int dayNumber = leftDaysWithGaps + 1; dayNumber <= leftDaysWithGaps + mNumberOfVisibleDays + 1; dayNumber++)
            {
                // Check if the day is today.
                day = (Calendar)mToday.Clone();
                day.Add(Calendar.Date, dayNumber - 1);
                bool sameDay = isSameDay(day, mToday);

                // Draw the day labels.
                var DateTimeInter = getDateTimeInterpreter();

                string dayLabel = DateTimeInter.interpretDate(day, Language);
                if (dayLabel == null)
                    throw new IllegalStateException("A DateTimeInterpreter must not return null date");
                canvas.DrawText(dayLabel, startPixel + mWidthPerDay / 2, mHeaderTextHeight + mHeaderRowPadding, sameDay ? mTodayHeaderTextPaint : mHeaderTextPaint);
                startPixel += mWidthPerDay + mColumnGap;
            }

        }




        private Calendar getTimeFromPoint(float x, float y)
        {
            int leftDaysWithGaps = (int)-(System.Math.Ceiling(mCurrentOrigin.X / (mWidthPerDay + mColumnGap)));
            float startPixel = mCurrentOrigin.X + (mWidthPerDay + mColumnGap) * leftDaysWithGaps +
                    mHeaderColumnWidth;
            for (int dayNumber = leftDaysWithGaps + 1;
                 dayNumber <= leftDaysWithGaps + mNumberOfVisibleDays + 1;
                 dayNumber++)
            {
                float start = (startPixel < mHeaderColumnWidth ? mHeaderColumnWidth : startPixel);
                if (mWidthPerDay + startPixel - start > 0
                        && x > start && x < startPixel + mWidthPerDay)
                {
                    Calendar day = (Calendar)mToday.Clone();
                    day.Add(Calendar.Date, dayNumber - 1);
                    float pixelsFromZero = y - mCurrentOrigin.Y - mHeaderTextHeight
                            - mHeaderRowPadding * 2 - mTimeTextHeight / 2 - mHeaderMarginBottom;
                    int hour = (int)(pixelsFromZero / mHourHeight);
                    int Minute = (int)(60 * (pixelsFromZero - hour * mHourHeight) / mHourHeight);
                    day.Add(Calendar.Hour, hour);
                    day.Set(Calendar.Minute, Minute);
                    return day;
                }
                startPixel += mWidthPerDay + mColumnGap;
            }
            return null;
        }


        private void drawEvents(Calendar date, float startFromPixel, Canvas canvas)
        {
            if (mEventRects != null && mEventRects.Count > 0)
            {
                for (int i = 0; i < mEventRects.Count; i++)
                {
                    if (isSameDay(mEventRects[i].mEvent.getStartTime(), date))
                    {

                        // Calculate top.
                        float top = mHourHeight * 24 * mEventRects[i].top / 1440 + mCurrentOrigin.Y + mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom + mTimeTextHeight / 2 + mEventMarginVertical;
                        float originalTop = top;
                        if (top < mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom + mTimeTextHeight / 2)
                            top = mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom + mTimeTextHeight / 2;

                        // Calculate bottom.
                        float bottom = mEventRects[i].bottom;
                        bottom = mHourHeight * 24 * bottom / 1440 + mCurrentOrigin.Y + mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom + mTimeTextHeight / 2 - mEventMarginVertical;

                        // Calculate left and right.
                        float left = startFromPixel + mEventRects[i].left * mWidthPerDay;
                        if (left < startFromPixel)
                            left += mOverlappingEventGap;
                        float originalLeft = left;
                        float right = left + mEventRects[i].width * mWidthPerDay;
                        if (right < startFromPixel + mWidthPerDay)
                            right -= mOverlappingEventGap;
                        if (left < mHeaderColumnWidth) left = mHeaderColumnWidth;

                        // Draw the event and the event name on top of it.
                        RectF eventRectF = new RectF(left, top, right, bottom);
                        if (bottom > mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom + mTimeTextHeight / 2 && left < right &&
                                eventRectF.Right > mHeaderColumnWidth &&
                                eventRectF.Left < Width &&
                                eventRectF.Bottom > mHeaderTextHeight + mHeaderRowPadding * 2 + mTimeTextHeight / 2 + mHeaderMarginBottom &&
                                eventRectF.Top < Height &&
                                left < right
                                )
                        {
                            mEventRects[i].rectF = eventRectF;
                            mEventBackgroundPaint.Color = (mEventRects[i].mEvent.getColor() == 0 ? mDefaultEventColor : mEventRects[i].mEvent.getColor());
                            canvas.DrawRect(mEventRects[(i)].rectF, mEventBackgroundPaint);
                            var value1 = mEventRects[(i)].mEvent.getName();
                            var value2 = mEventRects[(i)].rectF;

                            drawText(value1, value2, canvas, originalTop, originalLeft);
                        }
                        else
                            mEventRects[(i)].rectF = null;
                    }
                }
            }
        }




        private void drawText(string text, RectF rect, Canvas canvas, float originalTop, float originalLeft)
        {
            if (rect.Right - rect.Left - mEventPadding * 2 < 0) return;

            // Get text dimensions
            StaticLayout textLayout = new StaticLayout(text, mEventTextPaint, (int)(rect.Right - originalLeft - mEventPadding * 2), Android.Text.Layout.Alignment.AlignNormal, 1.0f, 0.0f, false);

            // Crop height
            int availableHeight = (int)(rect.Bottom - originalTop - mEventPadding * 2);
            int lineHeight = textLayout.Height / textLayout.LineCount;
            if (lineHeight < availableHeight && textLayout.Height > rect.Height() - mEventPadding * 2)
            {
                int lineCount = textLayout.LineCount;
                int availableLineCount = (int)System.Math.Floor((double)(lineCount * availableHeight / textLayout.Height));
                float widthAvailable = (rect.Right - originalLeft - mEventPadding * 2) * availableLineCount;
                try
                {
                    // textLayout = new StaticLayout(TextUtils.ellipsize(text, mEventTextPaint, widthAvailable, TextUtils.TruncateAt.END), mEventTextPaint, (int)(rect.right - originalLeft - mEventPadding * 2), Layout.Alignment.ALIGN_NORMAL, 1.0f, 0.0f, false);

                    //todo: Check it's make ??????????????????????????????????

                    ///// - -------------------------------------------------- Draw Title---------------------------------------------------------------------//
                    //var TitleEventTextPaint = mEventTextPaint;
                    //TitleEventTextPaint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Px,
                    //    Resource.Dimension.textMediumLarge, Resources.DisplayMetrics);
                    var mTextUtils = TextUtils.Ellipsize(text, mEventTextPaint, widthAvailable, TextUtils.TruncateAt.End);

                    textLayout = new StaticLayout(text, mEventTextPaint, (int)(rect.Right - originalLeft - mEventPadding * 2), Android.Text.Layout.Alignment.AlignNormal, 1.0f, 0.0f, false);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }




            }
            else if (lineHeight >= availableHeight)
            {
                try
                {


                    int width = (int)(rect.Right - originalLeft - mEventPadding * 2);
                    var mTextUtils = TextUtils.Ellipsize(text, mEventTextPaint, width, TextUtils.TruncateAt.End);
                    textLayout = new StaticLayout(text, mEventTextPaint, width, Android.Text.Layout.Alignment.AlignNormal, 1.0f, 1.0f, false);
                }
                catch (Exception ex)
                {

                }
            }

            // Draw text
            canvas.Save();
            canvas.Translate(originalLeft + mEventPadding, originalTop + mEventPadding);
            textLayout.Draw(canvas);
            canvas.Restore();
        }


        private void getMoreEvents(Calendar day)
        {

            // Delete all events if its not current month +- 1.
            deleteFarMonths(day);

            // Get more events if the month is changed.
            if (mEventRects == null)
                mEventRects = new List<EventRect>();
            if (mMonthChangeListener == null && !IsInEditMode)
                throw new IllegalStateException("You must provide a MonthChangeListener");

            // If a refresh was requested then reset some variables.
            if (mRefreshEvents)
            {
                mEventRects.Clear();
                mFetchedMonths = new int[3];
            }

            // Get events of previous month.
            int previousMonth = (day.Get(Calendar.Month) == 0 ? 12 : day.Get(Calendar.Month));
            int nextMonth = (day.Get(Calendar.Month) + 2 == 13 ? 1 : day.Get(Calendar.Month) + 2);
            int[] lastFetchedMonth = (int[])mFetchedMonths.Clone();
            if (mFetchedMonths[0] < 1 || mFetchedMonths[0] != previousMonth || mRefreshEvents)
            {
                if (!containsValue(lastFetchedMonth, previousMonth) && !IsInEditMode)
                {
                    List<WeekViewEvent> events = mMonthChangeListener.onMonthChange((previousMonth == 12) ? day.Get(Calendar.Year) - 1 : day.Get(Calendar.Year), previousMonth);
                    sortEvents(events);

                    foreach (var weekViewEvent in events)
                    {
                        cacheEvent(weekViewEvent);
                    }
                }
                mFetchedMonths[0] = previousMonth;
            }

            // Get events of this month.
            if (mFetchedMonths[1] < 1 || mFetchedMonths[1] != day.Get(Calendar.Month) + 1 || mRefreshEvents)
            {
                if (!containsValue(lastFetchedMonth, day.Get(Calendar.Month) + 1) && !IsInEditMode)
                {
                    List<WeekViewEvent> events = mMonthChangeListener.onMonthChange(day.Get(Calendar.Year), day.Get(Calendar.Month) + 1);
                    sortEvents(events);

                    foreach (var weekViewEvent in events)
                    {
                        cacheEvent(weekViewEvent);
                    }
                }
                mFetchedMonths[1] = day.Get(Calendar.Month) + 1;
            }

            // Get events of next month.
            if (mFetchedMonths[2] < 1 || mFetchedMonths[2] != nextMonth || mRefreshEvents)
            {
                if (!containsValue(lastFetchedMonth, nextMonth) && !IsInEditMode)
                {
                    List<WeekViewEvent> events = mMonthChangeListener.onMonthChange(nextMonth == 1 ? day.Get(Calendar.Year) + 1 : day.Get(Calendar.Year), nextMonth);
                    sortEvents(events);

                    foreach (var weekViewEvent in events)
                    {
                        cacheEvent(weekViewEvent);
                    }
                }
                mFetchedMonths[2] = nextMonth;
            }

            // Prepare to calculate positions of each events.
            List<EventRect> tempEvents = new List<EventRect>(mEventRects);
            mEventRects = new List<EventRect>();
            Calendar dayCounter = (Calendar)day.Clone();
            dayCounter.Add(Calendar.Month, -1);
            dayCounter.Set(Calendar.DayOfMonth, 1);
            Calendar maxDay = (Calendar)day.Clone();
            maxDay.Add(Calendar.Month, 1);
            maxDay.Set(Calendar.DayOfMonth, maxDay.GetActualMaximum(Calendar.DayOfMonth));

            // Iterate through each day to calculate the position of the events.
            while (dayCounter.TimeInMillis <= maxDay.TimeInMillis)
            {
                List<EventRect> eventRects = new List<EventRect>();
                //for (EventRect eventRect : tempEvents) {
                //    if (isSameDay(eventRect.event.getStartTime(), dayCounter))
                //        eventRects.Add(eventRect);
                //}
                foreach (var tempEvent in tempEvents)
                {
                    if (isSameDay(tempEvent.mEvent.getStartTime(), dayCounter))
                    {
                        eventRects.Add(tempEvent);
                    }
                }

                computePositionOfEvents(eventRects);
                dayCounter.Add(Calendar.Date, 1);
            }
        }



        private void cacheEvent(WeekViewEvent Event)
        {
            if (!isSameDay(Event.getStartTime(), Event.getEndTime()))
            {
                Calendar endTime = (Calendar)Event.getStartTime().Clone();
                endTime.Set(Calendar.HourOfDay, 23);
                endTime.Set(Calendar.Minute, 59);
                Calendar startTime = (Calendar)Event.getEndTime().Clone();
                startTime.Set(Calendar.HourOfDay, 0);
                startTime.Set(Calendar.Minute, 0);
                WeekViewEvent event1 = new WeekViewEvent(Event.getId(), Event.getName(), Event.getStartTime(), endTime);
                event1.setColor(Event.getColor());
                WeekViewEvent event2 = new WeekViewEvent(Event.getId(), Event.getName(), startTime, Event.getEndTime());
                event2.setColor(Event.getColor());
                mEventRects.Add(new EventRect(event1, Event, null));
                mEventRects.Add(new EventRect(event2, Event, null));
            }
            else
            {
                mEventRects.Add(new EventRect(Event, Event, null));
            }
        }

        public void sortEvents(List<WeekViewEvent> events)
        {
            events.Sort((event1, event2) =>
            {
                long start1 = event1.getStartTime().TimeInMillis;
                long start2 = event2.getStartTime().TimeInMillis;
                int comparator = start1 > start2 ? 1 : (start1 < start2 ? -1 : 0);
                if (comparator == 0)
                {
                    long end1 = event1.getEndTime().TimeInMillis;
                    long end2 = event2.getEndTime().TimeInMillis;
                    comparator = end1 > end2 ? 1 : (end1 < end2 ? -1 : 0);
                }
                return comparator;
            });
        }

        private void computePositionOfEvents(List<EventRect> eventRects)
        {
            //    List<List<EventRect>> collisionGroups = new ArrayList<List<EventRect>>();
            //for (EventRect eventRect : eventRects) {
            //    boolean isPlaced = false;
            //    outerLoop:
            //    for (List<EventRect> collisionGroup : collisionGroups) {
            //        for (EventRect groupEvent : collisionGroup) {
            //            if (isEventsCollide(groupEvent.event, eventRect.event)) {
            //                collisionGroup.add(eventRect);
            //                isPlaced = true;
            //                break outerLoop;
            //            }
            //        }
            //    }
            //    if (!isPlaced) {
            //        List<EventRect> newGroup = new ArrayList<EventRect>();
            //        newGroup.add(eventRect);
            //        collisionGroups.add(newGroup);
            //    }
            //}

            //for (List<EventRect> collisionGroup : collisionGroups) {
            //    expandEventsToMaxWidth(collisionGroup);
            //}

            List<List<EventRect>> collisionGroups = new List<List<EventRect>>();

            foreach (var eventRect in eventRects)
            {
                bool isPlaced = false;
            outerLoop:
                foreach (var collisionGroup in collisionGroups)
                {
                    foreach (var rect in collisionGroup)
                    {
                        if (isEventsCollide(rect.mEvent, eventRect.mEvent))
                        {
                            collisionGroup.Add(eventRect);
                            isPlaced = true;

                            goto breakouterLoop;

                        }
                    }
                }
                if (!isPlaced)
                {
                    List<EventRect> newGroup = new List<EventRect>();
                    newGroup.Add(eventRect);
                    collisionGroups.Add(newGroup);
                }
            breakouterLoop:
                Console.Write("Goto");
            }


            foreach (var collisionGroup in collisionGroups)
            {
                expandEventsToMaxWidth(collisionGroup);
            }
        }



        private void expandEventsToMaxWidth(List<EventRect> collisionGroup)
        {
            // Expand the events to maximum possible width.
            List<List<EventRect>> columns = new List<List<EventRect>>();
            columns.Add(new List<EventRect>());

            foreach (var eventRect in collisionGroup)
            {
                bool isPlaced = false;

                foreach (var column in columns)
                {
                    if (column.Count == 0)
                    {
                        column.Add(eventRect);
                        isPlaced = true;
                    }
                    else if (!isEventsCollide(eventRect.mEvent, column[column.Count - 1].mEvent))
                    {
                        column.Add(eventRect);
                        isPlaced = true;
                        break;
                    }
                }
                if (!isPlaced)
                {
                    List<EventRect> newColumn = new List<EventRect>();
                    newColumn.Add(eventRect);
                    columns.Add(newColumn);
                }
            }


            //Calculate left and right position for all the events.
            //Get the maxRowCount by looking in all columns.
            //int maxRowCount = 0;
            //for (List<EventRect> column : columns){
            //    maxRowCount = Math.max(maxRowCount, column.size());
            //}
            //for (int i = 0; i < maxRowCount; i++) {
            //    // Set the left and right values of the event.
            //    float j = 0;
            //    for (List<EventRect> column : columns) {
            //        if (column.size() >= i+1) {
            //            EventRect eventRect = column.get(i);
            //            eventRect.width = 1f / columns.size();
            //            eventRect.left = j / columns.size();
            //            eventRect.top = eventRect.event.getStartTime().get(Calendar.HourOfDay) * 60 + eventRect.event.getStartTime().get(Calendar.Minute);
            //            eventRect.bottom = eventRect.event.getEndTime().get(Calendar.HourOfDay) * 60 + eventRect.event.getEndTime().get(Calendar.Minute);
            //            mEventRects.Add(eventRect);
            //        }
            //        j++;
            //    }
            //}

            int maxRowCount = 0;
            foreach (var column in columns)
            {
                maxRowCount = System.Math.Max(maxRowCount, column.Count);
            }
            for (int i = 0; i < maxRowCount; i++)
            {
                // Set the left and right values of the event.
                float j = 0;
                //foreach (var column : columns) {
                ////    if (column.size() >= i+1) {
                ////        EventRect eventRect = column.get(i);
                ////        eventRect.width = 1f / columns.size();
                ////        eventRect.left = j / columns.size();
                ////        eventRect.top = eventRect.event.getStartTime().get(Calendar.HourOfDay) * 60 + eventRect.event.getStartTime().get(Calendar.Minute);
                ////        eventRect.bottom = eventRect.event.getEndTime().get(Calendar.HourOfDay) * 60 + eventRect.event.getEndTime().get(Calendar.Minute);
                ////        mEventRects.Add(eventRect);
                //   }

                foreach (var column in columns)
                {
                    if (column.Count >= i + 1)
                    {
                        EventRect eventRect = column[i];
                        eventRect.width = 1f / columns.Count;
                        eventRect.left = j / columns.Count;
                        eventRect.top = eventRect.mEvent.getStartTime().Get(Calendar.HourOfDay) * 60 +
                                        eventRect.mEvent.getStartTime().Get(Calendar.Minute);
                        eventRect.bottom = eventRect.mEvent.getEndTime().Get(Calendar.HourOfDay) * 60 +
                                           eventRect.mEvent.getEndTime().Get(Calendar.Minute);
                        mEventRects.Add(eventRect);
                    }



                    j++;
                }

            }
        }




        private
            bool isEventsCollide(WeekViewEvent event1, WeekViewEvent event2)
        {
            long start1 = event1.getStartTime().TimeInMillis;
            long end1 = event1.getEndTime().TimeInMillis;
            long start2 = event2.getStartTime().TimeInMillis;
            long end2 = event2.getEndTime().TimeInMillis;
            return !((start1 >= end2) || (end1 <= start2));
        }

        private bool isTimeAfterOrEquals(Calendar time1, Calendar time2)
        {
            return !(time1 == null || time2 == null) && time1.TimeInMillis >= time2.TimeInMillis;
        }

        private void deleteFarMonths(Calendar currentDay)
        {

            if (mEventRects == null) return;

            Calendar nextMonth = (Calendar)currentDay.Clone();
            nextMonth.Add(Calendar.Month, 1);
            nextMonth.Set(Calendar.DayOfMonth, nextMonth.GetActualMaximum(Calendar.DayOfMonth));
            nextMonth.Set(Calendar.HourOfDay, 12);
            nextMonth.Set(Calendar.Minute, 59);
            nextMonth.Set(Calendar.Second, 59);

            Calendar prevMonth = (Calendar)currentDay.Clone();
            prevMonth.Add(Calendar.Month, -1);
            prevMonth.Set(Calendar.DayOfMonth, 1);
            prevMonth.Set(Calendar.HourOfDay, 0);
            prevMonth.Set(Calendar.Minute, 0);
            prevMonth.Set(Calendar.Second, 0);

            List<EventRect> newEvents = new List<EventRect>();

            //for (EventRect eventRect : mEventRects) {
            //    bool isFarMonth = eventRect.event.getStartTime().TimeInMillis > nextMonth.TimeInMillis || eventRect.event.getEndTime().TimeInMillis < prevMonth.TimeInMillis;
            //    if (!isFarMonth) newEvents.Add(eventRect);
            //}

            foreach (var mEventRect in mEventRects)
            {
                bool isFarMonth = mEventRect.mEvent.getStartTime().TimeInMillis > nextMonth.TimeInMillis || mEventRect.mEvent.getEndTime().TimeInMillis < prevMonth.TimeInMillis;
                if (!isFarMonth) newEvents.Add(mEventRect);
            }

            mEventRects.Clear();
            //mEventRects.AddAll(newEvents);
            for (int i = 0; i < newEvents.Count; i++)
            {
                mEventRects.Add(newEvents[i]);
            }
        }

        public void goToToday()
        {
            Calendar today = Calendar.Instance;
            goToDate(today);
        }


        public void goToDate(Calendar date)
        {
            mScroller.ForceFinished(true);
            date.Set(Calendar.HourOfDay, 0);
            date.Set(Calendar.Minute, 0);
            date.Set(Calendar.Second, 0);
            date.Set(Calendar.Millisecond, 0);

            if (mAreDimensionsInvalid)
            {
                mScrollToDay = date;
                return;
            }

            mRefreshEvents = true;

            Calendar today = Calendar.Instance;
            today.Set(Calendar.HourOfDay, 0);
            today.Set(Calendar.Minute, 0);
            today.Set(Calendar.Second, 0);
            today.Set(Calendar.Millisecond, 0);

            long dateInMillis = date.TimeInMillis + date.TimeZone.GetOffset(date.TimeInMillis);
            long todayInMillis = today.TimeInMillis + today.TimeZone.GetOffset(today.TimeInMillis);
            int dateDifference = (int)((dateInMillis - todayInMillis) / (1000 * 60 * 60 * 24));

            mCurrentOrigin.X = -dateDifference * (mWidthPerDay + mColumnGap);
            // mStickyScroller.startScroll((int) mCurrentOrigin.X, 0, (int) (-dateDifference*(mWidthPerDay + mColumnGap)-mCurrentOrigin.X), 0);
            Invalidate();
        }



        public void notifyDatasetChanged()
        {
            mRefreshEvents = true;
            Invalidate();
        }

        public void goToHour(double hour)
        {
            if (mAreDimensionsInvalid)
            {
                mScrollToHour = hour;
                return;
            }

            int verticalOffset = 0;
            if (hour > 24)
                verticalOffset = mHourHeight * 24;
            else if (hour > 0)
                verticalOffset = (int)(mHourHeight * hour);

            if (verticalOffset > mHourHeight * 24 - Height + mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom)
                verticalOffset = (int)(mHourHeight * 24 - Height + mHeaderTextHeight + mHeaderRowPadding * 2 + mHeaderMarginBottom);

            mCurrentOrigin.Y = -verticalOffset;
            Invalidate();
        }



        public double getFirstVisibleHour()
        {
            return -mCurrentOrigin.Y / mHourHeight;
        }

        #endregion

        #region Interfaces


        public interface EventClickListener
        {
            void onEventClick(WeekViewEvent mEvent, RectF eventRect);
        }

        public interface MonthChangeListener
        {
            List<WeekViewEvent> onMonthChange(int newYear, int newMonth);
        }

        public interface EventLongPressListener
        {
            void onEventLongPress(WeekViewEvent mEvent, RectF eventRect);
        }

        public interface EmptyViewClickListener
        {
            void onEmptyViewClicked(Calendar time);
        }

        public interface EmptyViewLongPressListener
        {
            void onEmptyViewLongPress(Calendar time);
        }

        public interface ScrollListener
        {
            /**
             * Called when the first visible day has changed.
             *
             * (this will also be called during the first draw of the weekview)
             * @param newFirstVisibleDay The new first visible day
             * @param oldFirstVisibleDay The old first visible day (is null on the first call).
             */
            void onFirstVisibleDayChanged(Calendar newFirstVisibleDay, Calendar oldFirstVisibleDay);
        }


        #endregion

        #region Helper Methods


        private bool containsValue(int[] list, int value)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i] == value)
                    return true;
            }
            return false;
        }




        private bool isSameDay(Calendar dayOne, Calendar dayTwo)
        {
            return dayOne.Get(Calendar.Year) == dayTwo.Get(Calendar.Year) && dayOne.Get(Calendar.DayOfYear) == dayTwo.Get(Calendar.DayOfYear);
        }

        #endregion

    }


    #region Class New

    #region NewDateTimeInterpreter


    public class NewDateTimeInterpreter : DateTimeInterpreter
    {
        private int mDayNameLength;
        private int LENGTH_SHORT;

        public NewDateTimeInterpreter(int DayNameLength, int mLENGTH_SHORT)
        {
            mDayNameLength = DayNameLength;
            LENGTH_SHORT = mLENGTH_SHORT;
        }

        public string interpretDate(Calendar date, int Language)
        {
            SimpleDateFormat sdf;
            sdf = mDayNameLength == LENGTH_SHORT ? new SimpleDateFormat("EEEEE") : new SimpleDateFormat("EEE");
            try
            {
                string dayName = sdf.Format(date.Time).ToUpper();
                //return string.Format("%s %d/%02d", dayName, date.Get(Calendar.Month) + 1, date.Get(Calendar.DayOfMonth));
                string SunMon = string.Empty;
                switch (dayName)
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

                return string.Format("%s %d / %02d", SunMon + "abc", date.Get(Calendar.Month) + 1, date.Get(Calendar.DayOfMonth));


            }
            catch (System.Exception e)
            {
                return "";
            }
        }

        public string interpretTime(int hour)
        {
            string amPm;
            if (hour >= 0 && hour < 12) amPm = "AM";
            else amPm = "PM";
            if (hour == 0) hour = 12;
            if (hour > 12) hour -= 12;
            return string.Format("%02d %s", hour, amPm);
        }
    }
    #endregion

    #region EventRect

    public class EventRect
    {
        public WeekViewEvent mEvent;
        public WeekViewEvent originalEvent;
        public RectF rectF;
        public float left;
        public float width;
        public float top;
        public float bottom;

        public EventRect(WeekViewEvent Event, WeekViewEvent originalEvent, RectF rectF)
        {
            this.mEvent = Event;
            this.rectF = rectF;
            this.originalEvent = originalEvent;
        }

    }

    #endregion




    #endregion
}