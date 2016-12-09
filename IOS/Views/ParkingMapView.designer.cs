// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace FlexyPark.UI.Touch.Views
{
    [Register ("ParkingMapView")]
    partial class ParkingMapView
    {
        [Outlet]
        UIKit.UIButton btnInfo { get; set; }


        [Outlet]
        UIKit.UIButton btnStart { get; set; }


        [Outlet]
        UIKit.UICollectionView collectionRoutes { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstBottomSpace { get; set; }


        [Outlet]
        UIKit.UILabel lbExpectedTime { get; set; }


        [Outlet]
        UIKit.UILabel lbTravelDistance { get; set; }


        [Outlet]
        MapKit.MKMapView mapView { get; set; }


        [Outlet]
        UIKit.UITableView tableRoutes { get; set; }


        [Outlet]
        UIKit.UIView vRouteInfo { get; set; }


        [Outlet]
        UIKit.UIView vStart { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnInfo != null) {
                btnInfo.Dispose ();
                btnInfo = null;
            }

            if (btnStart != null) {
                btnStart.Dispose ();
                btnStart = null;
            }

            if (collectionRoutes != null) {
                collectionRoutes.Dispose ();
                collectionRoutes = null;
            }

            if (cstBottomSpace != null) {
                cstBottomSpace.Dispose ();
                cstBottomSpace = null;
            }

            if (lbExpectedTime != null) {
                lbExpectedTime.Dispose ();
                lbExpectedTime = null;
            }

            if (lbTravelDistance != null) {
                lbTravelDistance.Dispose ();
                lbTravelDistance = null;
            }

            if (mapView != null) {
                mapView.Dispose ();
                mapView = null;
            }

            if (tableRoutes != null) {
                tableRoutes.Dispose ();
                tableRoutes = null;
            }

            if (vRouteInfo != null) {
                vRouteInfo.Dispose ();
                vRouteInfo = null;
            }

            if (vStart != null) {
                vStart.Dispose ();
                vStart = null;
            }
        }
    }
}