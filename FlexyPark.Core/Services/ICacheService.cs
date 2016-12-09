using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Localization;
using FlexyPark.Core.Models;
using System.Dynamic;
using Cirrious.CrossCore;
using System.Reactive.Disposables;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.Core.Services
{
    public interface ICacheService
    {
        /// <summary>
        /// Store 'Adding a spot' status.
        /// </summary>
        /// <value>Next Status.</value>
        AddSpotStatus NextStatus { get; set; }

        /// <summary>
        /// Store 'Add Spot' Address.
        /// </summary>
        /// <value>The spot address.</value>
        string SpotAddress { get; set; }

        /// <summary>
        /// Store 'search mode' value to know user want to rent or reserve a parking spot.
        /// </summary>
        /// <value>The search mode.</value>
        SearchMode SearchMode { get; set; }

        bool NeedReloadEvent { get; set; }

        double CurrentLat{ get; set; }

        double CurrentLng{ get; set; }

        /// <summary>
        /// Gets or sets the text source (language binding manager).
        /// </summary>
        /// <value>The text source.</value>
        IMvxLanguageBinder TextSource { get; set; }

        IMvxLanguageBinder SharedTextSource { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>The current user.</value>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the current reservation.
        /// </summary>
        /// <value>The current reservation.</value>
        Reservation CurrentReservation { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        string SessionId { get; set; }

        List<Vehicle> UserVehicles { get; set; }

        Vehicle SelectedVehicle { get; set; }

        CreateParkingRequest CreateParkingRequest { get; set; }

        string ParkingId { get; set; }

        List<ExtendTimeItemViewModel> Extends { get; set; }

        double ExtendHours { get; set; }
    }

    public class CacheService : ICacheService
    {
        public List<ExtendTimeItemViewModel> Extends { get; set; }

        public double ExtendHours { get; set; }

        public string ParkingId { get; set; }

        public AddSpotStatus NextStatus { get; set; }

        public string SpotAddress { get; set; }

        public bool NeedReloadEvent { get; set; }

        public IMvxLanguageBinder TextSource { get; set; }

        public IMvxLanguageBinder SharedTextSource { get; set; }

        public User CurrentUser { get; set; }

        public Reservation CurrentReservation { get; set; }

        public string SessionId { get; set; }

        public List<Vehicle> UserVehicles { get; set; }

        public Vehicle SelectedVehicle { get; set; }

        private SearchMode mSearchMode = SearchMode.Now;

        public SearchMode SearchMode
        {
            get
            {
                return mSearchMode;
            }
            set
            {
                mSearchMode = value;
            }
        }

        public double CurrentLat{ get; set; }

        public double CurrentLng{ get; set; }

        private CreateParkingRequest mCreateParkingRequest = new CreateParkingRequest()
        {
            Context = "open",
            AvailableFromDate = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"), 
            AvailableToDate = DateTime.UtcNow.AddDays(365).ToString("yyyy-MM-dd hh:mm:ss"),
            NumberOfSpots = "1",
            AccessProcedure = "freeAccess",
            AccessKey = "noKey",
            GatePhoneNumber = "",
            CanRentWithoutOwnerConfirmation = "yes",
            MinimumTimeUnit = "hour",
            AmountOfMinimumTimeUnit = 1,
            Currency = "euro",
            HourlyRate = 1,
            WeeklyRate = 40,
            MonthlyRate = 100,
            Description = "balabal",
            VisiblePlateNumber = "1-ZXZZZ",
            Picture = "",
            Disabled = "no",
            DailyRate = 12
        };

        public CreateParkingRequest CreateParkingRequest
        {
            get { return mCreateParkingRequest; }
            set
            {
                mCreateParkingRequest = value;
            }
        }
        
    }
}
