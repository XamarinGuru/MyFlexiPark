using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;

namespace FlexyPark.Core.ViewModels
{
    public interface IAppSettingsView
    {
        T GetPreference<T>(string key);
        void SetPreference<T>(string key, T value);

        string GetAppVersion();

        bool CheckIfAppInstalled(string appName);

        void GotoStoreToInstallApp(string appName);

        void ShowLanguagePicker();
    }

    public class AppSettingsViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;
        private readonly IMvxTextProviderBuilder mTextProviderBuilder;

        #region Constructors

        public AppSettingsViewModel(IPlatformService platformService, IMvxTextProviderBuilder textProviderBuilder , IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
            this.mTextProviderBuilder = textProviderBuilder;
        }

        #endregion

        #region Init

        public async void Init()
        {
            //HACK : await for the view to be set in ViewDidLoad (iOS) / OnCreate (Android)
            await System.Threading.Tasks.Task.Delay(200);

            if (View != null)
            {
                AppVersion = View.GetAppVersion();
                IsWazeInstalled = View.GetPreference<bool>(AppConstants.Waze);
                IsGoogleMapsInstalled = View.GetPreference<bool>(AppConstants.GoogleMaps);
                IsNavmiiInstalled = View.GetPreference<bool>(AppConstants.Navmii);
                Language = View.GetPreference<string>(AppConstants.Language);
                if (string.IsNullOrEmpty(Language))
                    Language = AppConstants.Languages[0];
            }
        }

        #endregion

        public IAppSettingsView View { get; set; }

        #region Properties

        #region IsLoading

        private bool mIsLoading = true;

        public bool IsLoading
        {
            get
            {
                return mIsLoading;
            }
            set
            {
                mIsLoading = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region AppVersion

        private string mAppVersion = string.Empty;

        public string AppVersion
        {
            get
            {
                return mAppVersion;
            }
            set
            {
                mAppVersion = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsWazeInstalled

        private bool mIsWazeInstalled = false;

        public bool IsWazeInstalled
        {
            get
            {
                return mIsWazeInstalled;
            }
            set
            {
                if (View.CheckIfAppInstalled(AppConstants.Waze))
                {
                    mIsWazeInstalled = value;
                }
                else
                {
                    if (!IsLoading)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, "Warning", string.Format("Looks like you have not installed Waze yet. Please go to {0} to install Waze, then try again.", mPlatformService.OS == OS.Touch ? "App Store" : "Google Play Store"), "Cancel", null,
                            new string[] { "Install Waze" },
                                () =>
                                {
                                    if (View != null)
                                        View.GotoStoreToInstallApp(AppConstants.Waze);
                                }));
                    }
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsGoogleMapsInstalled

        private bool mIsGoogleMapsInstalled = false;

        public bool IsGoogleMapsInstalled
        {
            get
            {
                return mIsGoogleMapsInstalled;
            }
            set
            {
                if (View.CheckIfAppInstalled(AppConstants.GoogleMaps))
                {
                    mIsGoogleMapsInstalled = value;
                }
                else
                {
                    if (!IsLoading)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, "Warning", string.Format("Looks like you have not installed Google Maps yet. Please go to {0} to install GoogleMaps, then try again.", mPlatformService.OS == OS.Touch ? "App Store" : "Google Play Store"), "Cancel", null,
                            new string[] { "Install Google Maps" },
                            () =>
                            {
                                if (View != null)
                                    View.GotoStoreToInstallApp(AppConstants.GoogleMaps);
                            }));
                    }
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsNavmiiInstalled

        private bool mIsNavmiiInstalled = false;

        public bool IsNavmiiInstalled
        {
            get
            {
                return mIsNavmiiInstalled;
            }
            set
            {
                if (View.CheckIfAppInstalled(AppConstants.Navmii))
                {
                    mIsNavmiiInstalled = value;
                }
                else
                {
                    if (!IsLoading)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, "Warning", string.Format("Looks like you have not installed Navmii yet. Please go to {0} to install Navmii, then try again.", mPlatformService.OS == OS.Touch ? "App Store" : "Google Play Store"), "Cancel", null,
                            new string[] { "Install Navmii" },
                                () =>
                                {
                                    if (View != null)
                                        View.GotoStoreToInstallApp(AppConstants.Navmii);
                                }));
                    }
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Languages

        private string[] mLanguages = AppConstants.Languages;

        public string[] Languages
        {
            get { return mLanguages; }
            set
            {
                mLanguages = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Language

        private string mLanguage = AppConstants.Languages[0];

        public string Language
        {
            get
            {
                return mLanguage;
            }
            set
            {
                mLanguage = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region ShowLanguagePickerCommand

        private MvxCommand mShowLanguagePickerCommand = null;

        public MvxCommand ShowLanguagePickerCommand
        {
            get
            {
                if (mShowLanguagePickerCommand == null)
                {
                    mShowLanguagePickerCommand = new MvxCommand(this.ShowLanguagePicker);
                }
                return mShowLanguagePickerCommand;
            }
        }

        private void ShowLanguagePicker()
        {
            if (View != null)
                View.ShowLanguagePicker();
        }

        #endregion

        private MvxCommand mSaveSettingsCommand = null;
        public MvxCommand SaveSettingsCommand
        {
            get
            {
                if (mSaveSettingsCommand == null)
                    mSaveSettingsCommand = new MvxCommand(this.SaveSettings);
                return mSaveSettingsCommand;
            }
        }

        private void SaveSettings()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
            //do update preferences
            if (View != null)
            {
                View.SetPreference<bool>(AppConstants.Waze, IsWazeInstalled);
                View.SetPreference<bool>(AppConstants.GoogleMaps, IsGoogleMapsInstalled);
                View.SetPreference<bool>(AppConstants.Navmii, IsNavmiiInstalled);
                View.SetPreference<string>(AppConstants.Language, Language.Equals(AppConstants.Languages[0]) ? string.Empty :  Language);
            }

            //reload language
            mTextProviderBuilder.LoadResources(Language.Equals(AppConstants.Languages[0]) ? string.Empty :  Language);
            RaisePropertyChanged(() => TextSource);
            RaisePropertyChanged(() => SharedTextSource);

            //for iOS, to automatically update any text when language changed
            Mvx.Resolve<IMvxMessenger>().Publish(new TextSourceMessage(this));

            //then close
            Close(this);
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
        }


        #endregion

        #region Methods

        private void PickLanguage(string whichLanguage)
        {
            mTextProviderBuilder.LoadResources(whichLanguage);
        }

        #endregion


    }
}

