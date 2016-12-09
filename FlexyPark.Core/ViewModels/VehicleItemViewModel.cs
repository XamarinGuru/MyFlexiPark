using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public class VehicleItemViewModel : MvxViewModel
    {
        private readonly ChooseVehicleViewModel mParentVM;

        #region Constructors

        public VehicleItemViewModel(ChooseVehicleViewModel parentVM)
        {
            this.mParentVM = parentVM;
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region IsEditMode
        private bool mIsEditMode = false;
        public bool IsEditMode
        {
            get{
                return mIsEditMode;
            }
            set{
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Vehicle

        private Vehicle mVehicle = new Vehicle();

        public Vehicle Vehicle
        {
            get
            {
                return mVehicle;
            }
            set
            {
                mVehicle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Checked


        private bool mChecked;

        public bool Checked
        {
            get
            {
                return mChecked;
            }
            set
            {
                mChecked = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoEditVehicleCommand

        private MvxCommand mGotoEditVehicleCommand = null;

        public MvxCommand GotoEditVehicleCommand
        {
            get
            {
                if (mGotoEditVehicleCommand == null)
                {
                    mGotoEditVehicleCommand = new MvxCommand(this.GotoEditVehicle);
                }
                return mGotoEditVehicleCommand;
            }
        }

        private void GotoEditVehicle()
        {
            ShowViewModel<AddVehicleViewModel>(new {isEditMode = true, vehicleId = Vehicle.VehicleId, plateNumber =  Vehicle.PlateNumber, type = Vehicle.Type});
        }

        #endregion

        #region DeleteVehicleCommand

        private MvxCommand mDeleteVehicleCommand = null;

        public MvxCommand DeleteVehicleCommand
        {
            get
            {
                if (mDeleteVehicleCommand == null)
                {
                    mDeleteVehicleCommand = new MvxCommand(this.DeleteVehicle);
                }
                return mDeleteVehicleCommand;
            }
        }

        private void DeleteVehicle()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(mParentVM, mParentVM.SharedTextSource.GetText("WarningText"), mParentVM.SharedTextSource.GetText("DeleteVehicleText"), mParentVM.SharedTextSource.GetText("NoText"), null,
                new string[] {mParentVM.SharedTextSource.GetText("YesText")},
                ()=>{
                //delete vehicle
                mParentVM.DeleteVehicle(Vehicle.VehicleId);
            }));
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

