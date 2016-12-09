using System;
using Foundation;
using UIKit;
using FlexyPark.Core.Models;
using Cirrious.MvvmCross.Binding.Touch.Views;
using FlexyPark.UI.Touch.Views.Cells;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class RouteTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("RouteCell");

        private ParkingMapView parkingMapView;

        public RouteTableSource (UITableView tableView, ParkingMapView parkingMapView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("RouteCell", NSBundle.MainBundle), RouteCell.Key);

            this.parkingMapView = parkingMapView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 40f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (RouteCell.Key) as RouteCell;

            if (cell == null)
                cell = RouteCell.Create();

            cell.ViewModel = item as RouteItem;

            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as VehicleCell;
            base.RowSelected(tableView, indexPath);
        }
    }
}

