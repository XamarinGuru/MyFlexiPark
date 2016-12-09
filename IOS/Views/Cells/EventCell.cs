
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Touch.Views.Cells
{
    public partial class EventCell : BaseTableCell
    {
        public static readonly NSString Key = new NSString("EventCell");
        public static readonly UINib Nib;

        public EventItemViewModel ViewModel { get; set;}

        public UILabel Title
        {
            get{return lbTitle;}
        }

        public UILabel StartDate
        {
            get{return lbStartDate;}
        }

        public UILabel EndDate
        {
            get{return lbEndDate;}
        }

        public UIButton EditButton
        {
            get{
                return btnEdit;
            }
        }

        static EventCell()
        {
            Nib = UINib.FromName("EventCell", NSBundle.MainBundle);
        }

        public EventCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<EventCell, EventItemViewModel>();
                    set.Bind(lbTitle).To(vm=>vm.Unavaiability.Title);
                    set.Bind(lbStartDate).To(vm=>vm.StartDate).WithConversion("EventDateTime");
                    set.Bind(lbEndDate).To(vm=>vm.EndDate).WithConversion("EventDateTime");

                    set.Bind(btnEdit).To(vm=>vm.EditEventCommand);
                    set.Bind(btnEdit).For(v=>v.Hidden).To(vm=>vm.IsEditMode).WithConversion("BooleanToHidden");

                    set.Bind(btnDelete).To(vm=>vm.DeleteEventCommand);
                    set.Bind(btnDelete).For(v=>v.Hidden).To(vm=>vm.IsEditMode).WithConversion("BooleanToHidden");

                    set.Apply();
                });
        }

        public static EventCell Create()
        {
            return (EventCell)Nib.Instantiate(null, null)[0];
        }
    }
}

