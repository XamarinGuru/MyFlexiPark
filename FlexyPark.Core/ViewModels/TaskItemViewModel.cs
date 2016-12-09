using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class TaskItemViewModel : MvxViewModel
    {
        #region Constructors

        public TaskItemViewModel()
        {
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
                if (mTextSource == null)
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

        #region Index

        private int mIndex = 0;

        public int Index
        {
            get
            {
                return mIndex;
            }
            set
            {
                mIndex = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowPleaseWait

        private bool mIsShowPleaseWait = false;

        public bool IsShowPleaseWait
        {
            get
            {
                return mIsShowPleaseWait;
            }
            set
            {
                mIsShowPleaseWait = value;
              
                RaisePropertyChanged();
                if (mIsShowPleaseWait) IsShowArrow = false;
                RaisePropertyChanged("IsShowArrow");
            }
        }

        #endregion

        #region Enabled

        private bool mEnabled = false;

        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
                RaisePropertyChanged();
                IsShowArrow = (value & !Finished);
                RaisePropertyChanged("IsShowArrow");

            }
        }

        #endregion

        #region Title

        private string mTitle = string.Empty;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Finished

        private bool mFinished = false;

        public bool Finished
        {
            get
            {
                return mFinished;
            }
            set
            {
                mFinished = value;
                RaisePropertyChanged();
                IsShowArrow = (!value & Enabled);
                RaisePropertyChanged("IsShowArrow");
            }
        }

        #endregion

        #region IsShowArrow


        private bool mIsShowArrow = false;

        public bool IsShowArrow
        {
            get
            {
                return mIsShowArrow;
            }
            set
            {
                mIsShowArrow = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Methods

        #endregion
    }
}

