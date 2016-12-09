using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Constructors

        public SettingsViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
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

        #region GotoMyProfileCommand

        private MvxCommand mGotoMyProfileCommand = null;

        public MvxCommand GotoMyProfileCommand
        {
            get
            {
                if (mGotoMyProfileCommand == null)
                {
                    mGotoMyProfileCommand = new MvxCommand(this.GotoMyProfile);
                }
                return mGotoMyProfileCommand;
            }
        }

        private void GotoMyProfile()
        {
            ShowViewModel<MyProfileViewModel>(new { tabIndex = MyProfileTab.Common , isSignUp = false});
            //ShowViewModel<CommonProfileViewModel> (new { isSignUp = false });
        }

        #endregion

        #region GotoAppSettingsCommand

        private MvxCommand mGotoAppSettingsCommand = null;

        public MvxCommand GotoAppSettingsCommand
        {
            get
            {
                if (mGotoAppSettingsCommand == null)
                {
                    mGotoAppSettingsCommand = new MvxCommand(this.GotoAppSettings);
                }
                return mGotoAppSettingsCommand;
            }
        }

        private void GotoAppSettings()
        {
            ShowViewModel<AppSettingsViewModel>();
        }

        #endregion

        #region GotoMyVehiclesCommand

        private MvxCommand mGotoMyVehiclesCommand = null;

        public MvxCommand GotoMyVehiclesCommand {
            get {
                if (mGotoMyVehiclesCommand == null) {
                    mGotoMyVehiclesCommand = new MvxCommand (this.GotoMyVehicles);
                }
                return mGotoMyVehiclesCommand;
            }
        }

        private void GotoMyVehicles ()
        {
            ShowViewModel<ChooseVehicleViewModel> (new { mode = ChooseVehicleMode.NoAction });
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

