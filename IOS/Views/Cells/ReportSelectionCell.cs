
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class ReportSelectionCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("ReportSelectionCell");
        public static readonly UINib Nib;

        static ReportSelectionCell()
        {
            Nib = UINib.FromName("ReportSelectionCell", NSBundle.MainBundle);
        }

        public UILabel Title
        {
            get{
                return lbTitle;
            }
        }

        public ReportSelectionCell(IntPtr handle)
            : base(handle)
        {
        }

        public static ReportSelectionCell Create()
        {
            return (ReportSelectionCell)Nib.Instantiate(null, null)[0];
        }
    }
}

