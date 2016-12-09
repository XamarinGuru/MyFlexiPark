
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.Models;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class RouteCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("RouteCell");
        public static readonly UINib Nib;

        public RouteItem ViewModel { get; set;}

        static RouteCell()
        {
            Nib = UINib.FromName("RouteCell", NSBundle.MainBundle);
        }

        public RouteCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<RouteCell,RouteItem>();

                    set.Bind(lbDistance).To(vm=>vm.Distance);
                    set.Bind(lbInstruction).To(vm=>vm.Instruction);

                    set.Apply();
                });
        }

        public static RouteCell Create()
        {
            return (RouteCell)Nib.Instantiate(null, null)[0];
        }
    }
}

