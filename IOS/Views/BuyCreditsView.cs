
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.ValueConverters;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class BuyCreditsView : BaseView
    {
        public BuyCreditsView()
            : base("BuyCreditsView", null)
        {
        }

        public new BuyCreditsViewModel ViewModel
        {
            get
            {
                return base.ViewModel as BuyCreditsViewModel;
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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.RaisePropertyChanged("CreditsValues");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetTitle("Buy Credits");
			
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<BuyCreditsView, BuyCreditsViewModel>();


            var source = new ProductTableSource(tableProducts, this);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.ProductsInformation);

            #region Language Binding

            #endregion

            set.Apply();

            tableProducts.Source = source;
            tableProducts.ReloadData();
        }
    }
}

