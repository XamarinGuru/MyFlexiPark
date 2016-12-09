using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public interface IReportSelectionView
    {
        /// <summary>
        /// iOS only
        /// </summary>
        void ReloadTable();
    }

    public class ReportSelectionViewModel : BaseViewModel
    {
        #region Constructors

        public ReportSelectionViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init()
        {
            await Task.Delay(200);
            string[] ListProblem = new[]
            {
                /*mCacheService.TextSource.GetText("ExitText"),
                mCacheService.TextSource.GetText("ExtensionText"),*/
                mCacheService.TextSource.GetText("AlreadyTakenText"),
                mCacheService.TextSource.GetText("CarOnTheStreetText"),
                mCacheService.TextSource.GetText("NoMarkText"),
                mCacheService.TextSource.GetText("NoSpotText"),
                mCacheService.TextSource.GetText("ParkingFullText"),
                mCacheService.TextSource.GetText("RefuseTheSpotText"),
                mCacheService.TextSource.GetText("CanNotLeaveText"),
                mCacheService.TextSource.GetText("LeaveAndReportProblemText")
               
            };
            Problems = new ObservableCollection<string>(ListProblem);

            if (View != null)
                View.ReloadTable();
        }

        #endregion

        public IReportSelectionView View { get; set; }

        #region Properties

        #region Problems

        private ObservableCollection<string> mProblems = new ObservableCollection<string>();

        public ObservableCollection<string> Problems
        {
            get
            {
                return mProblems;
            }
            set
            {
                mProblems = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region ReportItemSelectedCommand

        private MvxCommand<int> mReportItemSelectedCommand = null;

        public MvxCommand<int> ReportItemSelectedCommand
        {
            get
            {
                if (mReportItemSelectedCommand == null)
                {
                    mReportItemSelectedCommand = new MvxCommand<int>(this.ReportItemSelected);
                }
                return mReportItemSelectedCommand;
            }
        }

        private void ReportItemSelected(int index)
        {
            var mode = ReportMode.PlateNumber;

            switch (index)
            {
                case 0:
                    mode = ReportMode.PlateNumber;
                    break;
                case 1:
                    mode = ReportMode.Unreachable;
                    break;
                case 2:
                    mode = ReportMode.NotFound;
                    break;
                case 3:
                    mode = ReportMode.Refund;
                    break;
                case 4:
                    mode = ReportMode.Full;
                    break;
                case 5:
                    mode = ReportMode.PictureRefuse;
                    break;
                case 6:
                    mode = ReportMode.CallOwner;
                    break;
                case 7:
                    mode = ReportMode.PictureLeave;
                    break;
                default:
                    break;
            }

            ShowViewModel<ReportViewModel>(new {mode = mode});
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

