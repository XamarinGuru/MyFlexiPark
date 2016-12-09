
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class ExtendTimeCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("ExtendTimeCell");
        public static readonly UINib Nib;

        public ExtendTimeItemViewModel ViewModel { get; set;}

        static ExtendTimeCell()
        {
            Nib = UINib.FromName("ExtendTimeCell", NSBundle.MainBundle);
        }

        public ExtendTimeCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<ExtendTimeCell, ExtendTimeItemViewModel>();
                    set.Bind(lbPrice).To(vm=>vm.Price).WithConversion("Money");
                    set.Bind(lbHour).To(vm=>vm.Hours).WithConversion("Duration");
                    set.Bind(lbTime).To(vm=>vm.Time).WithConversion("DateTimeToString", "Time");
                    set.Apply();
                });
        }

        public static ExtendTimeCell Create()
        {
            return (ExtendTimeCell)Nib.Instantiate(null, null)[0];
        }

		public void AdjustFontSize()
		{
			lbPrice.Font = FontHelper.AdjustFontSize(lbPrice.Font);
			lbHour.Font = FontHelper.AdjustFontSize(lbHour.Font);
			lbTime.Font = FontHelper.AdjustFontSize(lbTime.Font);
		}
    }
}

