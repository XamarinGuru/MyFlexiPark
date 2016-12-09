using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;
using FlexyPark.Core.Helpers;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public interface IBaseView 
    {
        bool CheckInternetConnection();
    }

    public class BaseViewModel : MvxViewModel
    {
        public readonly IApiService mApiService;
        public readonly ICacheService mCacheService;

        #region Constructors

        public BaseViewModel(IApiService apiService, ICacheService cacheService)
        {
            this.mApiService = apiService;
            this.mCacheService = cacheService;
        }

        #endregion

        #region Properties

        #region TextSource
        private IMvxLanguageBinder mTextSource;
        public IMvxLanguageBinder TextSource
        {
            get{
                if(mTextSource == null)
                    mTextSource = new MvxLanguageBinder(AppConstants.NameSpace, GetType().Name);
                
                Mvx.Resolve<ICacheService>().TextSource = mTextSource;
                return mTextSource;
            }
            set{
                mTextSource = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SharedTextSource
        private IMvxLanguageBinder mSharedTextSource;
        public IMvxLanguageBinder SharedTextSource
        {
            get
            {
                if (mSharedTextSource == null)
                    mSharedTextSource = new MvxLanguageBinder(AppConstants.NameSpace, "Shared");

                Mvx.Resolve<ICacheService>().SharedTextSource = mSharedTextSource;
                return mSharedTextSource;
            }
            set
            {
                mSharedTextSource = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /*#region PageTitle

        private string mPageTitle = string.Empty;

        public string PageTitle
        {
            get
            {
                return mPageTitle;
            }
            set
            {
                mPageTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region BackTitle

        private string mBackTitle = string.Empty;

        public string BackTitle
        {
            get
            {
                return mBackTitle;
            }
            set
            {
                mBackTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion*/

        #endregion

        protected void ClearStackAndShowViewModel<TViewModel>() where TViewModel : BaseViewModel
        {
            var presentationBundle =
                new MvxBundle(new Dictionary<string, string> {{PresentationBundleFlagKeys.ClearStack, ""}});

            ShowViewModel<TViewModel>(presentationBundle: presentationBundle);
        }

        protected void ShowViewModelParameter<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            var text = Mvx.Resolve<IMvxJsonConverter>().SerializeObject(parameter);

            base.ShowViewModel<TViewModel>(new Dictionary<string, string>()
            {
                {"parameter", text}
            });
		}

		protected void ShowChildViewModel<TViewModel>() where TViewModel : BaseChildViewModel
		{
			var presentationBundle = new MvxBundle(new Dictionary<string, string> { { PresentationBundleFlagKeys.Child, "" } });

			ShowViewModel<TViewModel>(presentationBundle: presentationBundle);
		}

        protected void ShowViewModel<TViewModel>(string presentationFlag) where TViewModel : BaseViewModel
        {
            var presentationBundle = new MvxBundle(new Dictionary<string, string> { { presentationFlag, "" } });

            ShowViewModel<TViewModel>(presentationBundle: presentationBundle);
        }

        protected void ShowViewModel<TViewModel>(string presentationFlag, IMvxBundle parameterBundle) where TViewModel : BaseViewModel
        {
            var presentationBundle = new MvxBundle(new Dictionary<string, string> { { presentationFlag, "" } });

            ShowViewModel<TViewModel>(parameterBundle: parameterBundle, presentationBundle: presentationBundle);
        }

        protected void ShowViewModelWithCallback<TViewModel, TReturn, TParam>(TParam param = default(TParam), Action<TReturn> returnObject = null) where TViewModel : MvxViewModel
        {
            if (returnObject != null && param != null)
            {
                var delegateKey = Mvx.Resolve<IDelegateManager>().SetAction<TReturn, TParam>(returnObject, param);
                base.ShowViewModel<TViewModel>(new NavigationWithDelegateParameter() { DelegateKey = delegateKey});
            }
        }

        #region View

        public IBaseView BaseView { get; set;}

        #endregion

        #region Commands

        #region BackCommand

        private MvxCommand mBackCommand = null;

        public MvxCommand BackCommand
        {
            get
            {
                if (mBackCommand == null)
                {
                    mBackCommand = new MvxCommand(this.Back);
                }
                return mBackCommand;
            }
        }

        private void Back()
        {
            Close(this);
        }

        #endregion

        #endregion
    }


    /*public class BaseDelegateViewModel<T, K> : BaseViewModel
    {
        public virtual void CloseAndReturn(T message, K param)
        {
            var delegateManager = Mvx.Resolve<IDelegateManager>();

            var action = delegateManager.GetAction<T, K>(mDelegateKey.DelegateKey);
            if (action != null)
            {
                action(message);
                delegateManager.Remove(mDelegateKey.DelegateKey);
                action = null;
            }
            this.Close(this);
        }

        private NavigationWithDelegateParameter mDelegateKey;
        public void Init(NavigationWithDelegateParameter delegateKey)
        {
            mDelegateKey = delegateKey;
        }
    }*/

    public class NavigationWithDelegateParameter
    {
        public string DelegateKey {get;set;}
    }
}

