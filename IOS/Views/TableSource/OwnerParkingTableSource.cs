using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class OwnerParkingTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("OwnerParkingCell");

        private MyOwnParkingView myOwnParkingView;

        public OwnerParkingTableSource (UITableView tableView, MyOwnParkingView myOwnParkingView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("OwnerParkingCell", NSBundle.MainBundle), OwnerParkingCell.Key);

            this.myOwnParkingView = myOwnParkingView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 100f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (OwnerParkingCell.Key) as OwnerParkingCell;

            if (cell == null)
                cell = OwnerParkingCell.Create();

            cell.ViewModel = item as OwnerParkingItemViewModel;

            cell.trailingConstraint.Constant = cell.ViewModel.IsEditMode ? 50f : 8f;

            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as OwnerParkingCell;

            myOwnParkingView.ViewModel.OwnParkingItemSelectedCommand.Execute(cell.ViewModel);
            
            base.RowSelected(tableView, indexPath);
        }

        //NSIndexPath selectedRowIndexPath;

        /*public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (selectedRowIndexPath != null)
            {
                var previousSelectedCell = tableView.CellAt(selectedRowIndexPath) as VehicleCell;
                previousSelectedCell.Accessory = UITableViewCellAccessory.None;
            }

            var cell = tableView.CellAt(indexPath) as VehicleCell;
            cell.Accessory = UITableViewCellAccessory.Checkmark;

            selectedRowIndexPath = indexPath;

            tableView.DeselectRow(indexPath,true);

        }*/
    }
}

