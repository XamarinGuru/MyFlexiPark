
using System;

using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.UI.Touch.Views.TableSource;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ReportSelectionView : BaseView, IReportSelectionView
    {
        ReportSelectionTableSource source;

        public ReportSelectionView()
            : base("ReportSelectionView", null)
        {
        }

        public new ReportSelectionViewModel ViewModel
        {
            get{
                return base.ViewModel as ReportSelectionViewModel;
            }
            set{
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
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<ReportSelectionView, ReportSelectionViewModel>();

            source = new ReportSelectionTableSource(tableReports, this, ViewModel.Problems);
            set.Bind(source).For(v => v.ItemsSource).To(vm => vm.Problems);

            set.Apply();

            tableReports.Source = source;
            tableReports.ReloadData();
        }

        #region IReportSelectionView implementation

        public void ReloadTable()
        {
            source.Problems = ViewModel.Problems;
            tableReports.ReloadData();
        }

        #endregion
    }
}

