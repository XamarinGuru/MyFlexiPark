using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Touch.Views.Cells;
using FlexyPark.UI.Touch.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreGraphics;

namespace FlexyPark.UI.Touch.Views.TableSource
{
    public class ReportSelectionTableSource : MvxTableViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("ReportSelectionCell");

        private ReportSelectionView reportSelectionView;

        public ObservableCollection<string> Problems;

        public ReportSelectionTableSource (UITableView tableView, ReportSelectionView reportSelectionView, ObservableCollection<string> problems) : base (tableView)
        {
            tableView.RegisterNibForCellReuse (UINib.FromName ("ReportSelectionCell", NSBundle.MainBundle), ReportSelectionCell.Key);

            this.reportSelectionView = reportSelectionView;

            this.Problems = problems;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (Problems != null && Problems.Count > 0)
            {
                var height = GetHeightForMultilineLabelWithString(Problems[indexPath.Row], tableView.Frame.Width - 40f);

                return height + 30f;
            }

            return 50f;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {           
            var cell = tableView.DequeueReusableCell (ReportSelectionCell.Key) as ReportSelectionCell;

            if (cell == null)
                cell = ReportSelectionCell.Create();

            cell.Title.Text = item as string;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath) as TaskCell;

            reportSelectionView.ViewModel.ReportItemSelectedCommand.Execute((int)indexPath.Row);

            tableView.DeselectRow(indexPath,true);
        }

        public nfloat GetHeightForMultilineLabelWithString(string str, nfloat width)
        {
            var nativeString = new NSString (str);
            CGSize maxLabelSize = new CGSize (width, float.MaxValue);
            UIFont font = UIFont.FromName ("Helvetica", 20f);

            CGRect textRect = nativeString.GetBoundingRect (maxLabelSize, NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes{ Font = font } , null);

            return textRect.Size.Height;
        }
    }
}

