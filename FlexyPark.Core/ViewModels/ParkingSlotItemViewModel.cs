using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class ParkingSlotItemViewModel : MvxViewModel
    {
        #region Constructors

        public ParkingSlotItemViewModel()
        {
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region IsShowClock

        private bool mIsShowClock = false;

        public bool IsShowClock
        {
            get
            {
                return mIsShowClock;
            }
            set
            {
                mIsShowClock = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ParkingSpot

        private ParkingSpot mParkingSpot = new ParkingSpot();

        public ParkingSpot ParkingSpot
        {
            get
            {
                return mParkingSpot;
            }
            set
            {
                mParkingSpot = value;
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

