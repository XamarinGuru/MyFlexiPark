
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Services
{
    public interface IFixMvvmCross
    {
        MyProfileViewModel MyProfileViewModel { get; set; }
        ParkingReservedViewModel ParkingReservedViewModel { get; set; }
        AddSpotCalendarViewModel AddSpotCalendarViewModel { get; set; }
    }

    public class FixMvvmCross : IFixMvvmCross
    {
        public MyProfileViewModel MyProfileViewModel { get; set; }
        public ParkingReservedViewModel ParkingReservedViewModel { get; set; }
        public AddSpotCalendarViewModel AddSpotCalendarViewModel { get; set; }
    }
}