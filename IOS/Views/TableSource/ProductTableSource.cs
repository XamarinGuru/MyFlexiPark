using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class ProductTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("ProductCell");

        private BuyCreditsView mBuyCreditsView;

        public ProductTableSource (UITableView tableView, BuyCreditsView buyCreditsView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("ProductCell", NSBundle.MainBundle), ProductCell.Key);

            this.mBuyCreditsView = buyCreditsView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 65f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (ProductCell.Key) as ProductCell;

            if (cell == null)
                cell = ProductCell.Create();
            
            //cell.Type.Text = item as string;
            cell.ViewModel = item as Tuple<string,string,string,string,double>;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as ProductCell;
            mBuyCreditsView.ViewModel.GotoDetailsCommand.Execute(cell.ViewModel);
        }
    }
}

