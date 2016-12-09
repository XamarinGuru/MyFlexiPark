using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class MyReservationTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("ReservationCell");

        private MyReservationsView myReservationsView;

        public MyReservationTableSource (UITableView tableView, MyReservationsView myReservationsView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("ReservationCell", NSBundle.MainBundle), ReservationCell.Key);

            this.myReservationsView = myReservationsView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 150f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (ReservationCell.Key) as ReservationCell;

            if (cell == null)
                cell = ReservationCell.Create();

            cell.ViewModel = item as ReservationItemViewModel;
            //cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as ReservationCell;
            myReservationsView.ViewModel.GotoParkingReservedCommand.Execute(cell.ViewModel);
        }
    }
}

