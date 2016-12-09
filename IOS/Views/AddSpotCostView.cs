
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.UI.Touch.Views
{
    public partial class AddSpotCostView : BaseView, IAddSpotCostView
    {
        public AddSpotCostView()
            : base("AddSpotCostView", null)
        {
        }

        public new AddSpotCostViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddSpotCostViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.View = this;

            SetTitle(ViewModel.TextSource.GetText("PageTitle"));
			
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<AddSpotCostView, AddSpotCostViewModel>();

            set.Bind(lbSelectedPrice).To(vm => vm.SelectedPrice).WithConversion("Money");
            set.Bind(lbRecommendedPrice).To(vm => vm.RecommendedPrice).WithConversion("Money");

            set.Bind(sdTime).For(s => s.Value).To(vm => vm.SelectedValue);
            set.Bind(sdTime).For("ValueChanged").To(vm => vm.HandleValueChangedCommand);

            #region Language Binding

            set.Bind(lbRecomPrice).To(vm => vm.TextSource).WithConversion("Language", "RecommendedPriceText");
            set.Bind(lbSelePrice).To(vm => vm.TextSource).WithConversion("Language", "SelectedPriceText");

            #endregion

            set.Apply();

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel.DoneCommand.Execute();
            Mvx.Resolve<ICacheService>().CreateParkingRequest.HourlyRate = ViewModel.SelectedPrice;
        }

        #region IAddSpotCostView implementation

        public void SetSliderValue()
        {
            decimal value = (decimal)Math.Round((decimal)sdTime.Value * 2m, MidpointRounding.AwayFromZero) / 2m;
            sdTime.SetValue((float)Math.Round(value, 1), true);
            ViewModel.SelectedValue = (float)value;
        }

        #endregion
    }
}

