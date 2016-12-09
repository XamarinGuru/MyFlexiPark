using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class VehicleTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("VehicleCell");

        private ChooseVehicleView chooseVehicleView;

        public VehicleTableSource (UITableView tableView, ChooseVehicleView chooseVehicleView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("VehicleCell", NSBundle.MainBundle), VehicleCell.Key);

            this.chooseVehicleView = chooseVehicleView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 65f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (VehicleCell.Key) as VehicleCell;

            if (cell == null)
                cell = VehicleCell.Create();

            cell.ViewModel = item as VehicleItemViewModel;

            cell.trailingConstraint.Constant = cell.ViewModel.IsEditMode ? 90f : 8f;

            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as VehicleCell;
            if(!cell.ViewModel.IsEditMode)
                chooseVehicleView.ViewModel.VehicleItemClickCommand.Execute(cell.ViewModel);
            
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

