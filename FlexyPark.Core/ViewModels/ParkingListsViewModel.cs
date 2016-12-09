using System;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using FlexyPark.Core.Helpers;
using System.Globalization;
using Cirrious.CrossCore.Platform;
using System.Diagnostics;


namespace FlexyPark.Core.ViewModels
{
    public interface IParkingListsView
    {
        void SetSliderValue();

        /// <summary>
        /// Shows the date picker.
        /// </summary>
        void ShowDatePicker();

        /// <summary>
        /// Shows the time picker.
        /// </summary>
        void ShowTimePicker();

        /// <summary>
        /// iOS only.
        /// </summary>
        void ResetHeight();

        void StartTimer();

        void StopTimer();

        void Start60sTimer();

        void Stop60sTimer();
    }

    public class ParkingListsViewModel : BaseViewModel
    {
        #region Constructors

        public ParkingListsViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init(long startTimeStamp, double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

            if (mCacheService.SelectedVehicle == null)
                Debug.WriteLine("Selected Vehicle is null. Something has gone wrong @.@ !");

            Vehicle = mCacheService.SelectedVehicle;

            if (startTimeStamp != 0)
            {
                StartTimeStamp = startTimeStamp;
                EndBookingDateTime = Helpers.TimestampHelpers.UnixTimeStampToDateTime(StartTimeStamp);
            }

            UpdateValidTime();

            if (mCacheService.SearchMode == SearchMode.Later)
                Title = "Please choose";
            else
                Title = "Acquiring location…";

            await Task.Delay(100);


//            if (View != null)
//            {
//                View.StartTimer();
//                if (mCacheService.SearchMode == SearchMode.Now)
//                    View.Start60sTimer();
//            }



//            ValidTime = DateTime.UtcNow.ToLocalTime().Add(TimeSpan.FromHours(ParkingTime + 1));
//
//            if (startTimeStamp != 0)
//            {
//                StartTimeStamp = startTimeStamp;
//                ValidTime = StartTimeStamp.UnixTimeStampToDateTime().Add(TimeSpan.FromHours(ParkingTime + 1));
//            }

            //IsShowParkingLists = ParkingTime != 0;

            //GetParkingLists();
        }

        #endregion

        public IParkingListsView View { get; set; }

        #region Properties

        #region Title

        private string mTitle = string.Empty;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Counter

        private int mCounter = 1;

