
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class OwnerParkingCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("OwnerParkingCell");
        public static readonly UINib Nib;

        public OwnerParkingItemViewModel ViewModel { get; set; }

        public NSLayoutConstraint trailingConstraint
        {
            get
            {
                return cstStatusTrailing;
            }
        }

        static OwnerParkingCell()
        {
            Nib = UINib.FromName("OwnerParkingCell", NSBundle.MainBundle);
        }

        public OwnerParkingCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<OwnerParkingCell, OwnerParkingItemViewModel>();

                    set.Bind(lbLocation).To(vm => vm.Parking.Location);
                    set.Bind(lbStatus).To(vm => vm.Parking.Availability);

                    /*set.Bind(btnEdit).For(v=>v.Hidden).To(vm=>vm.IsEditMode).WithConversion("BooleanToHidden");
                    set.Bind(btnEdit).To(vm=>vm.GotoEditParkingSpotCommand);*/

                    /*set.Bind(btnDelete).For(v => v.Hidden).To(vm => vm.IsEditMode).WithConversion("BooleanToHidden");
                    set.Bind(btnDelete).To(vm => vm.DeleteParkingSpotCommand);*/

                    set.Apply();

                    //lbLocation.Font = FontHelper.AdjustFontSize(lbLocation.Font);
                });
        }

        public static OwnerParkingCell Create()
        {
            return (OwnerParkingCell)Nib.Instantiate(null, null)[0];
        }
    }
}

