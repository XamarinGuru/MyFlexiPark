using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Helpers;
using System.Linq;
using FlexyPark.Core.Services;
using Flurl.Http;

namespace FlexyPark.Core.ViewModels
{
    public interface IReportView
    {
        Task<byte[]> TakePicture();

        Task<byte[]> SelectFromLibrary();

        void SavePictureToLibrary(byte[] imgData);

        //void ShowPicker();

        void CallOwner(string phoneNumber);

        /// <summary>
        /// iOS only.
        /// </summary>
        void ConfigHeight(int index);
    }

    public class ReportViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;
        #region Constructors

        public ReportViewModel(IApiService apiService, ICacheService cacheService , IPlatformService platformService)
            : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
        }

        #endregion

        #region Init

        public void Init(ReportMode mode)
        {
            Mode = mode;

            switch (Mode)
            {
                case ReportMode.PlateNumber: 
                    LeavingStatus = "parkingOccupied";
                    LeaveComment = "the spot is already taken";
                    break;
                case ReportMode.Unreachable:
                    LeavingStatus = "parkingUnreachable";
                    LeaveComment = "there is a car on the street...";
                    break;
                case ReportMode.NotFound:
                    LeavingStatus = "parkingMarkNotFound";
                    //LeaveComment = " there is no mark...";
                    break;
                case ReportMode.Refund:
                    LeavingStatus = "parkingNotFound";
                    //LeaveComment = "there is no spot";
                    break;

                case ReportMode.Full:
                    LeavingStatus = "parkingFull";
                    //LeaveComment = "the parking is full";
                    break;
                case ReportMode.PictureRefuse:
                    LeavingStatus = "parkingRefused";
                    LeaveComment = "I refuse the spot";
                    break;
                case ReportMode.CallOwner:
                    LeavingStatus = "leavingImpossible";
                    //LeaveComment = "I can’t leave the spot";
                    break;
                case ReportMode.PictureLeave:
                    LeavingStatus = "problemReported";
                    LeaveComment = "I leave and report a problem";
                    break;


            }
        }

        #endregion

        public IReportView View { get; set; }

        #region Properties

        #region LeavingStatus

        public string LeavingStatus { get; set; }

        #endregion

        #region LeaveComment

        public string LeaveComment { get; set; }

        #endregion

        #region Mode

        private ReportMode mMode = ReportMode.PlateNumber;

        public ReportMode Mode
        {
            get
            {
                return mMode;
            }
            set
            {
                mMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Problem

        private string mProblem = string.Empty;

        public string Problem
        {
            get
            {
                return mProblem;
            }
            set
            {
                mProblem = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PlateNumber

        private string mPlateNumber = string.Empty;

        public string PlateNumber
        {
            get
            {
                return mPlateNumber;
            }
            set
            {
                mPlateNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Image

        private byte[] mImage = null;

        public byte[] Image
        {
            get
            {
                return mImage;
            }
            set
            {
                mImage = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #endregion

        #region Commands

        #region AddPictureCommand

        private MvxCommand mAddPictureCommand = null;

        public MvxCommand AddPictureCommand
        {
            get
            {
                if (mAddPictureCommand == null)
                {
                    mAddPictureCommand = new MvxCommand(this.AddPicture);
                }
                return mAddPictureCommand;
            }
        }

        private void AddPicture()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, mCacheService.TextSource.GetText("PleaseChooseOptionText"), mCacheService.TextSource.GetText("PleaseChooseOptionText"), mCacheService.TextSource.GetText("CancelText"), null, new string[] { mCacheService.TextSource.GetText("TakeAPictureText"), mCacheService.TextSource.GetText("SelectFromLibraryText") }, () =>
                    {
                        TakeAPicture();
                    }, () =>
                    {
                        SelectFromLibrary();
                    }));
        }

        #endregion

        #region SendReportCommand

        private MvxCommand mSendReportCommand = null;

        public MvxCommand SendReportCommand
        {
            get
            {
                if (mSendReportCommand == null)
                {
                    mSendReportCommand = new MvxCommand(this.SendReport);
                }
                return mSendReportCommand;
            }
        }

        private void SendReport()
        {
            if (Mode == ReportMode.CallOwner)
                return;

            if (Mode == ReportMode.PlateNumber && string.IsNullOrEmpty(PlateNumber))
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, TextSource.GetText("PleaseFillPlateNumberText")));
                return;
            }

            if ((Mode == ReportMode.PictureLeave || Mode == ReportMode.PictureRefuse) && string.IsNullOrEmpty(Problem))
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, TextSource.GetText("PleaseFillProblemText")));
                return;
            }

            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("WarningText"), SharedTextSource.GetText("ReportThisParkingText"), SharedTextSource.GetText("NoText"), null, new string[] { SharedTextSource.GetText("YesText") }, 
                    async () =>
                    {
                        LeaveBooking();
                    }
                ));
        }

        #endregion

        #region CallOwnerCommand

        private MvxCommand mCallOwnerCommand = null;

        public MvxCommand CallOwnerCommand
        {
            get
            {
                if (mCallOwnerCommand == null)
                {
                    mCallOwnerCommand = new MvxCommand(this.CallOwner);
                }
                return mCallOwnerCommand;
            }
        }

        private void CallOwner()
        {
            if (View != null)
                View.CallOwner("12345678");
        }

        #endregion

        #endregion

        #region Methods

        #region Take A Picture

        private async void TakeAPicture()
        {
            if (View != null)
            {
                var bytes = await View.TakePicture();
                if (bytes != null)
                {
                    Image = bytes;
                    View.SavePictureToLibrary(bytes);
                }
            }
        }

        #endregion

        #region Select From Library

        private async void SelectFromLibrary()
        {
            if (View != null)
            {
                var bytes = await View.SelectFromLibrary();
                if (bytes != null)
                {
                    Image = bytes;
                    View.SavePictureToLibrary(bytes);
                }
            }
        }

        #endregion

        #region LeaveBooking

        private async void LeaveBooking()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                if (Mode == ReportMode.PictureLeave || Mode == ReportMode.PictureRefuse)
                    LeaveComment = Problem;

                if (Mode == ReportMode.PlateNumber || Mode == ReportMode.Unreachable)
                    LeaveComment = PlateNumber;
                
                var result = await mApiService.LeaveBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, mCacheService.CurrentReservation.Parking.ParkingId, mCacheService.CurrentReservation.BookingId, "0/1", LeaveComment, LeavingStatus);
                if (result != null)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                    if (result.Response.Status.Equals("success"))
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                        mPlatformService.SetPreference<long>(AppConstants.BookingExpiredTime, 0);
                        mPlatformService.SetPreference<long>(AppConstants.CurrentBookingId, 0);
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                    }

                    ShowViewModel<ParkingListsViewModel>(presentationFlag: PresentationBundleFlagKeys.ParkingList);
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion

        #endregion

    }
}

