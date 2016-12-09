using System;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.ViewModels;

namespace FlexyPark.Core.ViewModels
{
    public class AddSpotStatusViewModel : BaseViewModel
    {
        #region Constructors

        public AddSpotStatusViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init(bool disabled)
        {
            IsParkingDisabled = disabled;
        }

        #endregion

        #region Properties

        #region IsParkingDisabled

        private bool mIsParkingDisabled = false;

        public bool IsParkingDisabled
        {
            get
            {
                return mIsParkingDisabled;
            }
            set
            {
                mIsParkingDisabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region DoneCommand

        private MvxCommand mDoneCommand = null;

        public MvxCommand DoneCommand
        {
            get
            {
                if (mDoneCommand == null)
                {
                    mDoneCommand = new MvxCommand(this.Done);
                }
                return mDoneCommand;
            }
        }

        private void Done()
        {
            mCacheService.CreateParkingRequest.Disabled = IsParkingDisabled.ToString();
            Close(this);
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

