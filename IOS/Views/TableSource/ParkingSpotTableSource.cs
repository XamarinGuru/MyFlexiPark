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
	public class ParkingSpotTableSource : MvxTableViewSource
	{
		private static readonly NSString CellIdentifier = new NSString ("ParkingSlotCell");

		private ParkingListsView parkListsView;

		public ParkingSpotTableSource (UITableView tableView, ParkingListsView parkListsView) : base (tableView)
		{
			tableView.RegisterNibForCellReuse (UINib.FromName ("ParkingSlotCell", NSBundle.MainBundle), ParkingSlotCell.Key);

			this.parkListsView = parkListsView;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 65f;
		}

		protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
		{           
			var cell = tableView.DequeueReusableCell (ParkingSlotCell.Key) as ParkingSlotCell;

			if (cell == null)
				cell = ParkingSlotCell.Create ();

			cell.ViewModel = item as ParkingSlotItemViewModel;

            //HACK : to be sure that this will get into this only 1 time
            /*if (!cell.HasAdjust && !DeviceHelper.IsPad)
                cell.AdjustFontSize(cell.ContentView);*/
            

//          if (cell.viewRating.ViewWithTag (111) == null) {
//				RatingConfig ratingConfig = new RatingConfig (UIImage.FromFile ("black_star.png"), UIImage.FromFile ("yellow_star.png"), UIImage.FromFile ("yellow_star.png"));
//
//				// Create the view.
//				var ratingView = new PDRatingView (new CGRect (0f, 0f, cell.viewRating.Bounds.Width, cell.viewRating.Bounds.Height), ratingConfig);
//
//				// Allow rating on a scale of 1 to 10.
//				//ratingConfig.ScaleSize = 10;
//
//				//half-round or whole-round rating
//				decimal rating = 3.58m;
//				decimal halfRoundedRating = Math.Round (rating * 2m, MidpointRounding.AwayFromZero) / 2m;
//				decimal wholeRoundedRating = Math.Round (rating, MidpointRounding.AwayFromZero);
//				ratingView.AverageRating = halfRoundedRating;
//
//				//not allow rating
//				ratingView.UserInteractionEnabled = false;
//
//				// [Optional] Do something when the user selects a rating.
//				/*ratingView.RatingChosen += (sender, e) => {
//                (new UIAlertView("Rated!", e.Rating.ToString() + " stars", null, "Ok")).Show();
//            };*/
//
//				// [Required] Add the view to the 
//				ratingView.Tag = 111;
//				cell.viewRating.Add (ratingView);
//			}

			return cell;

		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath) as ParkingSlotCell;

			parkListsView.ViewModel.GotoBookingCommand.Execute(cell.ViewModel);

			//if (cell.ViewModel.ParkingSpot.BookingType == "immediate")
	  //          parkListsView.ViewModel.GotoBookingCommand.Execute (cell.ViewModel);
			//else if (cell.ViewModel.ParkingSpot.BookingType == "delayed")
			//	parkListsView.ViewModel.GotoDelayedParkingMapCommand.Execute(cell.ViewModel);
			
			tableView.DeselectRow (indexPath, true);

		}
	}
}

