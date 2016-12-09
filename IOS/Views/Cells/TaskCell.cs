
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class TaskCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("TaskCell");
        public static readonly UINib Nib;

        public TaskItemViewModel ViewModel {get;set;}

        public UIImageView Icon
        {
            get{return ivIcon;}
        }

        public UILabel title
        {
            get{
                return lbTitle;
            }
        }

        static TaskCell()
        {
            Nib = UINib.FromName("TaskCell", NSBundle.MainBundle);
        }

        public TaskCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<TaskCell,TaskItemViewModel>();
                    set.Bind(lbTitle).To(vm=>vm.Title);
                    set.Bind(lbPleaseWait).For(v=>v.Hidden).To(vm=>vm.IsShowPleaseWait).WithConversion("BooleanToHidden");

                    set.Bind(lbPleaseWait).To(vm=>vm.TextSource).WithConversion("Language", "PleaseWaitText");
                    set.Apply();
                });
        }

        public static TaskCell Create()
        {
            return (TaskCell)Nib.Instantiate(null, null)[0];
        }
    }
}

