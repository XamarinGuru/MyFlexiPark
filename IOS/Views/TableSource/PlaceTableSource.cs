using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;
using System.Collections.ObjectModel;
using FlexyPark.Core.Services;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class PlaceTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("PlaceCell");

        private ParkingSearchView parkingSearchView;

        public PlaceTableSource (UITableView tableView, ParkingSearchView parkingSearchView) : base(tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("PlaceCell", NSBundle.MainBundle), PlaceCell.Key);

            this.parkingSearchView = parkingSearchView;

        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 80f;
        }


        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (PlaceCell.Key) as PlaceCell;

            if (cell == null)
                cell = PlaceCell.Create();

            cell.ViewModel = item as HereMapView;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as PlaceCell;
            parkingSearchView.ViewModel.SelectPlaceCommand.Execute(cell.ViewModel);
        }

    }
}

