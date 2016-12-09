using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class ProductCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("ProductCell");
        public static readonly UINib Nib;

        public Tuple<string,string,string,string,double> ViewModel;

        static ProductCell()
        {
            Nib = UINib.FromName("ProductCell", NSBundle.MainBundle);
        }



        public ProductCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<ProductCell, Tuple<string,string,string,string>>();

                    set.Bind(lbName).To(vm=>vm.Item2);
                    set.Bind(lbPrice).To(vm=>vm.Item4);

                    set.Apply();
                });
        }



        public static ProductCell Create()
        {
            return (ProductCell)Nib.Instantiate(null, null)[0];
        }
    }
}
