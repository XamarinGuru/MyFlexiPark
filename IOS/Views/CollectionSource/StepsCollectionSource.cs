using System;
using Foundation;
using Cirrious.MvvmCross.Binding.Touch.Views;
using FlexyPark.UI.Touch.Views.Cells;
using UIKit;
using FlexyPark.Core.ViewModels;
using CoreGraphics;
using FlexyPark.UI.Touch.Helpers;
using System.Security.Cryptography;
using FlexyPark.Core.Models;

namespace FlexyPark.UI.Touch.Views.TableSource
{
	public class StepsCollectionSource : MvxCollectionViewSource
    {
        private static readonly NSString CellIdentifier = new NSString ("StepCell");

		public StepsCollectionSource (UICollectionView collectionView, NSString key) : base (collectionView, key) 
        {			
			collectionView.RegisterNibForCell (UINib.FromName ("StepCell", NSBundle.MainBundle), CellIdentifier);
        }

		protected override UICollectionViewCell GetOrCreateCellFor (UICollectionView collectionView, NSIndexPath indexPath, object item)
		{
			var cell = collectionView.DequeueReusableCell (CellIdentifier, indexPath) as StepCell;

			if (cell == null)
				cell = StepCell.Create();

			cell.ViewModel = item as RouteItem;

			return cell;
		}
       
    }
}

