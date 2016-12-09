
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class VehicleTypeCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("VehicleTypeCell");
        public static readonly UINib Nib;

        public VehicleTypeItemViewModel ViewModel {get;set;}

        public UILabel Type
        {
            get{
                return lbType;
            }
        }

        static VehicleTypeCell()
        {
            Nib = UINib.FromName("CarTypeCell", NSBundle.MainBundle);
        }

        public VehicleTypeCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<VehicleTypeCell, VehicleTypeItemViewModel>();
                    set.Bind(lbType).To(vm=>vm.Type);
                    set.Apply();
                });
        }

        public static VehicleTypeCell Create()
        {
            return (VehicleTypeCell)Nib.Instantiate(null, null)[0];
        }
    }
}

