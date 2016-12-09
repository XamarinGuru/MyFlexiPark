using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public class OwnerParkingItemViewModel : MvxViewModel
    {
        private readonly ICacheService mCacheService;
		private readonly MyOwnParkingViewModel mParentVM;

        #region Constructors

		public OwnerParkingItemViewModel( ICacheService cacheService, MyOwnParkingViewModel parentVM)
        {
			mParentVM = parentVM;
            mCacheService = cacheService;
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region TextSource

        private IMvxLanguageBinder mTextSource = null;

        public IMvxLanguageBinder TextSource
        {
            get
            {
                if(mTextSource == null)
                    mTextSource = new MvxLanguageBinder(AppConstants.NameSpace, GetType().Name);

                return mTextSource;
            }
            set
            {
                mTextSource = value;
                RaisePropertyChanged();
            }
        }

        #endregion     

        #region IsEditMode

        private bool mIsEditMode = false;

        public bool IsEditMode
        {
            get
            {
                return mIsEditMode;
            }
            set
            {
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Parking

        private OwnerParkingsResponse mParking = null;

        public OwnerParkingsResponse Parking
        {
            get
            {
                return mParking;
            }
            set
            {
                mParking = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Location

        private string mLocation = string.Empty;

        public string Location
        {
            get
            {
                return mLocation;
            }
            set
            {
                mLocation = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Status

        private string mStatus = string.Empty;

        public string Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoEditParkingSpotCommand

        private MvxCommand mGotoEditParkingSpotCommand = null;

        public MvxCommand GotoEditParkingSpotCommand
        {
            get{
                if( mGotoEditParkingSpotCommand == null){
                    mGotoEditParkingSpotCommand = new MvxCommand(this.GotoEditParkingSpot);
                }
                return mGotoEditParkingSpotCommand;
            }
        }
        private void GotoEditParkingSpot()
        {
            mCacheService.NextStatus = AddSpotStatus.GotoSpot;
            ShowViewModel<AddSpotViewModel>(new {isEditMode = true});
        }
        #endregion

		#region DeleteParkingSpotCommand

		private MvxCommand mDeleteParkingSpotCommand = null;

		public MvxCommand DeleteParkingSpotCommand
		{
			get{
				if( mDeleteParkingSpotCommand == null){
					mDeleteParkingSpotCommand = new MvxCommand(this.DeleteParkingSpot);
				}
				return mDeleteParkingSpotCommand;
			}
		}
		private void DeleteParkingSpot()
		{
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(mParentVM, TextSource.GetText("DeleteASpotText"), TextSource.GetText("MessageText"), TextSource.GetText("NoText"), null, new string[] { TextSource.GetText("YesText") }, 
				()=>{
				//delete parking here
                mParentVM.DeleteParkingSpot();
			}));
		}
		#endregion

        #endregion

        #region Methods

        #endregion
    }
}

