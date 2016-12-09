using System;
using System.Collections.ObjectModel;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Localization;
using FlexyPark.Core.Services;
using Cirrious.CrossCore.Platform;

namespace FlexyPark.Core.ViewModels
{
    public interface IAddSpotView
    {
        void ReloadTasks();

        void GetGPS();

        void GetAccuracy();

        bool DetectLocationService();
    }

    public class AddSpotViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;
        private string ParkingId;

        #region Constructors

        public AddSpotViewModel(IApiService apiService, ICacheService cacheService, IPlatformService platformService)
            : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
        }

        #endregion

        #region Init

        public async void Init(bool isEditMode, string parameterObject)
        {
            Parking = !string.IsNullOrEmpty(parameterObject) ? Mvx.Resolve<IMvxJsonConverter>().DeserializeObject<OwnerParkingsResponse>(parameterObject) : null;
            if (mCacheService.CreateParkingRequest != null && mCacheService.CreateParkingRequest.HourlyRate != null && Parking != null)
            {
                mCacheService.CreateParkingRequest.HourlyRate = Parking.HourlyRate;
                mCacheService.ParkingId = Parking.ParkingId;
            }
            IsEditMode = isEditMode;

            if (isEditMode)
            {
                mCacheService.CurrentLat = Parking.Latitude;
                mCacheService.CurrentLng = Parking.Longitude;
            }

            await Task.Delay(200);

            Title = isEditMode ? mCacheService.TextSource.GetText("EditPageTitle") : mCacheService.TextSource.GetText("PageTitle");
            ButtonTitle = isEditMode ? "Save" : "Add";

            string[] TaskTitles = new string[]
            {
                TextSource.GetText("PleaseGoToText"),
                TextSource.GetText("GPSText"),
                TextSource.GetText("AccuracyText"),
                TextSource.GetText("PleaseSetAddressText"),
                TextSource.GetText("PleaseSetSizeText"),
                TextSource.GetText("PleaseSetCostText"),
                TextSource.GetText("PleaseSetCalendarText"),
                TextSource.GetText("ActivationText")
            };

            for (int i = 0; i < TaskTitles.Length; i++)
            {
                var title = TaskTitles[i];
                Tasks.Add(new TaskItemViewModel
                    {
                        Title = title,
                        Index = i,
                        Enabled = false,
                        Finished = false
                    });
            }

            //Tasks.FirstOrDefault(x => x.Index == (int)Status).Enabled = true;

            if (!IsEditMode)
            {
                if (mCacheService.NextStatus != AddSpotStatus.Complete)
                    Tasks.FirstOrDefault(x => x.Index == (int)mCacheService.NextStatus).Enabled = true;
                else
                    Tasks.FirstOrDefault(x => x.Index == (int)Status).Enabled = true;
            }
            else
            {
                foreach (var task in Tasks)
                {
                    task.Enabled = true;
                }
                mCacheService.CreateParkingRequest.Latitude = Parking.Latitude;
                mCacheService.CreateParkingRequest.Location = Parking.Location;
                mCacheService.CreateParkingRequest.Longitude = Parking.Longitude;
                //IsAddButtonEnabled = true;
            }
        }

        #endregion

        public IAddSpotView View { get; set; }

        #region Properties

        #region Parking

        private OwnerParkingsResponse mParking = null;

        public OwnerParkingsResponse Parking
        {
            get
            {
                return mParking;
            }
            set
            {
                mParking = value;
                RaisePropertyChanged();
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

        #region ButtonTitle

        private string mButtonTitle = string.Empty;

        public string ButtonTitle
        {
            get
            {
                return mButtonTitle;
            }
            set
            {
                mButtonTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsEditMode

        private bool mIsEditMode = false;

        public bool IsEditMode
        {
            get
            {
                return mIsEditMode;
            }
            set
            {
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Status

        private AddSpotStatus mStatus = AddSpotStatus.GotoSpot;

        public AddSpotStatus Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsAddButtonEnabled

        private bool mIsAddButtonEnabled = false;

        public bool IsAddButtonEnabled
        {
            get
            {
                return mIsAddButtonEnabled;
            }
            set
            {
                mIsAddButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Tasks

        private ObservableCollection<TaskItemViewModel> mTasks = new ObservableCollection<TaskItemViewModel>();

        public ObservableCollection<TaskItemViewModel> Tasks
        {
            get
            {
                return mTasks;
            }
            set
            {
                mTasks = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region ChooseTaskCommand

        private MvxCommand<TaskItemViewModel> mChooseTaskCommand = null;

        public MvxCommand<TaskItemViewModel> ChooseTaskCommand
        {
            get
            {
                if (mChooseTaskCommand == null)
                {
                    mChooseTaskCommand = new MvxCommand<TaskItemViewModel>(this.ChooseTask);
                }
                return mChooseTaskCommand;
            }
        }

        private void ChooseTask(TaskItemViewModel itemVM)
        {
            //get gps and accuracy
            if (itemVM.Index == 1 /*|| itemVM.Index == 2*/)
                return;

            switch (itemVM.Index)
            {
                case 0:
                    ShowViewModel<GotoSpotViewModel>();
                    break;
                case 2:
                    ShowViewModel<AddSpotAccuracyViewModel>();
                    break;
                case 3:
                    ShowViewModel<AddSpotAddressViewModel>(new {location = Parking != null ? Parking.Location : string.Empty});
                    break;
                case 4:
                    if (IsEditMode)
                        ShowViewModel<AddSpotSizeViewModel>(new {type = Parking.VehicleType});
                    else
                        ShowViewModel<AddSpotSizeViewModel>();
                    break;
                case 5:
                    {
                        if (IsEditMode)
                            ShowViewModel<AddSpotCostViewModel>(new {hourlyRate = Parking.HourlyRate, lat = Parking.Latitude, lng = Parking.Longitude});
                        else
                            ShowViewModel<AddSpotCostViewModel>();
                        break; 
                    }
                case 6:
                    {
                        if (IsEditMode)
                            ShowViewModel<AddSpotCalendarViewModel>(new {parkingId = Parking.ParkingId});
                        else
                            ShowViewModel<AddSpotCalendarViewModel>();
                        break; 
                    }
                case 7:
                    {
                        if (IsEditMode)
                            ShowViewModel<AddSpotStatusViewModel>(new {disabled = Parking.Disabled});
                        else
                            ShowViewModel<AddSpotStatusViewModel>(new {disabled = true});
                        break; 
                    }
                default:
                    break;
            }


        }

        #endregion

        #region AddSpotCommand

        private MvxCommand mAddSpotCommand = null;

        public MvxCommand AddSpotCommand
        {
            get
            {
                if (mAddSpotCommand == null)
                {
                    mAddSpotCommand = new MvxCommand(this.AddSpot);
                }
                return mAddSpotCommand;
            }
        }

        private async void AddSpot()
        {
            var taskLeft = Tasks.FirstOrDefault(x => x.Finished == false);
            if (taskLeft == null)
            {
                //call api
                if (BaseView != null && BaseView.CheckInternetConnection())
                {
                    if (IsEditMode)
                    {
                        var result = await mApiService.PutParking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, Parking.ParkingId);
                        if (result != null)
                        {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("Error {0}", result.Response.ErrorCode)));
                            if (result.Response.Status.Equals("success"))
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                            }
                            else
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("Error : {0}", result.Response.ErrorCode), "Ok", null));
                            }
                            Close(this);
                        }
                    }
                    else
                    {
                        //add new parking spot
                        var result = await mApiService.CreateParking(Mvx.Resolve<ICacheService>().CurrentUser.UserId);
                        if (result.Response != null)
                        {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("Error: {0}", result.Response.ErrorCode)));
							if (result.Response.Status.Equals("success"))
                            {
                                IsEditMode = true;
                                if (Parking == null)
                                    Parking = new OwnerParkingsResponse();
                                Parking.ParkingId = result.Response.ParkingId;

                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                            }
                            else
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                            }
                            Close(this);
                        }
                    }


                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                }

            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, string.Format("{0} {1}", taskLeft.Title, TextSource.GetText("HasNotBeenDoneText"))));
            }
        }

        #endregion

        #endregion

        #region Methods

        #region UpdateTasksStatus

        public async void UpdateTasksStatus(AddSpotStatus newStatus)
        {
            if (IsEditMode)
                return;
            
            if (newStatus == AddSpotStatus.GotoSpot)
                return;

            if ((int)Status < (int)newStatus)
            {
                Tasks.FirstOrDefault(x => x.Index == (int)Status).Finished = true;
            
                //update status
                Status = newStatus;
            }
            else
                return;

            foreach (var task in Tasks)
            {
                //task.Enabled = false;
                task.IsShowPleaseWait = false;
            }

            if (newStatus == AddSpotStatus.Complete)
            {
                IsAddButtonEnabled = true;
                if (View != null)
                    View.ReloadTasks();
                return;
            }
            else
            {
                Tasks.FirstOrDefault(x => x.Index == (int)Status).Enabled = true;
                if (Status == AddSpotStatus.GPS /*|| Status == AddSpotStatus.Accuracy*/)
                {
                    Tasks.FirstOrDefault(x => x.Index == (int)Status).IsShowPleaseWait = true;
                  
                }
            }

            if (View != null)
                View.ReloadTasks();

            if (View != null && Status == AddSpotStatus.GPS)
            {
                if (View.DetectLocationService())
                {
                    // Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                    View.GetGPS();
                    return;
                }
                else
                {
                    /*Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, "Error", "Location service is not avaiable. Please turn on Location Service and Try again.", "Try again", () =>
                            {
                                View.GetGPS();
                            }));*/
                    return;
                }
            }

            if (View != null && Status == AddSpotStatus.Accuracy)
            {
                //temporary comment out for ios testing
                //if(mPlatformService.OS == OS.Droid)
                //View.GetAccuracy();

                //await Task.Delay(200);
                //ShowViewModel<AddSpotAccuracyViewModel>();

                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                return;
            }
        }

        #endregion

        #region Close View Model

        public void CloseViewModel()
        {
            Close(this);
        }

        public async void DoAddOrSaveParkingSpot()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                if (IsEditMode)
                {
                    var result = await mApiService.PutParking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, Parking.ParkingId);
                    if (result != null)
                    {
                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("Error {0}", result.Response.ErrorCode)));
                        if (result.Response.Status.Equals("success"))
                        {
                            Parking.Disabled = result.Request.Disabled;
                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                        }
                        else
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("Error : {0}", result.Response.ErrorCode), "Ok", null));
                        }
                    }
                }
                else
                {
                    //add new parking spot
                    var result = await mApiService.CreateParking(Mvx.Resolve<ICacheService>().CurrentUser.UserId);
                    if (result != null)
                    {
                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("Error: {0} ", result.Response.ErrorCode)));
                        if (result.Response.Status.Equals("success"))
                        {
                            Mvx.Resolve<ICacheService>().ParkingId = result.Response.ParkingId;
                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                        }
                        else
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("Error: {0}", result.Response.ErrorCode), "Ok", null));
                        }
                    }
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
        }

        public async void SaveParkingSpot()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                var result = await mApiService.PutParking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, Mvx.Resolve<ICacheService>().ParkingId);
                if (result != null)
                {
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Result.Equals("success") ? "Success" : string.Format("Error {0}", result.Response.ErrorCode)));
                    if (result.Response.Status.Equals("success"))
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("Error : {0}", result.Response.ErrorCode), "Ok", null));
                    }
                }

                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
        }

        #endregion

        #endregion
    }
}

