using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class GotoSpotViewModel : BaseViewModel
    {
        #region Constructors

        public GotoSpotViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init()
        {
                
        }

        #endregion

        #region Properties

        #endregion

        #region Commands

        #region ImOnMySpotCommand

        private MvxCommand mImOnMySpotCommand = null;

        public MvxCommand ImOnMySpotCommand
        {
            get
            {
                if (mImOnMySpotCommand == null)
                {
                    mImOnMySpotCommand = new MvxCommand(this.ImOnMySpot);
                }
                return mImOnMySpotCommand;
            }
        }

        private void ImOnMySpot()
        {
            mCacheService.NextStatus = AddSpotStatus.GPS;
            Close(this);
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

