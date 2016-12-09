using System;
using System.Collections.Generic;
using System.Globalization;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core
{
    public class AppConstants
    {
        //Api
        public const string iOSToken = "myflexipark-iphone-v1.0-fzgzi6ymgyk4ouat";
        public const string AndroidToken = "myflexipark-androidphone-v1.0-um68wxk4efiv5m4p";

        //Google OAuth2
        public const string ClientID = "691897884212-e3j302hjju75316l3ik33kt60jivhtbg.apps.googleusercontent.com";
        public const string RedirectUrl = "https://www.example.com/oauth2callback";

        //Text Provider
        public const string NameSpace = "FlexyPark";
        public const string FolderResources = "MyFlexyParkResources/Text";

        public const string Waze = "Waze";
        public const string GoogleMaps = "GoogleMaps";
        public const string Navmii = "Navmii";
        public const string Language = "Language";

        //Desired Accuracy
        public const double DesiredAccuracy = 5;

        //Stripe
		public const string StripeBankTokenURL = "https://api.stripe.com/v1/tokens";
		public const string StripeAPIKey = "pk_test_rzpZhMtnEiR4vaO3r7GvwsEt";
		//public const string StripeAPIKey = "pk_live_pRF86Xpj7Y2vZGJ8Xc2w4xel";

        //Constants String
        public const string IsLogin = "IsLogin";
        public const string CurrentBookingId = "CurrentBookingId";
        public const string BookingExpiredTime = "BookingExpiredTime";

        public const string ValidityTime = "ValidityTime";
        public const string CCCardNumber = "CCCardNumber";
        public const string CCHolderName = "CCHolderName";
        public const string CCCryptogram = "CCCryptogram";

		public static double[] locCenterOfBelgium = new double[]{ 50.8480f, 4.3513f };

		public const int LimitPerPage = 20;
		public const int MaxDistance = 4000000;

        public static string[] Languages = new string[]
        {
            "English", "French", "Dutch"
        };

        public static string[] Repeats = new string[]
        {
            //"none", "everyDay", "everyWeek", "everyMonth"
            "once", "everyDay", "everyWeek", "every2Week", "everyMonth", "everyYear"

        };

        public static string[] RepeatsSource = new string[]
        {
//            "Never", "Every Day", "Every Week", "Every Month"
            "Never", "Every Day", "Every Week", "Every 2 Week", "Every Month", "Every Year"

        };

        public static string[] TaskTitles = new string[]
        {
            "Please go to your spot",
            "GPS",
            "Accuracy",
            "Please set the spot address",
            "Please set the spot size",
            "Please set the spot cost",
            "Please set the spot calendar"
        };

        public static string[] Problems = new string[]
        {
            /*"Exit",
                "Extension",*/
            "The spot is already taken",
            "There is a car on the street in front of the spot",
            "There is no mark on the garage door",
            "There is no spot",
            "The parking is full",
            "I refuse the spot",
            "I can't leave the spot",
            "I leave and report a problem"
        };

        public static List<string> ProductIds = new List<string>()
        {
            "com.myflexypark.crowd.10credits",
            "com.myflexypark.crowd.25credits"
        };

        public static string iOSTermsOfUse = "http://app.myflexipark.com/ios/law.php";
        public static string ADTermsOfUse = "http://app.myflexipark.com/android/law.php";
    }

    public enum SearchMode
    {
        Now,
        Later
    }

    public enum DirectionsMode
    {
        Driving,
        Walking
    }

    public enum ParkingStatus
    {
        Rented,
        Reserved
    }

    public enum ChooseVehicleMode
    {
        FirstSelect,
        ReSelect,
        NoAction
    }

    public enum MyProfileTab
    {
        Common = 0,
        Rent = 1,
        Own = 2
    }

    public enum AddSpotStatus
    {
        GotoSpot = 0,
        GPS = 1,
        Accuracy = 2,
        SpotAddress = 3,
        SpotSize = 4,
        SpotCost = 5,
        SpotCalendar = 6,
        Activation = 7,
        Complete = 8
    }

    public enum ReportMode
    {
        Unreachable,
        PlateNumber,
        Refund,
        NotFound,
        Full,
        PictureRefuse,
        PictureLeave,
        CallOwner
    }
}

