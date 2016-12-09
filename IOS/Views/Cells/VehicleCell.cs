
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class VehicleCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("VehicleCell");
        public static readonly UINib Nib;

        public VehicleItemViewModel ViewModel {get;set;}

        public NSLayoutConstraint trailingConstraint
        {
            get
            {return cstVehicleTypeTrailing;}
        }

        static VehicleCell()
        {
            Nib = UINib.FromName("VehicleCell", NSBundle.MainBundle);
        }

        public VehicleCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<VehicleCell, VehicleItemViewModel>();
                    set.Bind(lbPlateNumber).To(vm=>vm.Vehicle.PlateNumber);
                    set.Bind(lbVehicleType).To(vm=>vm.Vehicle.Type);
                    set.Bind(btnEdit).For(v=>v.Hidden).To(vm=>vm.IsEditMode).WithConversion("BooleanToHidden");
                    set.Bind(btnEdit).To(vm=>vm.GotoEditVehicleCommand);

                    set.Bind(btnDelete).For(v=>v.Hidden).To(vm=>vm.IsEditMode).WithConversion("BooleanToHidden");
                    set.Bind(btnDelete).To(vm=>vm.DeleteVehicleCommand);
                    set.Apply();
                });
        }

        public static VehicleCell Create()
        {
            return (VehicleCell)Nib.Instantiate(null, null)[0];
        }
    }
}

