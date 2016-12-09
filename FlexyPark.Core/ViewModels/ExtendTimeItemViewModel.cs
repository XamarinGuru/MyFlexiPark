using System;
using Cirrious.MvvmCross.ViewModels;

namespace FlexyPark.Core.ViewModels
{
    public class ExtendTimeItemViewModel : MvxViewModel
    {
        #region Constructors

        public ExtendTimeItemViewModel()
        {
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region Price

        private double mPrice = 0;

        public double Price
        {
            get
            {
                return mPrice;
            }
            set
            {
                mPrice = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Time

        private DateTime mTime = DateTime.UtcNow.ToLocalTime();

        public DateTime Time
        {
            get
            {
                return mTime;
            }
            set
            {
                mTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Hours

        private int mHours = 0;

        public int Hours
        {
            get
            {
                return mHours;
            }
            set
            {
                mHours = value;
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

