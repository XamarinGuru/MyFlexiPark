using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Threading.Tasks;
using FlexyPark.Core.Helpers;
using FlexyPark.Core.Services;
using System.Globalization;

namespace FlexyPark.Core.ViewModels
{

    public class BookingViewModel : BaseViewModel
    {
        #region Constructors

        public BookingViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init(double distance, double cost, double remainingCredits, double toWait, double duration, long parkingid, long startTimestamp, long endTimestamp)
        {
            ParkingId = parkingid;

            Distance = distance;
            Cost = cost;
            RemainingCredits = remainingCredits;

            MinutesToWait = Convert.ToInt32(toWait);
            Duration = Convert.ToInt32(duration);

			#region test pay part
            //IsShowBuyCredits = RemainingCredits < Cost;
			#endregion

            Vehicle = mCacheService.SelectedVehicle;
            StartTimeStamp = startTimestamp;
            EndTimeStamp = endTimestamp;

            EndTime = endTimestamp.UnixTimeStampToDateTime();

            IsShowClock = mCacheService.SearchMode == SearchMode.Now;
        }
        #endregion

        #region Properties

        #region Address

        private string mAddress = string.Empty;

        public string Address
        {
            get
            {
                return mAddress;
            }
            set
            {
                mAddress = value;
            }
        }

        #endregion

