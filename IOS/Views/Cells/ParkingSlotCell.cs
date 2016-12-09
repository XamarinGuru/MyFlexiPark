
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class ParkingSlotCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("ParkingSlotCell");
        public static readonly UINib Nib;


        public ParkingSlotItemViewModel ViewModel { get; set;}

        public UIView viewRating
        {
            get{
                return vRating;
            }
        }

        static ParkingSlotCell()
        {
            Nib = UINib.FromName("ParkingSlotCell", NSBundle.MainBundle);
        }

        public ParkingSlotCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<ParkingSlotCell, ParkingSlotItemViewModel>();

                    set.Bind(lbDistance).To(vm=>vm.ParkingSpot.Distance).WithConversion("RoundedDistance");
                    set.Bind(lbProvider).To(vm=>vm.ParkingSpot.Provider);
                    set.Bind(lbPrice).To(vm=>vm.ParkingSpot.Cost).WithConversion("ParkingMoney");

                    set.Bind(ivClock).For(v=>v.Hidden).To(vm=>vm.IsShowClock).WithConversion("BooleanToHidden");

                    set.Apply();
                });
        }



        public static ParkingSlotCell Create()
        {
            return (ParkingSlotCell)Nib.Instantiate(null, null)[0];
        }

        /*public void AdjustFontSize()
        {
            lbPrice.Font = FontHelper.AdjustFontSize(lbPrice.Font);
            lbDistance.Font = FontHelper.AdjustFontSize(lbDistance.Font);
            lbProvider.Font = FontHelper.AdjustFontSize(lbProvider.Font);
            HasAdjust = true;
        }*/
    }
}

