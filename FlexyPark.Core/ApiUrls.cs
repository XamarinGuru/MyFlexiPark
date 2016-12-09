using System;
using Flurl;
using System.Globalization;
using FlexyPark.UI.Touch.Extensions;
using System.Xml;
using System.Runtime.Serialization;

namespace FlexyPark.Core
{
    public static class ApiUrls
    {
        //public const string Api = "http://local.beesightsoft.com/api/v1/current";
        //public const string FakeApi = "http://fake8.api.mfp.webili.com/";
        //public const string ApiV1 = "http://test.v1.api.mfp.webili.com/";
        //public const string RealApiV1 = "http://beesightsoft.v1.api.mfp.webili.com";




		//public const string RealApiV1 = "http://demo.v1.api.mfp.webili.com";
		public const string RealApiV1 = "http://yusuke.v1.api.mfp.webili.com";
		//public const string BaseWebURL = "http://www.demo.v1.www.mfp.webili.com/{0}/user/goto-parking";
		public const string BaseWebURL = "http://www.beesightsoft.v1.www.mfp.webili.com/{0}/user/goto-parking";




        //sandbox for testing
        public const string AppleVerifyReceipt = "https://sandbox.itunes.apple.com/verifyReceipt";

        //F0
        public static readonly string Login = Url.Combine(RealApiV1, "login");
        public static readonly string LostPassword = Url.Combine (RealApiV1, "resetPassword");

        //F1
        public static readonly string PersonRegister = Url.Combine(RealApiV1, "registerPerson");
        public static readonly string PersonGet = Url.Combine(RealApiV1, "persons/123.json");
        public static readonly string PersonPut = Url.Combine(RealApiV1, "persons/123.json");

        public static string BuildPersonUrl(string userId)
        {
            return Url.Combine(RealApiV1, "persons", userId);
        }

		public static string BuildPersonCardUrl(string userId)
		{
			return Url.Combine(RealApiV1, "persons", userId, "customer");
		}

		public static string BuildPersonBankUrl(string userId)
		{
			return Url.Combine(RealApiV1, "persons", userId, "merchant");
		}

		public static string BuildPersonPaymentUrl(string userId)
		{
			return Url.Combine(RealApiV1, "persons", userId, "paymentCard");
		}

		public static string BuildPersonBankAccountUrl(string userId)
		{
			return Url.Combine(RealApiV1, "persons", userId, "bankAccount");
		}

        //F2
        public static readonly string VehicleCreate = Url.Combine(RealApiV1, "persons/123/vehicles.json");
        public static readonly string VehicleList = Url.Combine(RealApiV1, "persons/123/vehicles.json");
        public static readonly string VehiclePut = Url.Combine(RealApiV1, "persons/123/vehicles/111.json");
        public static readonly string VehicleDelete = Url.Combine(RealApiV1, "persons/123/vehicles/111.json");

        public static string BuildVehicleUrl(string userId, string vehicleId = "")
        {
            if (string.IsNullOrEmpty(vehicleId))
                return Url.Combine(RealApiV1, "persons", userId, "vehicles");
            else
                return Url.Combine(RealApiV1, "persons", userId, "vehicles", vehicleId);
        }

        //F3
        public static readonly string AvaiableParkings = Url.Combine(RealApiV1, "parkings/available");

		//public static readonly string AvaiableParkingsInMap = Url.Combine(RealApiV1, "app_dev.php/parkings/available.json");

        //F4 Renters
        //        public static readonly string BookingListRenters = Url.Combine(FakeApi, "renters/123/bookings.json");
        //        public static readonly string BookingCreate = Url.Combine(FakeApi, "renters/123/parkings/456/bookings-create.json");
        //        public static readonly string BookingPay = Url.Combine(FakeApi, "renters/123/parkings/456/bookings/765-pay.json");
        //        public static readonly string BookingLeave = Url.Combine(FakeApi, "renters/123/parkings/456/bookings/765-leave.json");

