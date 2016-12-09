
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class ReservationCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("ReservationCell");
        public static readonly UINib Nib;

        public ReservationItemViewModel ViewModel { get; set;}

        static ReservationCell()
        {
            Nib = UINib.FromName("ReservationCell", NSBundle.MainBundle);
        }

        public ReservationCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<ReservationCell, ReservationItemViewModel>();

                    set.Bind(lbStart).To(vm=>vm.TextSource).WithConversion("Language", "StartTimeText");
                    set.Bind(lbEnd).To(vm=>vm.TextSource).WithConversion("Language", "EndTimeText");

                    set.Bind(lbEndTime).To(vm=>vm.EndTime).WithConversion("DateTimeToString", "Reservation");
                    set.Bind(lbStartTime).To(vm=>vm.StartTime).WithConversion("DateTimeToString", "Reservation");

                    set.Bind(lbAddress).To(vm=>vm.Reservation.Parking.Location);

                    set.Apply();
                });
        }

        public static ReservationCell Create()
        {
            return (ReservationCell)Nib.Instantiate(null, null)[0];
        }
    }
}

