using System;
using System.Threading.Tasks;
using Flurl.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Flurl;
using System.ServiceModel.Description;
using FlexyPark.Core.Models;
using System.Dynamic;
using System.Diagnostics;
using Cirrious.CrossCore;
using System.Net.Http;
using System.Net;
using Cirrious.CrossCore.Platform;
using Akavache.Sqlite3.Internal;
using FlexyPark.UI.Touch.Extensions;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;

namespace FlexyPark.Core.Services
{
    public interface IApiService
    {
        //F0
        Task<LoginResponse> Login(string email, string password, string token);

        Task<ApiResult<RegisterRequest, RegisterResponse>> LostPassword (string email, string deviceIdentifier, string token);

        //F1
        Task<ApiResult<RegisterRequest, RegisterResponse>> PersonRegister(string email, string password, string firstname, string lastname, string phonenumber, string token, string deviceId);

        Task<ApiResult<PersonGetRequest, User>> PersonGet(string userId);

        Task<ApiResult<PersonGetRequest, PersonGetCardResponse>> PersonGetCard(string userId);

		Task<ApiResult<PersonGetRequest, PersonGetBankResponse>> PersonGetBank(string userId);

		Task<ApiResult<PutUserRequest, ApiCommonResponse>> PersonPut(string userId, string firstName, string lastName, string email, string password, string phoneNumber, string facebookId, string street, string postalCode, string city, string country, string birthday);
		Task<ApiResult<PutUserRequest, ApiCommonResponse>> PersonPut(User user, string birthday, string street, string city, string postalCode, string country = "BE");

		Task<ApiResult<AddCardRequest, ApiCommonResponse>> AddCard(string userId, string tokenId);

		Task<bool> RemoveCard(string userId);

		Task<ApiResult<AddBankRequest, ApiCommonResponse>> AddBank(string userId, string tokenId);

        //F2
        Task<ApiResult<GetVehicleListRequest, List<Vehicle>>> GetVehicles(string userId);

        Task<ApiResult<CreateVehicleRequest, CreateVehicleResponse>> CreateVehicle(string userId, string type, string plateNumber);

        Task<ApiResult<PutVehicleRequest, ApiCommonResponse>> PutVehicle(string userId, string vehicleId, string type, string plateNumber);

        Task<bool> DeleteVehicle(string userId, string vehicleId);

        //F3
        Task<ApiResult<AvaiableParkingsRequest, List<ParkingSpot>>> AvaiableParkings(long startTimeStamp, long endTimestamp, int maxWaitingDuration, double gpsLatitude, double gpsLongitude, string maxDistance, string carSize);

        Task<ApiResult<AvaiableParkingsRequest, List<ParkingSpot>>> AvaiableParkingsExtensions(long startTimeStamp, long endTimestamp, int maxWaitingDuration, double gpsLatitude, double gpsLongitude, string maxDistance, string carSize);

		Task<ApiResult<AvaiableParkingsRequest, List<Parking>>> AvaiableParkingsInMap(double gpsLatitude, double gpsLongitude, double maxDistance, int start);

        //F4
        Task<ApiResult<RenterBookingRequest, List<Reservation>>> RenterBookings(string userId, long startTimestamp, string sorting, int? limit, int? start);

        Task<ApiResult<CreateBookingRequest, CreateBookingResponse>> CreateBooking(string userId, string parkingId, long startTimestamp, long endTimestamp, string renterCarPlateNumber);

        //Task<CreateBookingPacket> CreateBookingTest(string userId, string parkingId, long startTimestamp, long endTimestamp, string renterCarPlateNumber);

        Task<ApiResult<PayBookingRequest, PayBookingResponse>> PayBooking(string userId, string parkingId, string bookingId);

        Task<ApiResult<LeaveBookingRequest, ApiCommonResponse>> LeaveBooking(string userId, string parkingId, string bookingId, string rating, string review, string leavingStatus);

        //F5
        Task<ApiResult<OwnerParkingsRequest, List<OwnerParkingsResponse>>> OwnerParkings(string userId);

        Task<ApiResult<CreateParkingRequest, CreateParkingResponse>> CreateParking(string userId);

        Task<ApiResult<CreateParkingRequest, ApiCommonResponse>> PutParking(string userId, string parkingId);

        //F6
        Task<ApiResult<CreateBookingOwnerRequest, List<Reservation>>> OwnerBookings(string userId, string parkingId, long startTimestamp, long endTimestamp, string sorting);

        Task<ApiResult<OwnerUnavailabilityRequest, List<OwnerUnavailability>>> OwnerUnavailabilities(string userId, string parkingId, long startTimestamp, long endTimestamp, string sorting);

        Task<ApiResult<CreateUnavailabilitiesRequest, CreateUnavailabilitiesResponse>> CreateUnavailabilities(string userId, string parkingId, string title, long startTimestamp, long endTimestamp, Periodicity peridocity, string numberOfSpots);

        Task<ApiResult<PutUnavailabilitiesRequest, PutUnavailabilitiesResponse>> PutUnavailabilities(string userId, string parkingId, string unavailabilityId, string title, long startTimestamp, long endTimestamp, Periodicity peridocity, string numberOfSpots, bool isOnlyEvent, long startTimestampOfSelectedOccurrence);

        Task<bool> DeleteUnavailabilities(string userId, string parkingId, string unavailabilityId);

        //F7
        Task<RecommendPriceResponse> RecommendPrice(double lat, double lng);
        //F8
        Task<AddressResponse> AddressOf(double lat, double lng);
        //F9
        Task<ShortestDistanceResponse> ShortestDistance(double fromLat, double fromLng, double toLat, double toLng);

        //Check places
        Task<HereMapResponseWrapper> CheckPlaces(string street, double lat, double lng);


        //Verify Receipt ( IAP )
        Task<AppleVerifyReceiptResponse> AppleVerifyReceipt(string receiptEncoded);

        //Topup (update user credits)
        Task<ApiResult<TopupRequest, TopupResponse>> UpdateUserCredits(string userId, string store, double amount, string transactionReceiptEncoded);

        //Topup (update user credits)
        Task<ApiResult<TopupPaymentRequest, TopupResponse>> UpdateUserCreditsViaPayment (string userId, string cardToken, double amount);

        //F12
        Task<GetProductsResponseWrapper> GetProductIds(string store);
    }

    public class ApiService : IApiService
    {
        public async Task<LoginResponse> Login(string email, string password, string token)
        {
            //            return await ApiUrls.Login.SetQueryParams(
            //                new{
            //                email = email,
            //                password = password,
            //                token = token
            //            }).ToString().GetJsonAsyncHandler<LoginResponse>();
            try
            {
                LoginRequest login = new LoginRequest()
                {
                    Email = email,
                    Password = password,
                    Token = token
                };
                Debug.WriteLine(ApiUrls.Login);
                Debug.WriteLine(login);
				Debug.WriteLine("http method: POST");
                return await ApiUrls.Login.PostJsonAsync(login).ReceiveJson<LoginResponse>();
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
				if (ex.Call.ErrorResponseBody != null)
				{
					var error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
					Debug.WriteLine(error.Error);
					if (error.Error.Equals("unKnownToken") || error.Error.Equals("unknownToken")) {
                        var response = new LoginResponse ();
                        response.ErrorCode = @"888";
						response.Status = @"failed";
                        return response;
                    }
                }

            }
            catch (Exception e)
            {
                int i = 0;
            }

            return null;
        }

        public async Task<ApiResult<RegisterRequest, RegisterResponse>> LostPassword (string email, string deviceIdentifier, string token)
        {
            return await ApiUrls.LostPassword.PostJsonAsyncHandler<RegisterRequest, RegisterResponse> (new RegisterRequest () {
                Email = email,
                Token = token,
                DeviceIdentifier = deviceIdentifier
            });

        }

        public async Task<ApiResult<RegisterRequest, RegisterResponse>> PersonRegister(string email, string password, string firstname, string lastname, string phonenumber, string token, string deviceId)
        {
            var result = await ApiUrls.PersonRegister.PostJsonAsyncHandler<RegisterRequest, RegisterResponse>(new RegisterRequest()
                {
                    Email = email,
                    Password = password,
                    Token = token,
                    DeviceIdentifier = deviceId,
                    FirstName = firstname,
                    LastName = lastname,
                    PhoneNumber = phonenumber
                });

            return result;
        }

        public async Task<ApiResult<PersonGetRequest, User>> PersonGet(string userId)
        {
            return await ApiUrls.BuildPersonUrl(userId).GetJsonAsyncHandlerWithSessionId<PersonGetRequest, User>();
        }

		public async Task<ApiResult<PersonGetRequest, PersonGetCardResponse>> PersonGetCard(string userId)
		{
			return await ApiUrls.BuildPersonCardUrl(userId).GetJsonAsyncHandlerWithSessionId<PersonGetRequest, PersonGetCardResponse>();
		}