        public static string BuildBookingUrl(string userId, string parkingid = "", string bookingId = "")
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                if (string.IsNullOrEmpty(parkingid))
                {
                    return Url.Combine(RealApiV1, "renters", userId, "bookings");
                }
                else
                {
                    return Url.Combine(RealApiV1, "renters", userId, "parkings", parkingid, "bookings");
                }
            }
            else
            {
                return Url.Combine(RealApiV1, "renters", userId, "parkings", parkingid, "bookings", bookingId);
            }
        }
  

        //F5
        //        public static readonly string ParkingList = Url.Combine(FakeApi, "owners/123/parkings-list.json");
        //        public static readonly string ParkingCreate = Url.Combine(FakeApi, "owners/123/parkings-create.json");
        //        public static readonly string ParkingPut = Url.Combine(FakeApi, "owners/123/parkings/456-put.json");

        public static string BuildParking(string ownerId, string parkingid)
        {
            if (string.IsNullOrEmpty(parkingid))
            {
                return Url.Combine(RealApiV1, "owners", ownerId, "parkings");
            }
            else
                return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingid);
        }

        //F6 Owners
        //        public static readonly string BookingListOwners = Url.Combine(FakeApi, "owners/123/parkings/456/bookings-list.json");
        //        public static readonly string UnavailabilitiesList = Url.Combine(FakeApi, "owners/123/parkings/456/unavailabilities-list.json");
        //        public static readonly string UnavailabilitiesCreate = Url.Combine(FakeApi, "owners/123/parkings/456/unavailabilities-create.json");
        //        public static readonly string UnavailabilitiesPut = Url.Combine(FakeApi, "owners/123/parkings/456/unavailabilities/333-put.json");
        //        public static readonly string UnavailabilitiesDelete = Url.Combine(FakeApi, "owners/123/parkings/456/unavailabilities/333-delete.json");

        public static string BuildBookingOwners(string ownerId, string parkingId)
        {
            return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingId, "bookings");
        }

        public static string BuildUnavaibilities(string ownerId, string parkingId, string unavaibilitiesId, bool isOnlyEvent)
        {
            if (string.IsNullOrEmpty(unavaibilitiesId))
            {
                return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingId, "unavailabilities");
            }
            else
            {
                if (isOnlyEvent)
                    return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingId, "unavailabilities", unavaibilitiesId, "thisOccurrenceOnly");
                else
                    return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingId, "unavailabilities", unavaibilitiesId, "futurOccurrences");

            }
        }

        public static string BuildDeleteUnabaibilities(string ownerId, string parkingId, string unavaibilitiesId)
        {
            return Url.Combine(RealApiV1, "owners", ownerId, "parkings", parkingId, "unavailabilities", unavaibilitiesId);
        }

        //F7
        public static readonly string RecommendPrice = Url.Combine(RealApiV1, "locations/50.711205:4.587973/price.json");

        public static string BuildRecommendPriceUrl(double lat, double lng)
        {
            return Url.Combine(RealApiV1, "locations", string.Format("{0}:{1}", lat.ParseToCultureInfo(new CultureInfo("en-US")), lng.ParseToCultureInfo(new CultureInfo("en-US"))), "price.json");
        }

        //F8
        public static readonly string ShortestDistance = Url.Combine(RealApiV1, "distances/50.812290:4.329650,50.673859:4.615169/shortest.json");

        public static string BuildShortestDistanceUrl(double fromLat, double fromLng, double toLat, double toLng)
        {
            return Url.Combine(RealApiV1, "distances", string.Format("{0}:{1},{2}:{3}", fromLat.ParseToCultureInfo(new CultureInfo("en-US")), fromLng.ParseToCultureInfo(new CultureInfo("en-US")), toLat.ParseToCultureInfo(new CultureInfo("en-US")), toLng.ParseToCultureInfo(new CultureInfo("en-US"))), "shortest.json");
        }

        //F9
        public static readonly string AddressOf = Url.Combine(RealApiV1, "locations/50.812290:4.329650/address.json");

        public static string BuildAddressOfUrl(double lat, double lng)
        {
            return Url.Combine(RealApiV1, "locations", string.Format("{0}:{1}", lat.ParseToCultureInfo(new CultureInfo("en-US")), lng.ParseToCultureInfo(new CultureInfo("en-US"))), "address.json");
        }
           
        //F11
        public static string BuildTopUpUrl(string userId)
        {
            return Url.Combine(RealApiV1, "persons", userId, "topup");
        }

        public static string BuildTopUpCreditCardUrl (string userId)
        {
            return Url.Combine (RealApiV1, "persons", userId, "topupWithCreditCard");
        }

        //PATCH operations and data
        public static string PayBookingOperations = "[{\"op\": \"replace\",\"path\":\"/status\",\"value\":\"paid\"}]";
        public static string PayBookingData = "{\"action\": \"pay\"}";

        public static string LeaveParkingOperations = "[{ \"op\": \"replace\", \"path\": \"/leavingStatus\", \"value\": \"left\" }, { \"op\": \"replace\", \"path\": \"/rating\", \"value\": \"0/1\" }, { \"op\": \"replace\", \"path\": \"/review\", \"value\": \"blablabla111\" }]";

        public static string BuildLeaveParkingOperations(string leavingStatus, string rating, string review)
        {
            rating = "\"" + rating + "\"";
            review = "\"" + review + "\"";
            if (string.IsNullOrEmpty(leavingStatus))
            {
                string data = "[{ \"op\": \"replace\", \"path\": \"/leavingStatus\", \"value\": \"left\" }, { \"op\": \"replace\", \"path\": \"/rating\", \"value\": " + rating + " }, { \"op\": \"replace\", \"path\": \"/review\", \"value\": " + review + " }]";
                return data;
 
            }
            else
            {
                leavingStatus = "\"" + leavingStatus + "\"";
                string data = "[{ \"op\": \"replace\", \"path\": \"/leavingStatus\", \"value\": " + leavingStatus + " }, { \"op\": \"replace\", \"path\": \"/rating\", \"value\": " + rating + " }, { \"op\": \"replace\", \"path\": \"/review\", \"value\": " + review + " }]";
                return data;

            }
        }

        public static string LeaveParkingData = "{\"action\": \"leaveSpot\"}";

        public static string BuildStoreProductIdsUrl(string store)
        {
            return Url.Combine(RealApiV1, "stores", store, "products");
        }

        //HereMap API
        public static string HereMapGeocoderUri = "http://geocoder.cit.api.here.com/6.2/geocode.json";
        public static string HereMapApiGeneration = "9";
        public static string HereMapAppCode = "7JgTQWkddS3NgjSAYYfMrg";
        public static string HereMapAppId = "yD9hzJ1jw1QDU1i2b4Ia";


    }
}

 