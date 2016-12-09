using System;

using Foundation;
using UIKit;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class PlaceCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("PlaceCell");
        public static readonly UINib Nib;

        public HereMapView ViewModel { get; set;}

        static PlaceCell()
        {
            Nib = UINib.FromName("PlaceCell", NSBundle.MainBundle);
        }


        public PlaceCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<PlaceCell, HereMapView>();

                    set.Bind(lbStreet).To(vm=>vm.Result[0].BindingStreet);
                    set.Bind(lbCountry).To(vm=>vm.Result[0].BindingCity);

                    set.Apply();
                });
        }



        public static PlaceCell Create()
        {
            return (PlaceCell)Nib.Instantiate(null, null)[0];
        }
    }
}
