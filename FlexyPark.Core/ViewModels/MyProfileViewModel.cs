using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public interface IMyProfileView
    {
        /// <summary>
        /// Shows the correct tab according to index.
        /// </summary>
        /// <param name="tabIndex">Tab index.</param>
        void ShowTab(int tabIndex); 
    }

    public class MyProfileViewModel : BaseViewModel
    {
        #region Constructors

        public MyProfileViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
        }

       

        #endregion

        #region Init

        public async void Init(MyProfileTab tabIndex, bool isSignUp)
        {
            CommonVM = new CommonProfileViewModel(mApiService, mCacheService);
            OwnVM = new OwnProfileViewModel(mApiService, mCacheService);

            IsAllowTabChange = !isSignUp;

			if (!isSignUp)
			{
				CommonVM.Init(isSignUp);
				OwnVM.Init();

				GetPersonInformation();
			}

            await Task.Delay(200);

            if (View != null)
            {
                View.ShowTab((int)tabIndex);
            }
        }

        #endregion

        public IMyProfileView View {get;set;}

        #region Properties

        /*#region TextSource

        private IMvxLanguageBinder mTextSource = null;

        public IMvxLanguageBinder TextSource
        {
            get
            {
                if (mTextSource == null)
                    mTextSource = new MvxLanguageBinder(AppConstants.NameSpace, GetType().Name);

                Mvx.Resolve<ICacheService>().TextSource = mTextSource;
                return mTextSource;
            }
            set
            {
                mTextSource = value;
                RaisePropertyChanged();
            }
        }

        #endregion */

        #region CommonVM

        private CommonProfileViewModel mCommonVM = null;

        public CommonProfileViewModel CommonVM
        {
            get
            {
                return mCommonVM;
            }
            set
            {
                mCommonVM = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RentVM

        private RentFragmentViewModel mRentVM = null;

        public RentFragmentViewModel RentVM
        {
            get
            {
                return mRentVM;
            }
            set
            {
                mRentVM = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region OwnVM

        private OwnProfileViewModel mOwnVM = null;

        public OwnProfileViewModel OwnVM
        {
            get
            {
                return mOwnVM;
            }
            set
            {
                mOwnVM = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsAllowTabChange

        private bool mIsAllowTabChange = false;

        public bool IsAllowTabChange
        {
            get
            {
                return mIsAllowTabChange;
            }
            set
            {
                mIsAllowTabChange = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Methods

		private async void GetPersonInformation()
		{
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
			if (mCacheService.CurrentUser != null && !string.IsNullOrEmpty(mCacheService.CurrentUser.UserId))
			{
				var result = await mApiService.PersonGet(mCacheService.CurrentUser.UserId);
				if (result != null)
				{
					CommonVM.User = result.Response;
					OwnVM.User = result.Response;
				}
				
			}
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
		}

        #endregion
    }
}

