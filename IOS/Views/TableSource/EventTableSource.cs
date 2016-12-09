using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Helpers;
using System.Collections.ObjectModel;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class EventTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("EventCell");

        private AddSpotCalendarView addSpotCalendarView;

        public EventTableSource (UITableView tableView, AddSpotCalendarView addSpotCalendarView) : base(tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("EventCell", NSBundle.MainBundle), EventCell.Key);

            this.addSpotCalendarView = addSpotCalendarView;

        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 165f;
        }

        /*public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell (EventCell.Key) as EventCell;

            if (cell == null)
                cell = EventCell.Create();

            cell.Title.Text = mEvents[indexPath.Row].Summary;
            cell.StartDate.Text = ConvertDateTimeToString(mEvents[indexPath.Row].Start.DateTime);
            cell.EndDate.Text = ConvertDateTimeToString(mEvents[indexPath.Row].End.DateTime);

            return cell;
        }*/


        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (EventCell.Key) as EventCell;

            if (cell == null)
                cell = EventCell.Create();

            cell.ViewModel = item as EventItemViewModel;

            /*cell.Title.Text = mEvents[indexPath.Row].Summary;
            cell.StartDate.Text = ConvertDateTimeToString(mEvents[indexPath.Row].Start.DateTime);
            cell.EndDate.Text = ConvertDateTimeToString(mEvents[indexPath.Row].End.DateTime);*/

            if (!cell.ViewModel.IsEditMode)
                cell.Accessory = addSpotCalendarView.ViewModel.SelectedEvents.Contains(cell.ViewModel) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            else
                cell.Accessory = UITableViewCellAccessory.None;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as EventCell;
            //addSpotCalendarView.ViewModel.EventItemClickedCommand.Execute(cell.ViewModel);
        }

    }
}

