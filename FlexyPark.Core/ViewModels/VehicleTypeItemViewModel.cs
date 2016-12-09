using System;
using Cirrious.MvvmCross.ViewModels;

namespace FlexyPark.Core.ViewModels
{
    public class VehicleTypeItemViewModel : MvxViewModel
    {
        #region Constructors

        public VehicleTypeItemViewModel()
        {
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region IsChecked
       
        private bool mIsChecked = false;

        public bool IsChecked
        {
            get
            {
                return mIsChecked;
            }
            set
            {
                mIsChecked = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Type

        private string mType = string.Empty;

        public string Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
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

