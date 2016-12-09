//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.Content.PM;
//using Android.Graphics;
//using Android.Graphics.Drawables;
//using Android.OS;
//using Android.Runtime;
//using Android.Support.V4.App;
//using Android.Support.V4.View;
//using Android.Views;
//using Android.Widget;
//using Cirrious.CrossCore;
//using Cirrious.MvvmCross.Droid.Fragging.Fragments;
//using Cirrious.MvvmCross.Plugins.Messenger;
//using FlexyPark.Core;
//using FlexyPark.Core.Messengers;
//using FlexyPark.Core.Services;
//using FlexyPark.Core.ViewModels;
//using FlexyPark.UI.Droid.Controls;
//using FlexyPark.UI.Droid.Services;
//using Google.Apis.Authentication.OAuth2;
//using Google.Apis.Calendar.v3.Data;
//using Google.Apis.Util;
//using MonoDroid.TimesSquare;
//using CalendarService = Google.Apis.Calendar.v3.CalendarService;

//namespace FlexyPark.UI.Droid.Activity
//{

//    [Activity(Label = "AddSpotCalendarView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
//    public class AddSpotCalendarView : BaseView, IAddSpotCalendarView
//    {
//        //#region UI Controls

//        //private TextNeueBold tvEdit;
//        //private Dialog mDialog;
//        //private MvxDialogFragment dialog;
//        //private ViewPager viewPager;
//        //private ImageView CalendarNext, CalendarBack;

//        //#endregion

//        //#region Variables

//        //private GoogleAuthenticator mAuthenticator;
//        //private CalendarService mCalendarService;

//        //private const string TAG = "FlexyPark.UI.Droid";

//        //#endregion

//        //#region Constructors

//        //public new AddSpotCalendarViewModel ViewModel
//        //{
//        //    get { return base.ViewModel as AddSpotCalendarViewModel; }
//        //    set
//        //    {
//        //        base.ViewModel = value;
//        //    }
//        //}


//        #endregion

//        #region Overrides

//        protected override void OnViewModelSet()
//        {
//            base.OnViewModelSet();
//            ViewModel.View = this;
//        }

//        protected override void OnResume()
//        {
//            base.OnResume();

//            //if (Mvx.Resolve<ICacheService>().NeedReloadEvent)
//            //{
//            //    //reload event here
//            //    ViewModel.GetEvents();
//            //    Mvx.Resolve<ICacheService>().NeedReloadEvent = false;
//            //}

//            //if (ViewModel.NeedToClose)
//            //{
//            //    ViewModel.NeedToClose = false;
//            //    Finish();
//            //}
//        }

//        protected override void OnCreate(Bundle bundle)
//        {
//            base.OnCreate(bundle);
            
//            SetContentView(Resource.Layout.AddSpotCalendarView);
//            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true, null));
//            Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel = ViewModel;
//            tvEdit = FindViewById<TextNeueBold>(Resource.Id.tvEdit);
//            CalendarBack = FindViewById<ImageView>(Resource.Id.ivCalendarBack);
//            CalendarNext = FindViewById<ImageView>(Resource.Id.ivCalendarNext);
//            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);

//            CalendarBack.Click += (sender, args) =>
//            {
//                int index = viewPager.CurrentItem - 1;
//                viewPager.SetCurrentItem(--viewPager.CurrentItem, false);
//            };

//            CalendarNext.Click += (sender, args) =>
//            {
//                int index = viewPager.CurrentItem + 1;
//                viewPager.SetCurrentItem(++viewPager.CurrentItem, false);
//            };
//            SetButtonEffects(new List<int>()
//            {
//                Resource.Id.ivCalendarBack,
//                Resource.Id.ivCalendarNext
//            });
//            //if (mAuthenticator == null)
//            //{
//            //    mAuthenticator = new GoogleAuthenticator(AppConstants.ClientID, new Uri(AppConstants.RedirectUrl), CalendarService.Scopes.Calendar.GetStringValue());
//            //    mAuthenticator.AllowCancel = true;
//            //    mAuthenticator.Completed += mAuthenticator_Completed;
//            //}

//            //StartActivity(mAuthenticator.GetUI(this));





//            //calendar.OnDateSelected +=
//            //    (s, e) =>
//            //    {
//            //        //Toast.MakeText(this, e.SelectedDate.ToShortDateString(), ToastLength.Short).Show();

//            //        for (int i = 0; i < ViewModel.Events.Count; i++)
//            //        {
//            //            if (ViewModel.Events[i].StartDate.DayOfYear == e.SelectedDate.DayOfYear)
//            //            {
//            //                dialog = new EventDialog() { ViewModel = this.ViewModel };
//            //                dialog.Show(SupportFragmentManager, "EventDialog");

//            //                break;
//            //            }
//            //        }



//            //    };





//            // calendar.Init(DateTime.Now, nextYear)
//            //.InMode(CalendarPickerView.SelectionMode.Single)
//            //.WithSelectedDate(DateTime.Now);




//            //btnDialog.Click += (s, e) =>
//            //{
//            //    var dialogView =
//            //        (CalendarPickerView)LayoutInflater.Inflate(Resource.Layout.dialog, null, false);
//            //    dialogView.Init(DateTime.Now, nextYear);
//            //    new AlertDialog.Builder(this).SetTitle("I'm a dialog!")
//            //        .SetView(dialogView)
//            //        .SetNeutralButton("Dismiss",
//            //            (sender, args) => { }).Create().Show();
//            //};

