using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.UI.Touch.Helpers;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class TaskTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("TaskCell");

        private AddSpotView addSpotView;

        public TaskTableSource (UITableView tableView, AddSpotView addSpotView) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("TaskCell", NSBundle.MainBundle), TaskCell.Key);

            this.addSpotView = addSpotView;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return 45f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (TaskCell.Key) as TaskCell;

            if (cell == null)
                cell = TaskCell.Create();

            cell.ViewModel = item as TaskItemViewModel;

            cell.UserInteractionEnabled = cell.ViewModel.Enabled;

            cell.Accessory = cell.ViewModel.IsShowPleaseWait ? UITableViewCellAccessory.None : (cell.ViewModel.Finished ? UITableViewCellAccessory.Checkmark : (cell.ViewModel.Enabled ? UITableViewCellAccessory.DisclosureIndicator: UITableViewCellAccessory.None));

            //cell.ContentView.BackgroundColor = cell.ViewModel.Finished ? UIColor.FromRGB(31,31,31) : (cell.ViewModel.Enabled ? UIColor.FromRGB(31,31,31) : UIColor.DarkGray);

            cell.title.TextColor = cell.ViewModel.Finished ? UIColor.White : (cell.ViewModel.Enabled ? UIColor.White : UIColor.DarkGray);

            //set image for cell
            switch(indexPath.Row)
            {
                case 0:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_home.png");
                    break;
                case 1:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_navigate.png");
                    break;
                case 2:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_chart.png");
                    break;
                case 3:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_location.png");
                    break;
                case 4:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_car.png");
                    break;
                case 5:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_euro.png");
                    break;
                case 6:
                    cell.Icon.Image = UIImage.FromFile("blue_icon_calendar.png");
                    break;
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as TaskCell;

            addSpotView.ViewModel.ChooseTaskCommand.Execute(cell.ViewModel);

            tableView.DeselectRow(indexPath,true);
        }
    }
}