		public async Task<ApiResult<PersonGetRequest, PersonGetBankResponse>> PersonGetBank(string userId)
		{
			return await ApiUrls.BuildPersonBankUrl(userId).GetJsonAsyncHandlerWithSessionId<PersonGetRequest, PersonGetBankResponse>();
		}

        public async Task<ApiResult<PutUserRequest, ApiCommonResponse>> PersonPut(string userId, string firstName, string lastName, string email, string password, string phoneNumber, string facebookId, string street, string postalCode, string city, string country, string birthday)
        {
			return await ApiUrls.BuildPersonUrl(userId).PutJsonAsyncHandler<PutUserRequest, ApiCommonResponse>(new PutUserRequest()
			{
				SessionId = Mvx.Resolve<ICacheService>().SessionId,
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				Password = password,
				PhoneNumber = phoneNumber,
				FacebookId = facebookId,
				Birthday = birthday,
				Street = street,
				City = city,
				PostalCode = postalCode,
				Country = country
			});
        }
		public async Task<ApiResult<PutUserRequest, ApiCommonResponse>> PersonPut(User user, string birthday, string street, string city, string postalCode, string country = "BE")
		{
			return await ApiUrls.BuildPersonUrl(user.UserId).PutJsonAsyncHandler<PutUserRequest, ApiCommonResponse>(new PutUserRequest()
			{
				SessionId = Mvx.Resolve<ICacheService>().SessionId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Password = user.Password,
				PhoneNumber = user.PhoneNumber,
				FacebookId = user.FacebookId,
				Birthday = birthday,
				Street = street,
				City = city,
				PostalCode = postalCode,
				Country = country
			});
		}

		public async Task<ApiResult<AddCardRequest, ApiCommonResponse>> AddCard(string userId, string tokenId)
		{
			return await ApiUrls.BuildPersonPaymentUrl(userId).PostJsonAsyncHandler<AddCardRequest, ApiCommonResponse>(new AddCardRequest()
			{
				SessionId = Mvx.Resolve<ICacheService>().SessionId,
				TokenId = tokenId
			});
		}

		public async Task<bool> RemoveCard(string userId)
		{
			return await ApiUrls.BuildPersonPaymentUrl(userId).DeleteJsonAsyncHandlerWithSessionId<DeleteVehicleRequest, ApiCommonResponse>();
		}

		public async Task<ApiResult<AddBankRequest, ApiCommonResponse>> AddBank(string userId, string tokenId)
		{
			return await ApiUrls.BuildPersonBankAccountUrl(userId).PostJsonAsyncHandler<AddBankRequest, ApiCommonResponse>(new AddBankRequest()
			{
				SessionId = Mvx.Resolve<ICacheService>().SessionId,
				BankAccountToken = tokenId
			});
		}

        public async Task<ApiResult<GetVehicleListRequest, List<Vehicle>>> GetVehicles(string userId)
        {
            Debug.WriteLine("Call get vehicles");
            return await ApiUrls.BuildVehicleUrl(userId).GetJsonAsyncHandlerWithSessionId<GetVehicleListRequest, List<Vehicle>>("userVehicles");
        }

        public async Task<ApiResult<CreateVehicleRequest, CreateVehicleResponse>> CreateVehicle(string userId, string type, string plateNumber)
        {
            return await ApiUrls.BuildVehicleUrl(userId).PostJsonAsyncHandler<CreateVehicleRequest, CreateVehicleResponse>(new CreateVehicleRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    Type = type,
                    PlateNumber = plateNumber
                });
        }

