using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class VehicleTypeTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("VehicleTypeCell");

        private AddVehicleView addVehicleView;

        private AddSpotSizeView addSpotSizeView;

        public VehicleTypeTableSource (UITableView tableView, AddVehicleView addVehicleView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("VehicleTypeCell", NSBundle.MainBundle), VehicleTypeCell.Key);

            this.addVehicleView = addVehicleView;
        }

        public VehicleTypeTableSource (UITableView tableView, AddSpotSizeView addSpotSizeView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("VehicleTypeCell", NSBundle.MainBundle), VehicleTypeCell.Key);

            this.addSpotSizeView = addSpotSizeView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 65f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (VehicleTypeCell.Key) as VehicleTypeCell;

            if (cell == null)
                cell = VehicleTypeCell.Create();
            
            //cell.Type.Text = item as string;
            cell.ViewModel = item as VehicleTypeItemViewModel;

            cell.Accessory = cell.ViewModel.IsChecked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

            return cell;

        }

        NSIndexPath selectedRowIndexPath;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (selectedRowIndexPath != null)
            {
                var previousSelectedCell = tableView.CellAt(selectedRowIndexPath) as VehicleTypeCell;
                previousSelectedCell.Accessory = UITableViewCellAccessory.None;
            }

            var cell = tableView.CellAt(indexPath) as VehicleTypeCell;
            cell.Accessory = UITableViewCellAccessory.Checkmark;

            selectedRowIndexPath = indexPath;

            tableView.DeselectRow(indexPath,true);

            if(addVehicleView != null)
            {
                addVehicleView.ViewModel.VehicleItemClickCommand.Execute(cell.ViewModel);
            }
            else if (addSpotSizeView != null)
            {
                addSpotSizeView.ViewModel.VehicleItemClickCommand.Execute(cell.ViewModel);
            }

        }
    }
}

