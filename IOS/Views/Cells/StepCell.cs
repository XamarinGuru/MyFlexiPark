
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.Models;

namespace FlexyPark.UI.Touch
{
	public partial class StepCell : MvxCollectionViewCell
	{
		public static readonly NSString Key = new NSString ("StepCell");
		public static readonly UINib Nib;

		public RouteItem ViewModel { get; set;}

		static StepCell ()
		{
			Nib = UINib.FromName ("StepCell", NSBundle.MainBundle);
		}

		public StepCell (IntPtr handle) : base (handle)
		{
			this.DelayBind(() =>
				{
					var set = this.CreateBindingSet<StepCell,RouteItem>();

                    set.Bind(lbDistance).To(vm=>vm.Distance).WithConversion("Step");
					set.Bind(lbInstruction).To(vm=>vm.Instruction);

					set.Apply();
				});
		}

		public static StepCell Create ()
		{
			return (StepCell)Nib.Instantiate (null, null) [0];
		}
	}
}