//            //FindViewById<Button>(Resource.Id.done_button).Click += (s, o) =>
//            //{
//            //    calendar = FindViewById<CalendarPickerView>(Resource.Id.calendar_view);
//            //    Logr.D(TAG, "Selected time in millis: " + calendar.SelectedDate);
//            //    string toast = "Selected: " + calendar.SelectedDate;
//            //    Toast.MakeText(this, toast, ToastLength.Short).Show();
//            //};
//        }

//        void mAuthenticator_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
//        {
//            if (e.IsAuthenticated)
//            {
//                mCalendarService = new CalendarService(mAuthenticator);
//                Mvx.Resolve<ICalendarService>().CachedCalendarService = mCalendarService;
//                ViewModel.GetEvents();
//            }
//            else
//                ViewModel.NeedToClose = true;
//        }

//        #endregion

//        #region Implements

//        public void UpdateCalendar()
//        {
//            #region Init ViewPager

//            viewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);
//            viewPager.SetCurrentItem(100, false);
//            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false, null));


//            #endregion
//        }

//        #endregion

//        #region Methods

//        private void OnCalendarListResponse(LazyResult<CalendarList> result)
//        {
//            CalendarList list = null;
//            try
//            {
//                list = result.GetResult();
//            }
//            catch (Exception e)
//            {
//                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
//                Console.WriteLine(e.Message);
//                return;
//            }

//            if (list != null && list.Items.Count > 0)
//            {
//                ViewModel.CalendarId = list.Items[0].Id;
//                mCalendarService.Events.List(list.Items[0].Id).FetchAsync(OnEventListResponse);
//            }

//            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
//        }

//        private void OnEventListResponse(LazyResult<Events> _events)
//        {
//            //Events events = null;
//            //try
//            //{
//            //    events = _events.GetResult();
//            //}
//            //catch (Exception e)
//            //{
//            //    Console.WriteLine(e.Message);
//            //    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
//            //    return;
//            //}

//            //if (events != null && events.Items.Count > 0)
//            //{
//            //    //select event that start date is larger than now
//            //    var filterList = events.Items.Where(x => !string.IsNullOrEmpty(x.Start.DateTime) && (DateTime.Parse(x.Start.DateTime).ToUniversalTime()).CompareTo(DateTime.UtcNow) >= 0).ToList();

//            //    if (filterList != null && filterList.Count == 0)
//            //    {
//            //        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this.ViewModel, "Sorry ! There is no event that will happen from now !"));
//            //        if (ViewModel.IsEditMode)
//            //            ViewModel.SwitchModeCommand.Execute();

//            //    }
//            //    else
//            //    {
//            //        RunOnUiThread(() =>
//            //        {
//            //            //mEvents.Clear();
//            //            ViewModel.Events.Clear();
//            //        });
//            //    }

//            //    foreach (var item in filterList)
//            //    {
//            //        RunOnUiThread(() =>
//            //        {
//            //            ViewModel.Events.Add(new EventItemViewModel(this.ViewModel)
//            //            {
//            //                IsEditMode = ViewModel.IsEditMode,
//            //                CalendarID = ViewModel.CalendarId,
//            //                EndDate = item.End.DateTime,
//            //                StartDate = item.Start.DateTime,
//            //                EventId = item.Id,
//            //                Summary = item.Summary
//            //            });
//            //        });

//            //    }

//            //    //reload event list on UI
//            //}
//            //else
//            //{
//            //    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this.ViewModel, "Sorry ! There is no event from Google Calendar!"));
//            //}

//            //Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
//        }



//        public void SetModeTitle(string title)
//        {
//            //RunOnUiThread(() =>
//            //{
//            //    tvEdit.Text = title;
//            //});
//            //throw new NotImplementedException();

//        }

//        public void ReloadTable()
//        {
//            //throw new NotImplementedException();
//        }

//        public void GetEvents()
//        {
//            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, true));
//            mCalendarService.CalendarList.List().FetchAsync(OnCalendarListResponse);
//        }

//        public void DeleteEvent(string eventId)
//        {
//            // Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, true));
//            mCalendarService.Events.Delete(ViewModel.CalendarId, eventId).FetchAsync(OnDeleteResponse);
//            ViewModel.GetEvents();


//        }

//        public void ClosePopup()
//        {
//            if (dialog != null)
//            {
//                dialog.Dismiss();
//            }
//        }

      

//        private void OnDeleteResponse(LazyResult<string> response)
//        {
//            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));
//            Console.WriteLine(response.GetResult());
//        }

//        #region Adpater ViewPager

//        private class ViewPagerAdapter : FragmentPagerAdapter
//        {
//            private int MaxCount = 200;


//            public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm)
//                : base(fm)
//            {

//            }

//            public override int Count
//            {
//                get
//                {
//                    return MaxCount;
//                }
//            }

//            public override Android.Support.V4.App.Fragment GetItem(int position)
//            {
//                int month = position - 100;
//                MvxFragment fragment;

//                fragment = new ItemViewPagerCalendarFragment(month);


//                return fragment;
//            }


//        }

//        #endregion



//        #endregion

//    }
//}