        #region StartTimeStamp

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
            }
        }

        #endregion

        #region EndTimeStamp

        private long mEndTimeStamp = 0;

        public long EndTimeStamp
        {
            get
            {
                return mEndTimeStamp;
            }
            set
            {
                mEndTimeStamp = value;
            }
        }

        #endregion

        #region NeedRelease

        private bool mNeedRelease = true;

        public bool NeedRelease
        {
            get
            {
                return mNeedRelease;
            }
            set
            {
                mNeedRelease = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowClock

        private bool mIsShowClock = false;

        public bool IsShowClock
        {
            get
            {
                return mIsShowClock;
            }
            set
            {
                mIsShowClock = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Cost

        private double mCost = 1;

        public double Cost
        {
            get
            {
                return mCost;
            }
            set
            {
                mCost = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region MinutesToWait

        private int mMinutesToWait = 4;

        public int MinutesToWait
        {
            get
            {
                return mMinutesToWait;
            }
            set
            {
                mMinutesToWait = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Duration

        private int mDuration = 2;

        public int Duration
        {
            get
            {
                return mDuration;
            }
            set
            {
                mDuration = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RemainingCredits

        private double mRemainingCredits = 0;

        public double RemainingCredits
        {
            get
            {
                return mRemainingCredits;
            }
            set
            {
                mRemainingCredits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Distance

        private double mDistance = 300;

        public double Distance
        {
            get
            {
                return mDistance;
            }
            set
            {
                mDistance = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndTime

        private DateTime mEndTime = DateTime.UtcNow.ToLocalTime();

        public DateTime EndTime
        {
            get
            {
                return mEndTime;
            }
            set
            {
                mEndTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowBuyCredits

        private bool mIsShowBuyCredits = false;

        public bool IsShowBuyCredits
        {
            get
            {
                return mIsShowBuyCredits;
            }
            set
            {
                mIsShowBuyCredits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region BookingId

        private long mBookingId = 0;

        public long BookingId
        {
            get
            {
                return mBookingId;
            }
            set
            {
                mBookingId = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ParkingId

        private long mParkingId = 0;

        public long ParkingId
        {
            get
            {
                return mParkingId;
            }
            set
            {
                mParkingId = value;
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


        #endregion

        #region Commands

        #region GotoParkingReservedCommand

        private MvxCommand mGotoParkingReservedCommand = null;

        public MvxCommand GotoParkingReservedCommand
        {
            get
            {
                if (mGotoParkingReservedCommand == null)
                {
                    mGotoParkingReservedCommand = new MvxCommand(this.GotoParkingReserved);
                }
                return mGotoParkingReservedCommand;
            }
        }

        private async void GotoParkingReserved()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(
				new AlertMessage(this, "Confirm", string.Format("Your payment card will be charged {0} €", string.Format("{0:0.0}", Cost)), "Cancel", () => { }, new string[] { "OK" },
						  async () =>
						  {
							  if (BaseView != null && BaseView.CheckInternetConnection())
							  {
								  Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

								  var result1 = await mApiService.CreateBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId.ToString(), StartTimeStamp, EndTimeStamp, Vehicle.PlateNumber);
								  if (result1 != null && result1.Response != null)
								  {
									  // Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result + "\n" + result.Response.BookingId));
									  //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? result.Response.Result : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
									  if (result1.Response.Status.Equals("success"))
									  {
										  BookingId = long.Parse(result1.Response.BookingId);
										  var result = await mApiService.PayBooking(mCacheService.CurrentUser.UserId, ParkingId.ToString(), result1.Response.BookingId);

										  if (result != null)
										  {
											  //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result + "\nRemaining credits : " + result.RemainingCredits));
											  //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : "Error"));
											  if (result.Response != null && result.Response.Status.Equals("success"))
											  {
												  Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));

												  NeedRelease = true;

												  if (mCacheService.SearchMode == SearchMode.Now)
												  {
													  //TODO : mCacheService.CurrentReservation = ??????

													  Parking parking = new Parking()
													  {
														  ParkingId = ParkingId.ToString(),
														  Location = result.Response.Location,
														  Latitude = result.Response.Latitude,
														  Longitude = result.Response.Longitude
													  };

													  Reservation reservation = new Reservation()
													  {
														  BookingId = BookingId.ToString(),
														  Cost = Cost.ToString(),
														  StartTimestamp = StartTimeStamp.ToString(),
														  EndTimestamp = EndTimeStamp.ToString(),
														  Parking = parking,
														  PlateNumber = Vehicle.PlateNumber,
														  VehicleType = Vehicle.Type,
													  };
													  mCacheService.CurrentReservation = reservation;
													  //mCacheService.CurrentUser.RemainingCredits = (double.Parse(mCacheService.CurrentUser.RemainingCredits, CultureInfo.InvariantCulture) - Cost).ToString();
													  //mCacheService.CurrentUser.RemainingCredits = result.Response.RemainingCredits;
													  ShowViewModel<ParkingReservedViewModel>(new { status = ParkingStatus.Rented });
												  }
												  else
													  ShowViewModel<MenuViewModel>(presentationFlag: PresentationBundleFlagKeys.Menu);
											  }
											  else if (result.ApiError.Error != null && result.ApiError.Error.Equals("cardTrouble"))
											  {
												  Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Please add a credit card in the Wallet section!"));
											  }
											  else
											  {
												  Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
											  }
										  }
										  else
										  {
											  //Close(this);
											  Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Error occur!"));
										  }
									  }
									  else
									  {
										  Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result1.Response.Status, result1.Response.ErrorCode), "Ok", null));
									  }

								  }
								  Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
							  }
							  else
							  {
								  Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
							  }
						  }));
        }

        #endregion

        #region GotoChooseVehicleCommand

        private MvxCommand mGotoChooseVehicleCommand = null;

        public MvxCommand GotoChooseVehicleCommand
        {
            get
            {
                if (mGotoChooseVehicleCommand == null)
                {
                    mGotoChooseVehicleCommand = new MvxCommand(this.GotoChooseVehicle);
                }
                return mGotoChooseVehicleCommand;
            }
        }

        private void GotoChooseVehicle()
        {
            //NeedRelease = false;
            //ShowViewModel<ChooseVehicleViewModel>();
        }

        #endregion

        #region GotoPaymentConfigurationCommand

        private MvxCommand mGotoPaymentConfigurationCommand = null;

        public MvxCommand GotoPaymentConfigurationCommand
        {
            get
            {
                if (mGotoPaymentConfigurationCommand == null)
                {
                    mGotoPaymentConfigurationCommand = new MvxCommand(this.GotoPaymentConfiguration);
                }
                return mGotoPaymentConfigurationCommand;
            }
        }

        private void GotoPaymentConfiguration()
        {
            NeedRelease = false;
            ShowViewModel<MyProfileViewModel>(new { tabIndex = MyProfileTab.Rent, isSignUp = false });

        }

        #endregion

        #region BuyCreditsCommand

        private MvxCommand mBuyCreditsCommand = null;

        public MvxCommand BuyCreditsCommand
        {
            get
            {
                if (mBuyCreditsCommand == null)
                {
                    mBuyCreditsCommand = new MvxCommand(this.BuyCredits);
                }
                return mBuyCreditsCommand;
            }
        }

        private void BuyCredits()
        {
            NeedRelease = false;
            //ShowViewModel<BuyCreditsViewModel>();
            ShowViewModel<RentProfileViewModel> ();
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

