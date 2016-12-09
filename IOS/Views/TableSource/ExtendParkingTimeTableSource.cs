using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using FlexyPark.UI.Touch.Views.Cells;
using UIKit;
using FlexyPark.Core.ViewModels;
using CoreGraphics;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class ExtendParkingTimeTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("ExtendTimeCell");

        private ExtendParkingTimeView extendParkingTimeView;

        public ExtendParkingTimeTableSource (UITableView tableView, ExtendParkingTimeView extendParkingTimeView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("ExtendTimeCell", NSBundle.MainBundle), ExtendTimeCell.Key);

            this.extendParkingTimeView = extendParkingTimeView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 65f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath,object item)
        {           
            var cell = tableView.DequeueReusableCell (ExtendTimeCell.Key) as ExtendTimeCell;

            if (cell == null)
                cell = ExtendTimeCell.Create();

            cell.ViewModel = item as ExtendTimeItemViewModel;

            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as ExtendTimeCell;

            extendParkingTimeView.ViewModel.GotoExtendParkingTimeConfirmCommand.Execute(cell.ViewModel);

            tableView.DeselectRow(indexPath,true);

        }
    }
}