        public int Counter
        {
            get
            {
                return mCounter;
            }
            set
            {
                mCounter = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region StartTimeStamp

        //used when book later

        private long mStartTimeStamp = 0;

        public long StartTimeStamp
        {
            get
            {
                return mStartTimeStamp;
            }
            set
            {
                mStartTimeStamp = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ParkingSlots

        private ObservableCollection<ParkingSlotItemViewModel> mParkingSlots = new ObservableCollection<ParkingSlotItemViewModel>();

        public ObservableCollection<ParkingSlotItemViewModel> ParkingSlots
        {
            get
            {
                return mParkingSlots;
            }
            set
            {
                mParkingSlots = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ParkingTime

        private int mParkingTime = 1;

        public int ParkingTime
        {
            get
            {
                return mParkingTime;
            }
            set
            {
                mParkingTime = value;
                if (StartTimeStamp != 0)
                    EndBookingDateTime = StartTimeStamp.UnixTimeStampToDateTime().AddHours(mParkingTime + 1);
                else
                    EndBookingDateTime = DateTime.UtcNow.ToLocalTime().AddHours(mParkingTime + 1);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ValidTime

        private DateTime mValidTime = DateTime.UtcNow.ToLocalTime();

        public DateTime ValidTime
        {
            get
            {
                return mValidTime;
            }
            set
            {
                mValidTime = value;
                if (ParkingTime == 4)
                    GetParkingLists();
                
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowEndBookingDate

        private bool mIsShowEndBookingDate = false;

        public bool IsShowEndBookingDate
        {
            get
            {
                return mIsShowEndBookingDate;
            }
            set
            {
                mIsShowEndBookingDate = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowParkingLists

        private bool mIsShowParkingLists = false;

        public bool IsShowParkingLists
        {
            get
            {
                return mIsShowParkingLists;
            }
            set
            {
                mIsShowParkingLists = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndBookingDateTime

        private DateTime mEndBookingDateTime = DateTime.UtcNow.ToLocalTime();

        public DateTime EndBookingDateTime
        {
            get
            {
                return mEndBookingDateTime;
            }
            set
            {
                mEndBookingDateTime = value;
                ValidTime = EndBookingDateTime;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Vehicle

        private Vehicle mVehicle = null;

        public Vehicle Vehicle
        {
            get
            {
                return mVehicle;
            }
            set
            {
                mVehicle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Latitude

        private double mLatitude = 0;

        public double Latitude
        {
            get
            {
                return mLatitude;
            }
            set
            {
                mLatitude = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Longitude

        private double mLongitude = 0;

        public double Longitude
        {
            get
            {
                return mLongitude;
            }
            set
            {
                mLongitude = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoBookingCommand

        private MvxCommand<ParkingSlotItemViewModel> mGotoBookingCommand = null;

		public MvxCommand<ParkingSlotItemViewModel> GotoBookingCommand
		{
			get
			{
				if (mGotoBookingCommand == null)
				{
					mGotoBookingCommand = new MvxCommand<ParkingSlotItemViewModel>(this.GotoBooking);
				}
				return mGotoBookingCommand;
			}
		}

		private async void GotoBooking(ParkingSlotItemViewModel itemVM)
		{
			/*if (BaseView != null &&  BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var result = await mApiService.CreateBooking("1","parkingId", 1101010101010 , 1010101010101, "carNumber");
                if (result != null)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result + "\n" + result.BookingId));
                     Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                    ShowViewModel<BookingViewModel>(new {timeout =  Int32.Parse(result.TimeoutDuration), cost = double.Parse(result.Cost), remainingCredits = Int32.Parse(result.RemainingCredits) });
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }*/

			if (BaseView != null && BaseView.CheckInternetConnection())
			{
				//Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

				//                long startTimeStamp = DateTime.UtcNow.DateTimeToTimeStamp();
				//                long endTimeStamp = DateTime.UtcNow.AddHours(ParkingTime + 1).DateTimeToTimeStamp();

				long startTimeStamp = ValidTime.AddHours(-(ParkingTime + 1)).DateTimeToTimeStamp();
				long endTimeStamp = ValidTime.DateTimeToTimeStamp();
				double toWait = Math.Round((double)(DateTime.UtcNow.DateTimeToTimeStamp() - startTimeStamp) / 60);

				if (StartTimeStamp != 0)
				{
					startTimeStamp = StartTimeStamp;
					endTimeStamp = startTimeStamp + ((ParkingTime + 1) * 3600);
					toWait = Math.Round((double)(startTimeStamp - DateTime.UtcNow.DateTimeToTimeStamp()) / 60);
				}

				if (itemVM.ParkingSpot.BookingType == "immediate")
				{
					ShowViewModel<BookingViewModel>(new
					{
						parkingid = itemVM.ParkingSpot.ParkingId,
						distance = double.Parse(itemVM.ParkingSpot.Distance, CultureInfo.InvariantCulture),
						cost = itemVM.ParkingSpot.Cost,
						remainingCredits = double.Parse(mCacheService.CurrentUser.RemainingCredits, CultureInfo.InvariantCulture),
						toWait = toWait,
						duration = Math.Round((double)(endTimeStamp - startTimeStamp) / 3600),
						startTimestamp = startTimeStamp,
						endTimestamp = endTimeStamp
					});
				}
				else if (itemVM.ParkingSpot.BookingType == "delayed")
				{
					ShowViewModel<DelayedParkingMapViewModel>(new
					{
						parkingid = itemVM.ParkingSpot.ParkingId,
						startTimestamp = startTimeStamp,
						endTimestamp = endTimeStamp
					});
				}

				//                var result = await mApiService.CreateBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, itemVM.ParkingSpot.ParkingId, startTimeStamp, endTimeStamp, Vehicle.PlateNumber);
				//                if (result != null && result.Response != null)
				//                {
				//                    // Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result + "\n" + result.Response.BookingId));
				//                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
				//                    if (result.Response.Result.Equals("success"))
				//                    {
				//                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result));
				//                    }
				//                    else
				//                    {
				//                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode), "Ok", null));
				//                    }


				//                }
				//                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
				//            }
				//            else
				//            {
				//                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
				//            }
			}
		}
		//#region GotoParkingMapCommand

		//private MvxCommand<ParkingSlotItemViewModel> mGotoDelayedParkingMapCommand = null;

		//public MvxCommand<ParkingSlotItemViewModel> GotoDelayedParkingMapCommand
		//{
		//	get
		//	{
		//		if (mGotoDelayedParkingMapCommand == null)
		//		{
		//			mGotoDelayedParkingMapCommand = new MvxCommand<ParkingSlotItemViewModel>(this.GotoDelayedParkingMap);
		//		}
		//		return mGotoDelayedParkingMapCommand;
		//	}
		//}

		//private async void GotoDelayedParkingMap(ParkingSlotItemViewModel itemVM)
		//{
		//	if (BaseView != null && BaseView.CheckInternetConnection())
		//	{
		//		//long startTimeStamp = ValidTime.AddHours(-(ParkingTime + 1)).DateTimeToTimeStamp();
		//		//long endTimeStamp = ValidTime.DateTimeToTimeStamp();

		//		//if (StartTimeStamp != 0)
		//		//{
		//		//	startTimeStamp = StartTimeStamp;
		//		//	endTimeStamp = startTimeStamp + ((ParkingTime + 1) * 3600);
		//		//}

		//		//ShowViewModel<DelayedParkingMapViewModel>(new
		//		//{
		//		//	parkingid = itemVM.ParkingSpot.ParkingId,
		//		//	startTimestamp = startTimeStamp,
		//		//	endTimestamp = endTimeStamp
		//		//});
		//		long startTimeStamp = ValidTime.AddHours(-(ParkingTime + 1)).DateTimeToTimeStamp();
		//		long endTimeStamp = ValidTime.DateTimeToTimeStamp();
		//		double toWait = Math.Round((double)(DateTime.UtcNow.DateTimeToTimeStamp() - startTimeStamp) / 60);

		//		if (StartTimeStamp != 0)
		//		{
		//			startTimeStamp = StartTimeStamp;
		//			endTimeStamp = startTimeStamp + ((ParkingTime + 1) * 3600);
		//			toWait = Math.Round((double)(startTimeStamp - DateTime.UtcNow.DateTimeToTimeStamp()) / 60);
		//		}

		//		ShowViewModel<DelayedParkingMapViewModel>(new
		//		{
		//			parkingid = itemVM.ParkingSpot.ParkingId,
		//			startTimestamp = startTimeStamp,
		//			endTimestamp = endTimeStamp
		//		});
		//	}
		//}

		//#endregion

		#endregion

		#region HandleValueChangedCommand

		private MvxCommand mHandleValueChangedCommand = null;

        public MvxCommand HandleValueChangedCommand
        {
            get
            {
                if (mHandleValueChangedCommand == null)
                {
                    mHandleValueChangedCommand = new MvxCommand(this.HandleValueChanged);
                }
                return mHandleValueChangedCommand;
            }
        }

        public void HandleValueChanged()
        {
//            ValidTime = DateTime.UtcNow.ToLocalTime().Add(TimeSpan.FromHours(ParkingTime));

            if (View != null)
            {
                View.SetSliderValue();
                View.ResetHeight();
            }

            if (StartTimeStamp == 0)
            {
                ValidTime = DateTime.UtcNow.ToLocalTime().Add(TimeSpan.FromHours(ParkingTime + 1));

            }
            else
            {
                ValidTime = TimestampHelpers.UnixTimeStampToDateTime(StartTimeStamp + ((ParkingTime + 1) * 3600));

            }

            //IsShowParkingLists = ParkingTime     != 0;
            IsShowEndBookingDate = (ParkingTime) == 4;
            //IsShowEndBookingDate = (ParkingTime+1) == 5;

            GetParkingLists();
        }

        #endregion

        #region SelectEndDateCommand

        private MvxCommand mSelectEndDateCommand = null;

        public MvxCommand SelectEndDateCommand
        {
            get
            {
                if (mSelectEndDateCommand == null)
                {
                    mSelectEndDateCommand = new MvxCommand(this.SelectEndDate);
                }
                return mSelectEndDateCommand;
            }
        }

        private void SelectEndDate()
        {
            if (View != null)
                View.ShowDatePicker();
        }

        #endregion

        #region SelectEndHourCommand

        private MvxCommand mSelectEndHourCommand = null;

        public MvxCommand SelectEndHourCommand
        {
            get
            {
                if (mSelectEndHourCommand == null)
                {
                    mSelectEndHourCommand = new MvxCommand(this.SelectEndHour);
                }
                return mSelectEndHourCommand;
            }
        }

        private void SelectEndHour()
        {
            if (View != null)
                View.ShowTimePicker();
        }

        #endregion

        #endregion

        #region Methods

        #region UpdateValidTime

        public void UpdateValidTime()
        {
            if (ParkingTime != 4)
            {
                ValidTime = DateTime.UtcNow.ToLocalTime().Add(TimeSpan.FromHours(ParkingTime + 1));

                if (StartTimeStamp != 0) //book later
                {
                    ValidTime = StartTimeStamp.UnixTimeStampToDateTime().Add(TimeSpan.FromHours(ParkingTime + 1));
                }
            }
                
        }

        #endregion

        #region Get Parking Lists

        public async void GetParkingLists()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                if (Latitude == 0 && Longitude == 0 && mCacheService.SearchMode == SearchMode.Later)
                    return;
                
                if (mCacheService.CurrentLat == 0 && mCacheService.CurrentLng == 0)
                    return;

                //if (mCacheService.SearchMode == SearchMode.Now)
                //{
                    Title = "Please choose";
                    if (View != null)
                    {
                        await Task.Delay(100);
                        View.Stop60sTimer();
                    }
                //}
                
                IsShowParkingLists = false;
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true, string.Empty, true));
                long startTimeStamp = DateTime.UtcNow.DateTimeToTimeStamp();
                //long endTimeStamp = DateTime.UtcNow.AddHours(ParkingTime + 1).DateTimeToTimeStamp();
                long endTimeStamp = ValidTime.DateTimeToTimeStamp();

                if (StartTimeStamp != 0)
                {
                    startTimeStamp = StartTimeStamp;
                }


                Vehicle = mCacheService.SelectedVehicle;

                double lat = Latitude != 0 ? Latitude : mCacheService.CurrentLat;
                double lng = Longitude != 0 ? Longitude : mCacheService.CurrentLng;

                ParkingSlots.Clear();
                string maxdistance = string.Empty;
                if (StartTimeStamp == 0)
                {
                    maxdistance = "1000";
                }
                else
                {
                    maxdistance = "10000";
                }
                var parkings = await mApiService.AvaiableParkings(startTimeStamp, endTimeStamp, 2, lat, lng, maxdistance, Vehicle.Type);

                if (parkings != null && parkings.Response.Count != 0)
                {
                    ObservableCollection<ParkingSlotItemViewModel> Tmp = new ObservableCollection<ParkingSlotItemViewModel>();
                    foreach (var parking in parkings.Response)
                    {
						if (mCacheService.SearchMode == SearchMode.Now && parking.BookingType == "delayed")
							continue;
                        Tmp.Add(new ParkingSlotItemViewModel() { ParkingSpot = parking, IsShowClock = startTimeStamp < Convert.ToInt64(parking.AvailabilityStartTimestamp) });
                    }
                    ParkingSlots = Tmp;
                }
                else
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "No parkings"));

                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                IsShowParkingLists = true;
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion

        #region CloseViewModel

        public void CloseViewModel()
        {
            Close(this);
        }

        #endregion

        #region ShowWeakSignalGPS

        public void ShowWeakSignalGPS()
        {
            if (View != null)
                View.Stop60sTimer();
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, "Weak gps signal: please go outside", "OK", () =>
                    {
                        CloseViewModel();
                    }));
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
        }

        #endregion

        #endregion
    }
}

