using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public class BaseTableCell : MvxTableViewCell
    {
        private bool HasAdjust { get; set;}

        public BaseTableCell(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            if (!HasAdjust && !DeviceHelper.IsPad)
                AdjustFontSize(this.ContentView);
            
            base.AwakeFromNib();
        }

        public void AdjustFontSize(UIView view)
        {
            foreach (var subview in view.Subviews)
            {
                if (subview.Subviews != null && subview.Subviews.Length != 0)
                {
                    AdjustFontSize(subview);
                }
                else
                {
                    if(subview is UIButton)
                    {
                        (subview as UIButton).Font = FontHelper.AdjustFontSize((subview as UIButton).Font);
                    }
                    else if(subview is UILabel)
                    {
                        (subview as UILabel).Font = FontHelper.AdjustFontSize((subview as UILabel).Font);
                    }
                }
            }

            HasAdjust = true;
        }
    }
}