        public async Task<ApiResult<PutVehicleRequest, ApiCommonResponse>> PutVehicle(string userId, string vehicleId, string type, string plateNumber)
        {
            return await ApiUrls.BuildVehicleUrl(userId, vehicleId).PutJsonAsyncHandler<PutVehicleRequest, ApiCommonResponse>(new PutVehicleRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    Type = type,
                    PlateNumber = plateNumber
                });
        }

        public async Task<bool> DeleteVehicle(string userId, string vehicleId)
        {
            return await ApiUrls.BuildVehicleUrl(userId, vehicleId).DeleteJsonAsyncHandlerWithSessionId<DeleteVehicleRequest, ApiCommonResponse>();
        }

        public async Task<ApiResult<RenterBookingRequest, List<Reservation>>> RenterBookings(string userId, long startTimestamp, string sorting, int? limit, int? start)
        {
            //Vuong's Code
            return await ApiUrls.BuildBookingUrl(userId, "", "").SetQueryParams(new RenterBookingRequest
                {
                    startTimestamp = startTimestamp
                }).ToString().GetJsonAsyncHandlerWithSessionId<RenterBookingRequest, List<Reservation>>("bookings");

            //            return await ApiUrls.BookingListRenters.SetQueryParams(new  RenterBookingRequest
            //                {
            //                    UserId = userId,
            //                    StartTimestamp = startTimestamp,
            //                    Sort = sorting,
            //                    Limit = limit,
            //                    Start = start
            //                }).ToString().GetJsonAsyncHandlerWithSessionId<RenterBookingRequest,List<Reservation>>("bookings");

            //            var url = ApiUrls.BuildBookingUrl(userId, "").SetQueryParams(new RenterBookingRequest
            //                {
            //                    UserId = userId,
            //                    StartTimestamp = startTimestamp,
            //                    Sort = sorting,
            //                    Limit = limit,
            //                    Start = start
            //                });
            //
            //
            //            var data = await url.ToString().GetJsonAsyncHandlerWithSessionId<RenterBookingRequest,List<Reservation>>("bookings");
            //            return data;



        }

        public async Task<ApiResult<CreateBookingRequest, CreateBookingResponse>> CreateBooking(string userId, string parkingId, long startTimestamp, long endTimestamp, string renterCarPlateNumber)
        {
            //            return await ApiUrls.BookingCreate.PostJsonAsyncFake<CreateBookingRequest, CreateBookingResponse>(new CreateBookingRequest()
            //                {
            //                    UserId = userId,
            //                    ParkingId = parkingId,
            //                    StartTimestamp = startTimestamp.ToString(),
            //                    EndTimestamp = endTimestamp.ToString(),
            //                    RenterCarPlateNumber = renterCarPlateNumber
            //                });

            //Vuong's Code
            return await ApiUrls.BuildBookingUrl(userId, parkingId).PostJsonAsyncHandler<CreateBookingRequest, CreateBookingResponse>(new CreateBookingRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    ParkingId = parkingId,
                    StartTimestamp = startTimestamp.ToString(),
                    EndTimestamp = endTimestamp.ToString(),
                    RenterCarPlateNumber = renterCarPlateNumber
                });
        }

        public async Task<ApiResult<PayBookingRequest, PayBookingResponse>> PayBooking(string userId, string parkingId, string bookingId)
        {
            return await ApiUrls.BuildBookingUrl(userId, parkingId, bookingId).PatchUrlEncodedAsyncHandler<PayBookingRequest, PayBookingResponse>(new PayBookingRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    operations = ApiUrls.PayBookingOperations,
                    data = ApiUrls.PayBookingData
                });
        }

        public async Task<ApiResult<LeaveBookingRequest, ApiCommonResponse>> LeaveBooking(string userId, string parkingId, string bookingId, string rating, string review, string leavingStatus)
        {
            return await ApiUrls.BuildBookingUrl(userId, parkingId, bookingId).PatchUrlEncodedAsyncHandler<LeaveBookingRequest, ApiCommonResponse>(new LeaveBookingRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    operations = ApiUrls.BuildLeaveParkingOperations(leavingStatus, rating, review),
                    data = ApiUrls.LeaveParkingData
                });
        }

        public async Task<ApiResult<OwnerParkingsRequest, List<OwnerParkingsResponse>>> OwnerParkings(string userId)
        {
            //return await ApiUrls.ParkingList.GetJsonAsyncFake<List<OwnerParkingsResponse>>("ownerParkings");
            //Vuong's Code
            return await ApiUrls.BuildParking(userId, "").ToString().GetJsonAsyncHandlerWithSessionId<OwnerParkingsRequest, List<OwnerParkingsResponse>>("ownerParkings");
        }

        public async Task<ApiResult<CreateParkingRequest, CreateParkingResponse>> CreateParking(string userId)
        {
            //            return await ApiUrls.ParkingCreate.PostJsonAsyncFake<CreateParkingRequest,CreateParkingResponse>(new CreateParkingRequest()
            //                {
            //                    UserId = userId,
            //                    Latitude = latitude,
            //                    Longitude = longitude,
            //                    GpsAccuracy = gpsAccuracy,
            //                    Location = location,
            //                    AvailableFromDate = avaiableFromDate,
            //                    AvailableToDate = avaiableToDate,
            //                    Availability = availability,
            //                    CarType = carType,
            //                    NumberOfSpots = numberOfSpots,
            //                    Context = context,
            //                    AccessProcedure = accessProcedure,
            //                    AccessKey = accessKey,
            //                    GatePhoneNumber = gatePhoneNumber,
            //                    CanRentWithoutOwnerConfirmation = canRentWithoutOwnerConfirmation,
            //                    MinimumTimeUnit = minimumTimeUnit,
            //                    AmountOfMinimumTimeUnit = amountOfMinimumTimeUnit,
            //                    Currency = currency,
            //                    HourlyRate = hourlyRate,
            //                    DailyRate = dailyRate,
            //                    WeeklyRate = weeklyRate,
            //                    MonthlyRate = monthlyRate,
            //                    Description = description,
            //                    VisiblePlateNumber = visiblePlateNumber,
            //                    Picture = picture,
            //                    Disabled = disabled
            //                });

            //Vuong's Code

            //            return await ApiUrls.BuildParking(userId, "").PostJsonAsyncHandler<CreateParkingRequest,CreateParkingResponse>(new CreateParkingRequest()
            //                {
            //                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
            //                    Latitude = latitude,
            //                    Longitude = longitude,
            //                    GpsAccuracy = gpsAccuracy,
            //                    Location = location,
            //                    AvailableFromDate = avaiableFromDate,
            //                    AvailableToDate = avaiableToDate,
            //                    Availability = availability,
            //                    CarType = carType,
            //                    NumberOfSpots = numberOfSpots,
            //                    Context = context,
            //                    AccessProcedure = accessProcedure,
            //                    AccessKey = accessKey,
            //                    GatePhoneNumber = gatePhoneNumber,
            //                    CanRentWithoutOwnerConfirmation = canRentWithoutOwnerConfirmation,
            //                    MinimumTimeUnit = minimumTimeUnit,
            //                    AmountOfMinimumTimeUnit = amountOfMinimumTimeUnit,
            //                    Currency = currency,
            //                    HourlyRate = hourlyRate,
            //                    DailyRate = dailyRate,
            //                    WeeklyRate = weeklyRate,
            //                    MonthlyRate = monthlyRate,
            //                    Description = description,
            //                    VisiblePlateNumber = visiblePlateNumber,
            //                    Picture = picture,
            //                    Disabled = disabled
            //                });

            var request = Mvx.Resolve<ICacheService>().CreateParkingRequest;
            request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
            return await ApiUrls.BuildParking(userId, string.Empty).PostJsonAsyncHandler<CreateParkingRequest, CreateParkingResponse>(request);
        }

        public async Task<ApiResult<CreateParkingRequest, ApiCommonResponse>> PutParking(string userId, string parkingId)
        {
            //            return await ApiUrls.ParkingPut.PostJsonAsyncFake<PutParkingRequest,ApiCommonResponse>(new PutParkingRequest()
            //                {
            //                    UserId = userId,
            //                    ParkingId = parkingId,
            //                    Latitude = latitude,
            //                    Longitude = longitude,
            //                    GpsAccuracy = gpsAccuracy,
            //                    Location = location,
            //                    AvaiableFromDate = avaiableFromDate,
            //                    AvaiableToDate = avaiableToDate,
            //                    Availability = availability,
            //                    CarType = carType,
            //                    NumberOfSpots = numberOfSpots,
            //                    Context = context,
            //                    AccessProcedure = accessProcedure,
            //                    AccessKey = accessKey,
            //                    GatePhoneNumber = gatePhoneNumber,
            //                    CanRentWithoutOwnerConfirmation = canRentWithoutOwnerConfirmation,
            //                    MinimumTimeUnit = minimumTimeUnit,
            //                    AmountOfMinimumTimeUnit = amountOfMinimumTimeUnit,
            //                    Currency = currency,
            //                    HourlyRate = hourlyRate,
            //                    DailyRate = dailyRate,
            //                    WeeklyRate = weeklyRate,
            //                    MonthlyRate = monthlyRate,
            //                    Description = description,
            //                    VisiblePlateNumber = visiblePlateNumber,
            //                    Picture = picture,
            //                    Disabled = disabled
            //                }); 
            var request = Mvx.Resolve<ICacheService>().CreateParkingRequest;
            request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
            return await ApiUrls.BuildParking(userId, parkingId).PutJsonAsyncHandler<CreateParkingRequest, ApiCommonResponse>(request);
        }

        public async Task<ApiResult<AvaiableParkingsRequest, List<ParkingSpot>>> AvaiableParkings(long startTimeStamp, long endTimestamp, int maxWaitingDuration, double gpsLatitude, double gpsLongitude, string maxDistance, string carSize)
        {
            var url = ApiUrls.AvaiableParkings.SetQueryParams(
                          new
						  {
							  startTimestamp = startTimeStamp.ToString(),
							  endTimestamp = endTimestamp.ToString(),
							  sorting = "distance,waiting,price,rating",
							  maxWaitingDuration = maxWaitingDuration.ToString(),
							  gpsLatitude = gpsLatitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  gpsLongitude = gpsLongitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  maxDistance = maxDistance,
							  carSize = carSize
						  });
			//var requestParkingList = url.ToString();
            return await url.ToString().GetJsonAsyncHandlerWithSessionId<AvaiableParkingsRequest, List<ParkingSpot>>("availableParkings");
        }

        public async Task<ApiResult<AvaiableParkingsRequest, List<ParkingSpot>>> AvaiableParkingsExtensions(long startTimeStamp, long endTimestamp, int maxWaitingDuration, double gpsLatitude, double gpsLongitude, string maxDistance, string carSize)
        {
            var url = ApiUrls.AvaiableParkings.SetQueryParams(
                          new
						  {
							  startTimestamp = startTimeStamp.ToString(),
							  endTimestamp = endTimestamp.ToString(),
							  maxWaitingDuration = maxWaitingDuration.ToString(),
							  gpsLatitude = gpsLatitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  gpsLongitude = gpsLongitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  maxDistance = maxDistance,
							  carSize = carSize
						  });

            return await url.ToString().GetJsonAsyncHandlerWithSessionId<AvaiableParkingsRequest, List<ParkingSpot>>("availableParkings");
        }

		public async Task<ApiResult<AvaiableParkingsRequest, List<Parking>>> AvaiableParkingsInMap(double gpsLatitude, double gpsLongitude, double maxDistance, int start)
		{
			var url = ApiUrls.AvaiableParkings.SetQueryParams(
						  new
						  {
							  	gpsLatitude = gpsLatitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  	gpsLongitude = gpsLongitude.ParseToCultureInfo(new CultureInfo("en-US")),
							  	maxDistance = maxDistance,
								limit = AppConstants.LimitPerPage,
								start = start
						  });

			return await url.ToString().GetJsonAsyncHandlerWithSessionId<AvaiableParkingsRequest, List<Parking>>("availableParkings");
		}

        public async Task<ApiResult<CreateBookingOwnerRequest, List<Reservation>>> OwnerBookings(string userId, string parkingId, long starttimestamp, long endtimestamp, string sorTing)
        {
            //return await ApiUrls.BookingListOwners.GetJsonAsyncFake < List<OwnerBooking> >("bookings");

            //Vuong's Code
            var url = ApiUrls.BuildBookingOwners(userId, parkingId).SetQueryParams(new
			{
				startTimestamp = starttimestamp.ToString(),
				endTimestamp = endtimestamp.ToString(),
				sorting = sorTing
			});
            return await url.ToString().GetJsonAsyncHandlerWithSessionId<CreateBookingOwnerRequest, List<Reservation>>("bookings");
        }

        public async Task<ApiResult<OwnerUnavailabilityRequest, List<OwnerUnavailability>>> OwnerUnavailabilities(string userId, string parkingId, long startTimestamp, long endTimestamp, string sorting)
        {
            var url = ApiUrls.BuildUnavaibilities(userId, parkingId, "", false).SetQueryParams(new
			{
				startTimestamp = startTimestamp.ToString(),
				endTimestamp = endTimestamp.ToString(),
				sorting = sorting
			});
            return await url.ToString().GetJsonAsyncHandlerWithSessionId<OwnerUnavailabilityRequest, List<OwnerUnavailability>>("unavailabilities");
        }

        public async Task<ApiResult<CreateUnavailabilitiesRequest, CreateUnavailabilitiesResponse>> CreateUnavailabilities (string userId, string parkingId, string title, long startTimestamp, long endTimestamp, Periodicity peridocity, string numberOfSpots)
        {
            var url = ApiUrls.BuildUnavaibilities (userId, parkingId, "", false);
            //            return await url.ToString().PostJsonAsyncHandler<CreateUnavailabilitiesRequest,CreateUnavailabilitiesResponse>(new CreateUnavailabilitiesRequest()
            //                {
            //                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
            //                    Title = title,
            //                    StartTimestamp = startTimestamp,
            //                    EndTimestamp = endTimestamp,
            //                    Peridocity = peridocity,
            //                    NumberOfSpots = numberOfSpots
            //                });
            var result = await url.ToString ().PostUrlEncodedAsyncHandler<CreateUnavailabilitiesRequest, CreateUnavailabilitiesResponse> (new CreateUnavailabilitiesRequest () {
                //sessionId = Mvx.Resolve<ICacheService>().SessionId,
                SessionId = Mvx.Resolve<ICacheService> ().SessionId,
                title = title,
                startTimestamp = startTimestamp,
                endTimestamp = endTimestamp,
                periodicity = Newtonsoft.Json.JsonConvert.SerializeObject (peridocity),
                numberOfSpots = numberOfSpots
            });
            if (result != null && result.ApiError != null &&  result.ApiError.Status != null && result.ApiError.Status.Equals ("999")) {
                result.Response = new CreateUnavailabilitiesResponse ();
                result.Response.Status = "failed";
                result.Response.ErrorCode = "You can not set an unavailability above a booking";
            }
            return result;
        }
        public async Task<ApiResult<PutUnavailabilitiesRequest, PutUnavailabilitiesResponse>> PutUnavailabilities(string userId, string parkingId, string unavailabilityId, string title, long startTimestamp, long endTimestamp, Periodicity peridocity, string numberOfSpots, bool isOnlyEvent, long startTimestampOfSelectedOccurrence)
        {
            string mPeriodicity = "{}";
            if (!isOnlyEvent)
                mPeriodicity = Newtonsoft.Json.JsonConvert.SerializeObject(peridocity);



            return await ApiUrls.BuildUnavaibilities(userId, parkingId, unavailabilityId, isOnlyEvent).PutUrlEncodedAsyncHandler<PutUnavailabilitiesRequest, PutUnavailabilitiesResponse>(new PutUnavailabilitiesRequest()
                {
                    SessionId = Mvx.Resolve<ICacheService>().SessionId,
                    unavailabilityId = unavailabilityId,
                    title = title,
                    startTimestampOfSelectedOccurrence = startTimestampOfSelectedOccurrence,
                    startTimestampOfFirstDeprecatedOccurrence = startTimestamp,
                    startTimestamp = startTimestamp,
                    endTimestamp = endTimestamp,
                    periodicity = mPeriodicity,
                    numberOfSpots = numberOfSpots

                });
        }

        public async Task<bool> DeleteUnavailabilities(string userId, string parkingId, string unavailabilityId)
        {
            return await ApiUrls.BuildDeleteUnabaibilities(userId, parkingId, unavailabilityId).ToString().DeleteJsonAsyncHandlerWithSessionId<DeleteUnavailabilitiesRequest, ApiCommonResponse>();
        }

        public async Task<RecommendPriceResponse> RecommendPrice(double lat, double lng)
        {
            return await ApiUrls.BuildRecommendPriceUrl(lat, lng).SetQueryParam("sessionId", Mvx.Resolve<ICacheService>().SessionId).ToString().GetJsonAsyncHandler<RecommendPriceResponse>();
        }

        public async Task<AddressResponse> AddressOf(double lat, double lng)
        {
            return await ApiUrls.BuildAddressOfUrl(lat, lng).SetQueryParam("sessionId", Mvx.Resolve<ICacheService>().SessionId).ToString().GetJsonAsyncHandler<AddressResponse>();
        }

        public async Task<ShortestDistanceResponse> ShortestDistance(double fromLat, double fromLng, double toLat, double toLng)
        {
            return await ApiUrls.BuildShortestDistanceUrl(fromLat, fromLng, toLat, toLng).GetJsonAsyncHandler<ShortestDistanceResponse>();
        }

        public async Task<HereMapResponseWrapper> CheckPlaces(string street, double lat, double lng)
        {
            return await ApiUrls.HereMapGeocoderUri.SetQueryParams(
                new
				{
					street = street,
					prox = string.Format("{0},{1}", lat.ParseToCultureInfo(new CultureInfo("en-US")), lng.ParseToCultureInfo(new CultureInfo("en-US"))),
					gen = ApiUrls.HereMapApiGeneration,
					maxresults = 10,
					app_code = ApiUrls.HereMapAppCode,
					app_id = ApiUrls.HereMapAppId
				}).ToString().GetHereMapJsonAsync<HereMapResponseWrapper>();
        }


        public async Task<AppleVerifyReceiptResponse> AppleVerifyReceipt(string receiptEncoded)
        {
            //            try{
            //                var data = await ApiUrls.AppleVerifyReceipt.PostJsonAsync(new AppleVerifyReceiptRequest
            //                    {
            //                        receipt_data = receiptEncoded
            //                    }).ReceiveJson<AppleVerifyReceiptResponse>();
            //                
            //                return data;
            //            }
            //            catch(FlurlHttpException ex)
            //            {
            //                string message = ex.Message;
            //            }
            //            return null;
            return await ApiUrls.AppleVerifyReceipt.ToString().PostJsonAsyncHandlerReturnJson<AppleVerifyReceiptRequest, AppleVerifyReceiptResponse>(new AppleVerifyReceiptRequest
                {
                    receipt_data = receiptEncoded
                });
        }

        public async Task<ApiResult<TopupRequest, TopupResponse>> UpdateUserCredits(string userId, string store, double amount, string transactionReceiptEncoded)
        {
            return await ApiUrls.BuildTopUpUrl(userId).ToString().PostUrlEncodedAsyncHandler<TopupRequest, TopupResponse>(new TopupRequest
                {
                    creditAmount = amount,
                    store = store,
                    transactionReceipt = transactionReceiptEncoded,
                    SessionId = Mvx.Resolve<ICacheService>().SessionId
                });
        }

        public async Task<ApiResult<TopupPaymentRequest, TopupResponse>> UpdateUserCreditsViaPayment (string userId, string cardToken, double amount)
        {
            return await ApiUrls.BuildTopUpCreditCardUrl (userId).ToString ().PostJsonAsyncHandler<TopupPaymentRequest, TopupResponse> (new TopupPaymentRequest {
                creditAmount = amount,
                cardToken = cardToken,
                SessionId = Mvx.Resolve<ICacheService> ().SessionId
            });
        }

        public async Task<GetProductsResponseWrapper> GetProductIds(string store)
        {
            return await ApiUrls.BuildStoreProductIdsUrl(store).SetQueryParams(new
			{
				sessionId = Mvx.Resolve<ICacheService>().SessionId
			}).ToString().GetJsonAsyncHandler<GetProductsResponseWrapper>();
        }

    }

    #region FakeJsonConverter
    public class FakeJsonConverter : JsonConverter
    {
        #region implemented abstract members of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            var responseToken = token.SelectToken("response");
            if (responseToken == null)
            {
                responseToken = token[mPropertyName];
                token["response"] = responseToken.DeepClone();
            }

            return token.ToObject(objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        #endregion

        #region Constructor

        private string mPropertyName = string.Empty;

        public FakeJsonConverter(string propertyName)
        {
            mPropertyName = propertyName;
        }

        #endregion
    }
    #endregion

    #region FlurlExtensions
    public static class FlurlExtensions
    {
        public static async Task<TResponse> PostJsonAsyncFake<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response")
        {
            /*var str = await url.PostJsonAsync(request).ReceiveString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest,TResponse>>(str, new FakeJsonConverter(responseNodeName));

            return result.Response;*/

            try
            {
                var str = await url.PostJsonAsync(request).ReceiveString();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));

                return result.Response;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                string responseBody = ex.Call.ErrorResponseBody;
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(TResponse);
        }

        public static async Task<TResponse> PostJsonAsyncFake2<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response")
        {
            /*var str = await url.PostJsonAsync(request).ReceiveString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest,TResponse>>(str, new FakeJsonConverter(responseNodeName));

            return result.Response;*/

            try
            {
                var str = await url.PostJsonAsync(request).ReceiveString();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(str, new FakeJsonConverter(responseNodeName));

                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                string responseBody = ex.Call.ErrorResponseBody;
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(TResponse);
        }

        public static async Task<TResponse> PostJsonAsyncHandlerReturnJson<TRequest, TResponse>(this string url, TRequest request, bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var data = await url.PostJsonAsync(request).ReceiveJson<TResponse>();
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: POST");
                isSucceed = true;
                return data;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;

                Debug.WriteLine(error.Error);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PostJsonAsyncHandlerReturnJson<TRequest, TResponse>(request);
                }
            }

            return default(TResponse);
        }

        public static async Task<ApiResult<TRequest, TResponse>> PostJsonAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.PostJsonAsync(request).ReceiveString();
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: POST");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;

                Debug.WriteLine(error.Error);
                if (error.Error.Equals ("unKnownToken") || error.Error.Equals ("unKnownEmail") || error.Error.Equals("unknownToken") || error.Error.Equals("unknownEmail")) {
                    var defaultReturn = new ApiResult<TRequest, TResponse> ();
                    defaultReturn.ApiError = new ApiError ();
                    defaultReturn.ApiError.Error = error.Error;
                    defaultReturn.ApiError.Status = "777";
                    return defaultReturn;
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PostJsonAsyncHandler<TRequest, TResponse>(request);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }

        public static async Task<ApiResult<TRequest, TResponse>> PostUrlEncodedAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.PostUrlEncodedAsync(request).ReceiveString();
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: POST");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;
                Debug.WriteLine(error.Error);
                if(error.Error.Equals ("collisionWithBooking"))
                {
                    var defaultReturn =  new ApiResult<TRequest, TResponse>();
                    defaultReturn.ApiError = new ApiError ();
                    defaultReturn.ApiError.Error = error.Error;
                    defaultReturn.ApiError.Status = "999";
                    return defaultReturn;
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PostUrlEncodedAsyncHandler<TRequest, TResponse>(request);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }

        public static async Task<TResponse> PostUrlEncodedAsyncHandler2<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.PostUrlEncodedAsync(request).ReceiveJson<ApiResult<TRequest, TResponse>>();
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: POST");
                //var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest,TResponse>>(str);
                isSucceed = true;
                return str.Response;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;
                Debug.WriteLine(error.Error);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PostUrlEncodedAsyncHandler2<TRequest, TResponse>(request);
                }
            }

            return default(TResponse);
        }

        public static async Task<ApiResult<TRequest, TResponse>> PutUrlEncodedAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.PutUrlEncodedAsync(request).ReceiveString();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: PUT");
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;
                Debug.WriteLine(error.Error);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PutUrlEncodedAsyncHandler<TRequest, TResponse>(request, responseNodeName, true);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }



        public static async Task<ApiResult<TRequest, TResponse>> PatchUrlEncodedAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                //var str = await url.PatchJsonAsync(request).ReceiveString();
				Debug.WriteLine(url);
				Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: PATCH");
                var str = await url.PatchUrlEncodedAsync(request).ReceiveString();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;
                Debug.WriteLine(error.Error);

				if (error.Error.Equals("cardTrouble"))
				{
					var defaultReturn = new ApiResult<TRequest, TResponse>();
					defaultReturn.ApiError = new ApiError();
					defaultReturn.ApiError.Error = error.Error;
					defaultReturn.ApiError.Status = "999";
					return defaultReturn;
				}
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PatchUrlEncodedAsyncHandler<TRequest, TResponse>(request, responseNodeName, true);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }

        public static async Task<ApiResult<TRequest, TResponse>> DeleteJsonAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response")
        {
            try
            {
                var str = await url.DeleteAsync().ReceiveString();
				Debug.WriteLine("http method: DELETE");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));

                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                Debug.WriteLine(error.Error);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(ApiResult<TRequest, TResponse>);
        }



        public static async Task<ApiResult<TRequest, TResponse>> PutJsonAsyncHandler<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response", bool isRetry = false) where TRequest : BaseRequest
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.PutJsonAsync(request).ReceiveString();
                Debug.WriteLine(url);
                Debug.WriteLine(request.ToString());
				Debug.WriteLine("http method: PUT");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
				Debug.WriteLine(message);
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                isSucceed = false;

                Debug.WriteLine(error.Error);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    request.SessionId = Mvx.Resolve<ICacheService>().SessionId;
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.PutJsonAsyncHandler<TRequest, TResponse>(request, responseNodeName, true);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }

        public static async Task<TResponse> PutJsonAsyncFake<TRequest, TResponse>(this string url, TRequest request, string responseNodeName = "response")
        {
            /*var str = await url.PutJsonAsync(request).ReceiveString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest,TResponse>>(str, new FakeJsonConverter(responseNodeName));

            return result.Response;*/

            try
            {
                var str = await url.PutJsonAsync(request).ReceiveString();
				Debug.WriteLine("http method: PUT");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));

                return result.Response;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                string responseBody = ex.Call.ErrorResponseBody;
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(TResponse);
        }

        public static async Task<TResponse> GetJsonAsyncFake<TResponse>(this string url, string responseNodeName = "response")
        {
            /* var str = await url.GetStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TResponse>>(str, new FakeJsonConverter(responseNodeName));

            return result.Response; */

            try
            {
                var str = await url.GetStringAsync();
				Debug.WriteLine("http method: GET");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TResponse>>(str, new FakeJsonConverter(responseNodeName));

                return result.Response;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                string responseBody = ex.Call.ErrorResponseBody;
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(TResponse);
        }

        public static async Task<TResponse> GetJsonAsyncHandler<TResponse>(this string url, bool isRetry = false)
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                Debug.WriteLine(url);
				Debug.WriteLine("http method: GET");
                var data = await url.GetJsonAsync<TResponse>();
                isSucceed = true;
                return data;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus != HttpStatusCode.Unauthorized && ex.Call.HttpStatus != HttpStatusCode.InternalServerError)
                {
                    string message = ex.Message;
                    error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                    Debug.WriteLine(error.Error);
                    isSucceed = false;
                }
                else
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.GetJsonAsyncHandler<TResponse>(true);
                }
            }

            return default(TResponse);
        }

        public static async Task<ApiResult<TRequest, TResponse>> GetJsonAsyncHandlerWithSessionId<TRequest, TResponse>(this string url, string responseNodeName = "response", bool isRetry = false)
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
                var str = await url.SetQueryParams(new { sessionId = Mvx.Resolve<ICacheService>().SessionId }).GetStringAsync();
                Debug.WriteLine(url.SetQueryParams(new { sessionId = Mvx.Resolve<ICacheService>().SessionId }));
				Debug.WriteLine("http method: GET");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest, TResponse>>(str, new FakeJsonConverter(responseNodeName));
                isSucceed = true;
                return result;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                if (ex.Call.HttpStatus == HttpStatusCode.InternalServerError)
                {
                    isSucceed = false;
                    Debug.WriteLine(ex.Call.ErrorResponseBody);
                }
                else if (ex.Call != null && ex.Call.ErrorResponseBody != null)
                {
                    error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                    Debug.WriteLine(error.Error);
                }
                isSucceed = false;

            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.GetJsonAsyncHandlerWithSessionId<TRequest, TResponse>(responseNodeName, true);
                }
            }

            return default(ApiResult<TRequest, TResponse>);
        }

        public static async Task<TResponse> GetHereMapJsonAsync<TResponse>(this string url)
        {
            try
            {
                return await url.GetJsonAsync<TResponse>();
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                int i = 0;
            }

            return default(TResponse);
        }

        public static async Task<bool> DeleteJsonAsyncHandlerWithSessionId<TRequest, TResponse>(this string url, string responseNodeName = "response", bool isRetry = false)
        {
            ApiError error = null;
            bool isSucceed = false;
            try
            {
				var str = await url.SetQueryParams(new { sessionId = Mvx.Resolve<ICacheService>().SessionId }).DeleteAsync();
				Debug.WriteLine(url);
				Debug.WriteLine("http method: DELETE");
                //                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<TRequest,TResponse>>(str, new FakeJsonConverter(responseNodeName));
                //                return result;
				isSucceed = str.StatusCode == System.Net.HttpStatusCode.Accepted || str.StatusCode == System.Net.HttpStatusCode.OK;
				return isSucceed;//str.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (FlurlHttpTimeoutException)
            {
                int i = 0;
            }
            catch (FlurlHttpException ex)
            {
                string message = ex.Message;
                error = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiError>(ex.Call.ErrorResponseBody);
                Debug.WriteLine(error.Error);
                isSucceed = false;

            }
            catch (Exception e)
            {
                int i = 0;
            }

            if (!isSucceed && (error != null) && !isRetry)
            {
                if (await SessionExpiredOrInvalid(error))
                {
                    Debug.WriteLine("Get the sessionId again !!!");
                    return await url.DeleteJsonAsyncHandlerWithSessionId<TRequest, TResponse>(responseNodeName, true);
                }
            }

            return false;
        }

        private static async Task<bool> SessionExpiredOrInvalid(ApiError error)
        {
            if (error.Error.Equals("unknownSessionId") || error.Error.Equals ("unKnownSessionId")  || error.Error.Equals("sessionExpired"))
            {
                //call the api again to get session id
                Debug.WriteLine("Retrieving the sessionId");
                var response = await Mvx.Resolve<IApiService>().Login(Mvx.Resolve<ICacheService>().CurrentUser.Email, Mvx.Resolve<ICacheService>().CurrentUser.Password,
                                   Mvx.Resolve<IPlatformService>().OS == OS.Touch ? AppConstants.iOSToken : AppConstants.AndroidToken);
                if (response != null)
                {
                    Mvx.Resolve<ICacheService>().SessionId = response.SessionId;
                    return true;
                }
                else
                    return false;
            }

            return false;
        }
    }
    #endregion

    #region Classes

    public class ApiResult<TRequest, TResponse>
    {
        public TRequest Request { get; set; }

        public TResponse Response { get; set; }

        public ApiError ApiError { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class ApiResult<TResponse>
    {
        public TResponse Response { get; set; }
    }

    public class BaseRequest
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    public class PersonGetRequest
    {
        public string UserId { get; set; }
    }

	public class AddCardRequest : BaseRequest
	{
		[JsonProperty("cardToken")]
		public string TokenId { get; set; }

		public override string ToString()
		{
			return string.Format("[AddCardRequest: TokenId={0}]", TokenId);
		}
	}

	public class AddBankRequest : BaseRequest
	{
		[JsonProperty("bankAccountToken")]
		public string BankAccountToken { get; set; }

		public override string ToString()
		{
			return string.Format("[AddBankRequest: TokenId={0}]", BankAccountToken);
		}
	}

    public class LoginRequest : BaseRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        public override string ToString()
        {
            return string.Format("[LoginRequest: Email={0}, Password={1}, Token={2}]", Email, Password, Token);
        }
    }

    public class LoginResponse
    {
        public string UserId { get; set; }

        public string SessionId { get; set; }

        public string Status { get; set; }

        public string ErrorCode { get; set; }
    }

    public class RegisterRequest : BaseRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("deviceIdentifier")]
        public string DeviceIdentifier { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[RegisterRequest: Email={0}, Password={1}, Token={2}, DeviceIdentifier={3}, FirstName={4}, LastName={5}, PhoneNumber={6}]", Email, Password, Token, DeviceIdentifier, FirstName, LastName, PhoneNumber);
        }

    }



    public class RegisterResponse
    {
        public string PersonId { get; set; }

        public string Status { get; set; }

        public string ErrorCode { get; set; }

        public override string ToString()
        {
            return string.Format("[RegisterResponse: PersonId={0}, Result={1}, ErrorCode={2}]", PersonId, Status, ErrorCode);
        }
    }

    public class GetVehicleListRequest
    {
        public string UserId { get; set; }
    }

    public class Vehicle
    {
        public string VehicleId { get; set; }

        public string Type { get; set; }

        public string PlateNumber { get; set; }
    }

    public class AvaiableParkingsRequest
    {
        [JsonProperty("startTimestamp")]
        public string StartTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public string EndTimestamp { get; set; }

        public string MaxWaitingDuration { get; set; }

        public string GpsLatitude { get; set; }

        public string GpsLongitude { get; set; }

        public string MaxDistance { get; set; }

        public string CarSize { get; set; }

        public string MinRating { get; set; }

        public string Provider { get; set; }

        public string Sorting { get; set; }

        public int Limit { get; set; }

        public int Start { get; set; }

        public override string ToString()
        {
            return string.Format("[AvaiableParkingsRequest: StartTimestamp={0}, EndTimestamp={1}, MaxWaitingDuration={2}, GpsLatitude={3}, GpsLongitude={4}, MaxDistance={5}, CarSize={6}, MinRating={7}, Provider={8}, Sorting={9}, Limit={10}, Start={11}]", StartTimestamp, EndTimestamp, MaxWaitingDuration, GpsLatitude, GpsLongitude, MaxDistance, CarSize, MinRating, Provider, Sorting, Limit, Start);
        }
    }

    public class ParkingSpot
    {
        public string ParkingId { get; set; }

		public string BookingType { get; set; }

        public string AvailabilityStartTimestamp { get; set; }

        public string AvailabilityDuration { get; set; }

        public string Distance { get; set; }

        public double Cost { get; set; }

        public string Rating { get; set; }

        public string Provider { get; set; }

		public string Latitude { get; set; }

		public string Longitude { get; set; }
    }

    /*public class GetUserResponse
    {
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string Email {get;set;}
        public string PhoneNumber {get;set;}
        public string FacebookId {get;set;}
        public string Street {get;set;}
        public string City {get;set;}
        public string PostalCode {get;set;}
        public string Country {get;set;}
        public string RemainingCredits {get;set;}
    }*/



    public class PutUserRequest : BaseRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("facebookId")]
        public string FacebookId { get; set; }

		[JsonProperty("dateOfBirth")]
		public string Birthday { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public override string ToString()
        {
			return string.Format("[PutUserRequest: UserId={0}, FirstName={1}, LastName={2}, Email={3}, PhoneNumber={4}, FacebookId={5}, Street={6}, City={7}, PostalCode={8}, Country={9}, Birthday={10}, Password={11}]", UserId, FirstName, LastName, Email, PhoneNumber, FacebookId, Street, City, PostalCode, Country, Birthday, Password);
        }
    }

    public class CreateVehicleRequest : BaseRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("plateNumber")]
        public string PlateNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateVehicleRequest: Type={0}, PlateNumber={1}]", Type, PlateNumber);
        }
    }

    public class CreateVehicleResponse
    {
        public string VehicleId { get; set; }

        public string Status { get; set; }

        public string ErrorCode { get; set; }
    }

    public class CreateBookingRequest : BaseRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("parkingId")]
        public string ParkingId { get; set; }

        [JsonProperty("startTimestamp")]
        public string StartTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public string EndTimestamp { get; set; }

        [JsonProperty("renterCarPlateNumber")]
        public string RenterCarPlateNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateBookingRequest: UserId={0}, ParkingId={1}, StartTimestamp={2}, EndTimestamp={3}, RenterCarPlateNumber={4}]", UserId, ParkingId, StartTimestamp, EndTimestamp, RenterCarPlateNumber);
        }
    }

    public class CreateBookingResponse
    {
        public string BookingId { get; set; }

        public string Cost { get; set; }

        public string TimeoutDuration { get; set; }

        public string RemainingCredits { get; set; }

        public string Status { get; set; }

        public string ErrorCode { get; set; }
    }

    public class CreateBookingOwnerRequest
    {
        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public string StartTimeStamp { get; set; }

        public string EndTimeStamp { get; set; }

        public string Sorting { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateBookingOwnerRequest: UserId={0}, ParkingId={1}, StartTimeStamp={2}, EndTimeStamp={3}, Sorting={4}]", UserId, ParkingId, StartTimeStamp, EndTimeStamp, Sorting);
        }
    }



    /// <summary>
    ///  Using PatchUrlEncodedAsync 
    /// </summary>
    public class PayBookingRequest : BaseRequest
    {
        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public string BookingId { get; set; }

        public string Action { get; set; }

        public string PaymentType { get; set; }

        public string operations { get; set; }

        public string data { get; set; }

        public string sessionId
        {
            get
            {
                return SessionId;
            }
            set
            { }
        }

        public override string ToString()
        {
            return string.Format("[PayBookingRequest: UserId={0}, ParkingId={1}, BookingId={2}, Action={3}, PaymentType={4}, operations={5}, data={6}, sessionId={7}]", UserId, ParkingId, BookingId, Action, PaymentType, operations, data, sessionId);
        }
    }

    /// <summary>
    ///  Using PatchUrlEncodedAsync 
    /// </summary>
    public class LeaveBookingRequest : BaseRequest
    {
        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public string BookingId { get; set; }

        public string operations { get; set; }

        public string data { get; set; }

        public string sessionId
        { 
            get
            {
                return SessionId;
            }
            set{ }
        }

        public override string ToString()
        {
            return string.Format("[LeaveBookingRequest: UserId={0}, ParkingId={1}, BookingId={2}, operations={3}, data={4}, sessionId={5}]", UserId, ParkingId, BookingId, operations, data, sessionId);
        }
    }

    public class PayBookingResponse
    {
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Location { get; set; }

        //public string RemainingCredits { get; set; }

		public string OwnerPhoneNumber { get; set; }

        public string PhoneNumberToOpenTheGate { get; set; }

        public string Status { get; set; }

        public string ErrorCode { get; set; }
    }

	public class PersonGetCardResponse : ApiCommonResponse
	{
		[JsonProperty("pspCustomerIdLast4")]
		public string CustomerIdLast4 { get; set; }

		[JsonProperty("paymentCard")]
		public PaymentCard Card { get; set; }
	}

	public class PaymentCard
	{
		[JsonProperty("brand")]
		public string Brand { get; set; }

		[JsonProperty("idLast4")]
		public string IdLast4 { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("numberLast4")]
		public string Last4 { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("holderName")]
		public string Name { get; set; }

		[JsonProperty("expMonth")]
		public int ExpMonth { get; set; }

		[JsonProperty("expYear")]
		public int ExpYear { get; set; }
	}

	public class PersonGetBankResponse : ApiCommonResponse
	{
		[JsonProperty("pspMerchantIdLast4")]
		public string CustomerIdLast4 { get; set; }

		[JsonProperty("bankAccount")]
		public BankAccount BankAccount { get; set; }
	}

	public class BankAccount
	{
		[JsonProperty("idLast4")]
		public string IdLast4 { get; set; }

		[JsonProperty("holderName")]
		public string Name { get; set; }

		[JsonProperty("holderType")]
		public string Type { get; set; }

		[JsonProperty("bankName")]
		public string BankName { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("numberLast4")]
		public string Last4 { get; set; }
	}

	public class AddCardResponse : ApiCommonResponse
	{
		[JsonProperty("card")]
		public CreditCard card { get; set; }
	}

	public class CreditCard
	{
		[JsonProperty("brand")]
		public string Brand { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("last4")]
		public string Last4 { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("expMonth")]
		public int ExpMonth { get; set; }

		[JsonProperty("expYear")]
		public int ExpYear { get; set; }
	}

    public class ApiCommonResponse
    {
        public string Status { get; set; }

        public string ErrorCode { get; set; }
    }

    public class RecommendPriceResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("prices")]
        public Prices Prices { get; set; }

    }

    public class Prices
    {
        [JsonProperty("hourly")]
        public double ParkingRateHourly { get; set; }

        [JsonProperty("daily")]
        public double ParkingRateDaily { get; set; }

        [JsonProperty("weekly")]
        public double ParkingRateWeekly { get; set; }

        [JsonProperty("monthly")]
        public double ParkingRateMonthly { get; set; }
    }

    public class ShortestDistanceResponse
    {
        public string Distance { get; set; }
    }

    public class AddressResponse
    {
        public string Address { get; set; }
    }

    public class PutVehicleRequest : BaseRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("plateNumber")]
        public string PlateNumber { get; set; }

        public override string ToString()
        {
            return string.Format("[PutVehicleRequest: UserId={0}, VehicleId={1}, Type={2}, PlateNumber={3}]", UserId, VehicleId, Type, PlateNumber);
        }
    }

    public class DeleteVehicleRequest : BaseRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
    }

    public class Reservation
    {
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        [JsonProperty("startTimestamp")]
        public string StartTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public string EndTimestamp { get; set; }

        [JsonProperty("renterCarPlateNumber")]
        public string PlateNumber { get; set; }

        [JsonProperty("periodicity")]
        public string Periodicity { get; set; }

        public string VehicleType { get; set; }

        [JsonProperty("cost")]
        public string Cost { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty ("leavingstatus")]
        public string LeavingStatus { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty ("review")]
        public string Review { get; set; }

        [JsonProperty ("ownerPhoneNumber")]
        public string OwnerPhoneNumber { get; set; }

        public Parking Parking { get; set; }
    }

    public class Parking
    {
        public string ParkingId { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Location { get; set; }
    }

    public class CreateParkingResponse
    {
        [JsonProperty("parkingId")]
        public string ParkingId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
    }

    public class CreateParkingRequest : BaseRequest
    {
        //[JsonProperty("userId")]
        //public string UserId { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("gpsAccuracy")]
        public string GpsAccuracy { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("availableFromDate")]
        public string AvailableFromDate { get; set; }

        [JsonProperty("availableToDate")]
        public string AvailableToDate { get; set; }

        [JsonProperty("carType")]
        public string CarType { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; }

        [JsonProperty("numberOfSpots")]
        public string NumberOfSpots { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("accessProcedure")]
        public string AccessProcedure { get; set; }

        [JsonProperty("accessKey")]
        public string AccessKey { get; set; }

        [JsonProperty("gatePhoneNumber")]
        public string GatePhoneNumber { get; set; }

        [JsonProperty("canRentWithoutOwnerConfirmation")]
        public string CanRentWithoutOwnerConfirmation { get; set; }

        [JsonProperty("minimumTimeUnit")]
        public string MinimumTimeUnit { get; set; }

        [JsonProperty("amountOfMinimumTimeUnit")]
        public int AmountOfMinimumTimeUnit { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("hourlyRate")]
        public double HourlyRate { get; set; }

        [JsonProperty("dailyRate")]
        public double DailyRate { get; set; }

        [JsonProperty("weeklyRate")]
        public double WeeklyRate { get; set; }

        [JsonProperty("monthlyRate")]
        public double MonthlyRate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("visiblePlateNumber")]
        public string VisiblePlateNumber { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("disabled")]
        public string Disabled { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateParkingRequest: Latitude={0}, Longitude={1}, GpsAccuracy={2}, Location={3}, AvailableFromDate={4}, AvailableToDate={5}, CarType={6}, Availability={7}, NumberOfSpots={8}, Context={9}, AccessProcedure={10}, AccessKey={11}, GatePhoneNumber={12}, CanRentWithoutOwnerConfirmation={13}, MinimumTimeUnit={14}, AmountOfMinimumTimeUnit={15}, Currency={16}, HourlyRate={17}, DailyRate={18}, WeeklyRate={19}, MonthlyRate={20}, Description={21}, VisiblePlateNumber={22}, Picture={23}, Disabled={24}]", Latitude, Longitude, GpsAccuracy, Location, AvailableFromDate, AvailableToDate, CarType, Availability, NumberOfSpots, Context, AccessProcedure, AccessKey, GatePhoneNumber, CanRentWithoutOwnerConfirmation, MinimumTimeUnit, AmountOfMinimumTimeUnit, Currency, HourlyRate, DailyRate, WeeklyRate, MonthlyRate, Description, VisiblePlateNumber, Picture, Disabled);
        }
    }

    public class PutParkingRequest : BaseRequest
    {
        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string GpsAccuracy { get; set; }

        public string Location { get; set; }

        public string AvaiableFromDate { get; set; }

        public string AvaiableToDate { get; set; }

        public string CarType { get; set; }

        public string Availability { get; set; }

        public string NumberOfSpots { get; set; }

        public string Context { get; set; }

        public string AccessProcedure { get; set; }

        public string AccessKey { get; set; }

        public string GatePhoneNumber { get; set; }

        public string CanRentWithoutOwnerConfirmation { get; set; }

        public string MinimumTimeUnit { get; set; }

        public int AmountOfMinimumTimeUnit { get; set; }

        public string Currency { get; set; }

        public double HourlyRate { get; set; }

        public double DailyRate { get; set; }

        public double WeeklyRate { get; set; }

        public double MonthlyRate { get; set; }

        public string Description { get; set; }

        public string VisiblePlateNumber { get; set; }

        public string Picture { get; set; }

        public string Disabled { get; set; }

        public override string ToString()
        {
            return string.Format("[PutParkingRequest: UserId={0}, ParkingId={1}, Latitude={2}, Longitude={3}, GpsAccuracy={4}, Location={5}, AvaiableFromDate={6}, AvaiableToDate={7}, CarType={8}, Availability={9}, NumberOfSpots={10}, Context={11}, AccessProcedure={12}, AccessKey={13}, GatePhoneNumber={14}, CanRentWithoutOwnerConfirmation={15}, MinimumTimeUnit={16}, AmountOfMinimumTimeUnit={17}, Currency={18}, HourlyRate={19}, DailyRate={20}, WeeklyRate={21}, MonthlyRate={22}, Description={23}, VisiblePlateNumber={24}, Picture={25}, Disabled={26}]", UserId, ParkingId, Latitude, Longitude, GpsAccuracy, Location, AvaiableFromDate, AvaiableToDate, CarType, Availability, NumberOfSpots, Context, AccessProcedure, AccessKey, GatePhoneNumber, CanRentWithoutOwnerConfirmation, MinimumTimeUnit, AmountOfMinimumTimeUnit, Currency, HourlyRate, DailyRate, WeeklyRate, MonthlyRate, Description, VisiblePlateNumber, Picture, Disabled);
        }
    }

    public class OwnerParkingsRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("limit")]
        public string Limit { get; set; }

        public override string ToString()
        {
            return string.Format("[OwnerParkingsRequest: UserId={0}, Sort={1}, Start={2}, Limit={3}]", UserId, Sort, Start, Limit);
        }
    }




    public class OwnerParkingsResponse
    {
        public string ParkingId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string GpsAccuracy { get; set; }

        public string Location { get; set; }

        public string AvaiableFromDate { get; set; }

        public string AvaiableToDate { get; set; }

        public string VehicleType { get; set; }

        public string Availability { get; set; }

        public string NumberOfSpots { get; set; }

        public string Context { get; set; }

        public string AccessProcedure { get; set; }

        public string AccessKey { get; set; }

        public string GatePhoneNumber { get; set; }

        public string CanRentWithoutOwnerConfirmation { get; set; }

        public string MinimumTimeUnit { get; set; }

        public int AmountOfMinimumTimeUnit { get; set; }

        public string Currency { get; set; }

        public double HourlyRate { get; set; }

        public double DailyRate { get; set; }

        public double WeeklyRate { get; set; }

        public double MonthlyRate { get; set; }

        public string Description { get; set; }

        public string VisiblePlateNumber { get; set; }

        public string Picture { get; set; }

        public string Disabled { get; set; }
    }



    public class OwnerBooking
    {
        public string BookingId { get; set; }

        public long StartTimestamp { get; set; }

        public long EndTimestamp { get; set; }

        public string PlateNumber { get; set; }

        public double Cost { get; set; }

        public string Currency { get; set; }

        public string Status { get; set; }

        public string Rating { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is OwnerBooking)
            {
                return (((OwnerBooking)obj).BookingId == this.BookingId);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class OwnerUnavailabilityRequest : BaseRequest
    {
        public string UserId { get; set; }

        public int ParkingId { get; set; }

        public long StartTimeStamp { get; set; }

        public long EndTimeStamp { get; set; }

        public string Sorting { get; set; }

        public override string ToString()
        {
            return string.Format("[OwnerUnavailabilityRequest: UserId={0}, ParkingId={1}, StartTimeStamp={2}, EndTimeStamp={3}, Sorting={4}]", UserId, ParkingId, StartTimeStamp, EndTimeStamp, Sorting);
        }


    }

    public class OwnerUnavailability
    {
        public string UnavailabilityId { get; set; }

        public string Title { get; set; }

        public long StartTimestamp { get; set; }

        public long EndTimestamp { get; set; }

        public string NumberOfSpots { get; set; }

        public Periodicity Periodicity { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is OwnerUnavailability)
            {
                var unavaiability = (OwnerUnavailability)obj;
                return ((unavaiability.UnavailabilityId == this.UnavailabilityId) && (unavaiability.EndTimestamp == this.EndTimestamp) && (unavaiability.StartTimestamp == this.StartTimestamp));
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Periodicity
    {
        [JsonProperty("repeat")]
        public string Repeat { get; set; }

        [JsonProperty("occurrenceAmount")]
        public string OccurrencesAmount { get; set; }

        [JsonProperty("occurrences")]
        public List<PeriodicityException> Occurrences { get; set; }

        [JsonProperty("exceptions")]
        public List<PeriodicityException> Exceptions { get; set; }
    }

    public class PeriodicityException
    {
        [JsonProperty("startTimestamp")]
        public long StartTimestamp { get; set; }
    }

    public class PeridocityCollision
    {
        public long StartTimestamp { get; set; }
    }

    public class CreateUnavailabilitiesResponse
    {
        public string UnavailabilityId { get; set; }

        public string ErrorCode { get; set; }

        public string Status { get; set; }
    }

    public class CreateUnavailabilitiesRequest : BaseRequest
    {
        public string sessionId { get { return base.SessionId; } set { } }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("startTimestamp")]
        public long startTimestamp { get; set; }

        [JsonProperty("endTimestamp")]
        public long endTimestamp { get; set; }

        [JsonProperty("periodicity")]
        public string periodicity { get; set; }

        [JsonProperty("numberOfSpots")]
        public string numberOfSpots { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateUnavailabilitiesRequest: sessionId={0}, title={1}, startTimestamp={2}, endTimestamp={3}, periodicity={4}, numberOfSpots={5}]", sessionId, title, startTimestamp, endTimestamp, periodicity, numberOfSpots);
        }
    }

    public class PutUnavailabilitiesResponse
    {
        public List<PeridocityCollision> Collisions { get; set; }

        public string ErrorCode { get; set; }

        public string Status { get; set; }
    }

    public class PutUnavailabilitiesRequest : BaseRequest
    {
        public string sessionId { get { return base.SessionId; } set { } }

        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public string unavailabilityId { get; set; }

        public string title { get; set; }

        public long startTimestampOfSelectedOccurrence { get; set; }

        public long startTimestampOfFirstDeprecatedOccurrence { get; set; }

        public long startTimestamp { get; set; }

        public long endTimestamp { get; set; }

        public string periodicity { get; set; }

        public string numberOfSpots { get; set; }

        public override string ToString()
        {
            return string.Format("[PutUnavailabilitiesRequest: sessionId={0}, UserId={1}, ParkingId={2}, unavailabilityId={3}, title={4}, startTimestampOfSelectedOccurrence={5}, startTimestampOfFirstDeprecatedOccurrence={6}, startTimestamp={7}, endTimestamp={8}, periodicity={9}, numberOfSpots={10}]", sessionId, UserId, ParkingId, unavailabilityId, title, startTimestampOfSelectedOccurrence, startTimestampOfFirstDeprecatedOccurrence, startTimestamp, endTimestamp, periodicity, numberOfSpots);
        }
    }

    public class DeleteUnavailabilitiesRequest
    {
        public string UserId { get; set; }

        public string ParkingId { get; set; }

        public string UnavailabilityId { get; set; }

        public override string ToString()
        {
            return string.Format("[DeleteUnavailabilitiesRequest: UserId={0}, ParkingId={1}, UnavailabilityId={2}]", UserId, ParkingId, UnavailabilityId);
        }
    }


    public class CreateBookingPacket
    {
        public CreateBookingRequest Request { get; set; }

        public CreateBookingResponse Response { get; set; }
    }

    public class RegisterPacket
    {
        public RegisterRequest Request { get; set; }

        public RegisterResponse Response { get; set; }
    }

    public class ApiError
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class RenterBookingRequest
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("startTimestamp")]
        public long startTimestamp { get; set; }

        public string sort { get; set; }

        public int? start { get; set; }

        public int? limit { get; set; }

        public override string ToString()
        {
            return string.Format("[RenterBookingRequest: userId={0}, startTimestamp={1}, sort={2}, start={3}, limit={4}]", userId, startTimestamp, sort, start, limit);
        }

    }

    public class HereMapResponseWrapper
    {
        public HereMapResponse Response { get; set; }
    }

    public class HereMapResponse
    {
        public List<HereMapView> View { get; set; }
    }

    public class HereMapView
    {
        public int ViewId { get; set; }

        public List<HereMapResult> Result { get; set; }
    }

    public class HereMapResult
    {
        public double Relevance { get; set; }

        public double Distance { get; set; }

        public HereMapLocation Location { get; set; }

        public string BindingStreet
        {
            get
            {
                return string.Format("{0} {1}", Location.Address.HouseNumber, Location.Address.Street);
            }
        }

        public string BindingCity
        {
            get
            {
                var country = Location.Address.AdditionalData.FirstOrDefault(x => x.key.Equals("CountryName"));
                if (string.IsNullOrEmpty(country.value))
                    country.value = string.Empty;
                return string.Format("{0} {1}, {2}", Location.Address.PostalCode, Location.Address.City, country.value);
            }
        }
    }

    public class HereMapLocation
    {
        public string LocationId { get; set; }

        public string LocationType { get; set; }

        public List<HereMapPosition> NavigationPosition { get; set; }

        public HereMapAddress Address { get; set; }
    }

    public class HereMapPosition
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class HereMapAddress
    {
        public string HouseNumber { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public List<HereMapAdditionalData> AdditionalData { get; set; }
    }

    public class HereMapAdditionalData
    {
        public string key { get; set; }

        public string value { get; set; }
    }

    public class TopupRequest : BaseRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("creditAmount")]
        public double creditAmount { get; set; }

        [JsonProperty("store")]
        public string store { get; set; }

        [JsonProperty("transactionReceipt")]
        public string transactionReceipt { get; set; }

        //[JsonProperty("sessionId")]
        //public string sessionId { get; set; }
    }

    public class TopupPaymentRequest : BaseRequest
    {
        //[JsonProperty ("userId")]
        //public string UserId { get; set; }

        [JsonProperty ("creditAmount")]
        public double creditAmount { get; set; }

        [JsonProperty ("cardToken")]
        public string cardToken { get; set; }

        //[JsonProperty ("sessionId")]
        //public string sessionId { get; set; }
    }

    public class TopupResponse
    {
        [JsonProperty("remainingCredits")]
        public string RemainingCredits { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
    }

    public class AppleVerifyReceiptRequest : BaseRequest
    {
        [JsonProperty("receipt-data")]
        public string receipt_data { get; set; }
    }

    public class AppleVerifyReceiptResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("receipt")]
        public AppleReceipt Receipt { get; set; }
    }

    public class AppleReceipt
    {
        public string original_purchase_date_pst { get; set; }

        public string purchase_date_ms { get; set; }

        public string unique_identifier { get; set; }

        public string original_transaction_id { get; set; }

        public string bvrs { get; set; }

        public string transaction_id { get; set; }

        public string quantity { get; set; }

        public string unique_vendor_identifier { get; set; }

        public string item_id { get; set; }

        public string product_id { get; set; }

        public string purchase_date { get; set; }

        public string original_purchase_date { get; set; }

        public string purchase_date_pst { get; set; }

        public string bid { get; set; }

        public string original_purchase_date_ms { get; set; }
    }

    public class IAPProducts
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }
    }

    public class GetProductsResponseWrapper
    {
        [JsonProperty("response")]
        public GetProductsResponse Response { get; set; }
    }

    public class GetProductsResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("products")]
        public List<IAPProducts> Products { get; set; }
    }

    #endregion

}

