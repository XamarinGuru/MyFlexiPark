
using System;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core;
using MapKit;
using CoreLocation;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using WYPopoverControllerBinding;
using CoreGraphics;
using FlexyPark.UI.Touch.Views.TableSource;
using System.Globalization;
using FlexyPark.Core.Models;
using System.Net.Sockets;
using System.Collections.Generic;
using FlexyPark.UI.Touch.Extensions;
using Xamarin.Geolocation;

namespace FlexyPark.UI.Touch.Views
{
    public partial class ParkingMapView : BaseView, IMKMapViewDelegate, IUICollectionViewDelegateFlowLayout, IUICollectionViewDelegate, IParkingMapView, ICLLocationManagerDelegate
	{
		MvxSubscriptionToken mMapKitToken;
        MvxSubscriptionToken mTimeToken;
		bool isShowRoute = false;
		CLLocationManager locationManager;
		MKCoordinateRegion overviewRegion;
		MKPointAnnotation destinationAnnotation; 

        UIBarButtonItem btnBarStart, btnBarOverviewResume;

		public ParkingMapView ()
			: base ("ParkingMapView", null)
		{
		}

		public new ParkingMapViewModel ViewModel {
			get {
				return base.ViewModel as ParkingMapViewModel;
			}
			set {
				base.ViewModel = value;
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Console.WriteLine ("Map Appear");
            if (locationManager == null)
            {
                locationManager = new CLLocationManager();
                //locationManager.DesiredAccuracy = waiting for customer
                locationManager.Delegate = this;
                if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")))
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }

            mTimeToken = Mvx.Resolve<IMvxMessenger>().Subscribe<TimeMessage>(message => {
                if(message.Sender.GetType() != typeof(ParkingReservedViewModel))
                    return;

                ViewModel.TotalParkingTime = message.TimeLeft;
            });
		}

        public override void ViewDidDisappear(bool animated)
        {
            if(locationManager !=null)
            {
                locationManager.Delegate = null;
                locationManager.Dispose();
                locationManager = null;
            }

            Mvx.Resolve<IMvxMessenger>().Unsubscribe<TimeMessage>(mTimeToken);

            base.ViewDidDisappear(animated);
        }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ViewModel.View = this;
			// Perform any additional setup after loading the view, typically from a nib.

           

			//subscribe mapkit token
			mMapKitToken = Mvx.Resolve<IMvxMessenger> ().SubscribeOnMainThread<NavigateMapMessage> (message => {
				if (message.Sender.GetType () != typeof(ParkingSummaryViewModel) && message.Sender.GetType () != typeof(ParkingReservedViewModel))
					return;

				if (!isShowRoute) {
					//FindRouteAndDrawRoute (message.SourceLat, message.SourceLng, message.DestinationLat, message.DestinationLng, message.DirectionsMode);
                    FindRouteAndDrawRoute (message.DestinationLat, message.DestinationLng, message.DirectionsMode);
				}
			});

            btnBarStart = new UIBarButtonItem (){ Title = ViewModel.TextSource.GetText("StartText") };
            btnBarOverviewResume = new UIBarButtonItem (){ Title = ViewModel.TextSource.GetText("OverviewText") };
            
			var set = this.CreateBindingSet<ParkingMapView, ParkingMapViewModel> ();

			var source = new RouteTableSource (tableRoutes, this);
			set.Bind (source).For (v => v.ItemsSource).To (vm => vm.Routes);

			var stepSource = new StepsCollectionSource (collectionRoutes, StepCell.Key);
			set.Bind (stepSource).For (v => v.ItemsSource).To (vm => vm.Routes);

			set.Bind (btnInfo).To (vm => vm.ShowRouteInfoCommand);

			set.Bind (btnStart).To (vm => vm.StartNavigateCommand);
			set.Bind (collectionRoutes).For (v => v.Hidden).To (vm => vm.IsNavigating).WithConversion ("BooleanToHidden");
			set.Bind (vStart).For (v => v.Hidden).To (vm => vm.HasStaredNavigation);
            set.Bind(vStart).For(v=>v.Hidden).To(vm=>vm.ExpectedTime).WithConversion("ExpectedTimeToBool");

			set.Bind (lbTravelDistance).To (vm => vm.TotalDistance).WithConversion ("Distance");
			set.Bind (lbExpectedTime).To (vm => vm.ExpectedTime).WithConversion ("ExpectedTime");

            set.Bind (btnBarOverviewResume).For (b => b.Title).To (vm => vm.OverviewResumeTitle);
            set.Bind (btnBarOverviewResume).To (vm => vm.ChangeNavigationModeCommand);
            set.Bind (btnBarStart).To (vm => vm.StartNavigateCommand);

            if (ViewModel.Status == ParkingStatus.Rented)
                set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.TotalParkingTime).WithConversion("ParkingTimer");
            else
            {
                set.Bind(NavigationItem).For(v => v.Title).To(vm => vm.Title);
            }

            #region Language Binding

            #endregion

			set.Apply ();

			tableRoutes.Source = source;
			tableRoutes.ReloadData ();

			collectionRoutes.Source = stepSource;
			collectionRoutes.Delegate = this;
			collectionRoutes.ReloadData ();

			btnInfo.Hidden = true;

			mapView.WeakDelegate = this;
			mapView.ShowsUserLocation = true;
			mapView.UserTrackingMode = MKUserTrackingMode.Follow;

			UIRotationGestureRecognizer rotateGesture = new UIRotationGestureRecognizer (HeadingChanged);
			rotateGesture.ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously;

			mapView.AddGestureRecognizer (rotateGesture);

            //20151013 (Duy-BSS) : Whenever put back the Tab, please uncomment this to get the exacted Map height
			//cstBottomSpace.Constant = UIScreen.MainScreen.Bounds.Height / 8f;
		}

		bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			return true;
		}

		nfloat currentAngle = 0f;
		nfloat lastAngle = 0f;
		void HeadingChanged (UIRotationGestureRecognizer gesture)
		{
//			Console.WriteLine ("Rotation changed " + gesture.Rotation);

			if (gesture.State == UIGestureRecognizerState.Began) {				
				currentAngle = lastAngle;
			}

//			if (gesture.State != UIGestureRecognizerState.Ended) {
//				lastAngle = currentAngle + gesture.Rotation;
//				lastAngle = (nfloat)((double)lastAngle % (2*Math.PI));
//			}

			if (gesture.State == UIGestureRecognizerState.Ended) {				
				lastAngle = currentAngle + gesture.Rotation;
				lastAngle = (nfloat)((double)lastAngle % (2 * Math.PI));
			}
			
			if (currentDirectionAnnotation != null) {
				var view = mapView.ViewForAnnotation (currentDirectionAnnotation);
				view.Transform = CGAffineTransform.MakeRotation ((nfloat)currentDirectionAnnotation.CurAngle + currentAngle + gesture.Rotation);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			//reset vRouteInfo size
			var size = new CGSize ();
			size.Width = UIScreen.MainScreen.Bounds.Width - 50f;
			size.Height = size.Width;
			vRouteInfo.Frame = new CGRect (new CGPoint (0, 0), size);

			stepCellSize.Width = UIScreen.MainScreen.Bounds.Width;
			stepCellSize.Height = 60f;
		}

		#region MKMapViewDelegate

		[Export ("mapView:rendererForOverlay:")]
		public virtual MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
		{
			if (overlay is MKPolyline) {
				Console.WriteLine ("get renderer for overlay");
				var renderer = new MKPolylineRenderer (overlay as MKPolyline);

				if (overlay.GetTitle () == "direction")
					renderer.StrokeColor = UIColor.Blue.ColorWithAlpha (0.1f);
				else
					renderer.StrokeColor = UIColor.Red.ColorWithAlpha (0.5f);
				renderer.LineWidth = 10;
				return renderer;
			}
			return null;
		}

		[Export ("mapView:viewForAnnotation:")]
		public MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			if (annotation is MKUserLocation) {
				return null;
			}

			if (annotation is StepAnnotation) {
				var annotationView = mapView.DequeueReusableAnnotation ("StepAnnotation") as MKPinAnnotationView;
				if (annotationView == null)
					annotationView = new MKPinAnnotationView (annotation, "StepAnnotation");
				annotationView.Annotation = annotation;
				annotationView.PinColor = MKPinAnnotationColor.Green;
				return annotationView;
			}

			if (annotation is DestinationAnnotation) {
				var annotationView = mapView.DequeueReusableAnnotation ("DesinationAnnotation") as MKPinAnnotationView;
				if (annotationView == null)
					annotationView = new MKPinAnnotationView (annotation, "DesinationAnnotation");
				annotationView.Annotation = annotation;
				annotationView.PinColor = MKPinAnnotationColor.Purple;
				return annotationView;
			}

			if (annotation is DirectionAnnotation) {
				DirectionAnnotation directionAnnotation = annotation as DirectionAnnotation;
				var annotationView = mapView.DequeueReusableAnnotation ("DirectionAnnotation") as MKAnnotationView;
				if (annotationView == null) {
					annotationView = new MKAnnotationView (annotation, "DirectionAnnotation");
					annotationView.Image = UIImage.FromFile ("small_blue_icon_navigate.png");
				}

				annotationView.Annotation = annotation;

				CGAffineTransform transform = CGAffineTransform.MakeRotation ((nfloat)directionAnnotation.CurAngle + lastAngle);
				annotationView.Transform = transform;

				return annotationView;
			}

			return null;
		}

    	[Export ("mapView:didUpdateUserLocation:")]
		public void DidUpdateUserLocation (MKMapView mapView, MKUserLocation userLocation)
		{
			if (userLocation != null) {
                Console.WriteLine("location: " + userLocation.Coordinate.Latitude + " " + userLocation.Coordinate.Longitude);

				if (!ViewModel.IsNavigating || !ViewModel.HasStaredNavigation)
					return;

				if (currentStepIndex < ViewModel.Routes.Count - 1) {
					RouteItem nextItem = ViewModel.Routes [currentStepIndex + 1];
					CLLocation nexLocation = new CLLocation (nextItem.Lat, nextItem.Long);

					double distanceToNext = userLocation.Location.DistanceFrom (nexLocation);

                    Console.WriteLine("distance to next steps " + distanceToNext);

					RouteItem currentItem = ViewModel.Routes [currentStepIndex];
					CLLocation currentLocation = new CLLocation (currentItem.Lat, currentItem.Long);
					double distanceToCurrent = userLocation.Location.DistanceFrom (currentLocation);

					double distanceCurrentToNext = nexLocation.DistanceFrom (currentLocation);

					if (distanceToNext < distanceCurrentToNext && distanceToCurrent > 5 && distanceToNext < 100) {
                        Console.WriteLine("move to next steps");
						collectionRoutes.SetContentOffset (new CGPoint (collectionRoutes.Frame.Width * (currentStepIndex + 1), 0), true);
						DrawStepForIndex (currentStepIndex + 1, false);
					} else if (currentStepIndex > 0) {				

						RouteItem prevItem = ViewModel.Routes [currentStepIndex - 1];
						CLLocation prevLocation = new CLLocation (prevItem.Lat, prevItem.Long);

						double distanceToPrev = userLocation.Location.DistanceFrom (prevLocation);
						double distancePrevToCurrent = prevLocation.DistanceFrom (currentLocation);

						if (distanceToPrev < 5 && distanceToCurrent > distancePrevToCurrent) {
							// user go back to prev step
							collectionRoutes.SetContentOffset (new CGPoint (collectionRoutes.Frame.Width * (currentStepIndex - 1), 0), true);
							DrawStepForIndex (currentStepIndex - 1, false);
						}
					}
				}
			}
		}

		#endregion

        /*private async void FindRouteAndDrawRoute (double sourceLat, double sourceLng, double destinationLat, double destinationLng, DirectionsMode directionsMode = DirectionsMode.Driving)
		{
            if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined ||  CLLocationManager.Status == CLAuthorizationStatus.Denied)
                return;

            //get current location
            Geolocator locator = new Geolocator(){ DesiredAccuracy = 100};
            var location = await locator.GetPositionAsync(50000);

            Console.WriteLine("Position Status: {0}", location.Timestamp);
            Console.WriteLine("Position Latitude: {0}", location.Latitude);
            Console.WriteLine("Position Longitude: {0}", location.Longitude);

            MKPlacemark source = new MKPlacemark (new CLLocationCoordinate2D (sourceLat, sourceLng), new NSDictionary ());
			MKMapItem sourceItem = new MKMapItem (source);

			desCoordinate = new CLLocationCoordinate2D (destinationLat, destinationLng);
			MKPlacemark destination = new MKPlacemark (new CLLocationCoordinate2D (destinationLat, destinationLng), new NSDictionary ());
			MKMapItem destinationItem = new MKMapItem (destination);

			MKDirectionsRequest request = new MKDirectionsRequest ();
			request.Source = sourceItem;
			request.Destination = destinationItem;
			request.TransportType = directionsMode == DirectionsMode.Driving ? MKDirectionsTransportType.Automobile : MKDirectionsTransportType.Walking;

			MKDirections direction = new MKDirections (request);

			direction.CalculateDirections (delegate(MKDirectionsResponse response, NSError error) {
				if (error == null) {
					//remove all routes that has been drawn on map
					if (mapView.Overlays != null && mapView.Overlays.Length != 0) {
						foreach (var overlay in mapView.Overlays) {
							mapView.RemoveOverlay (overlay);
						}
					}

					//check if have route
					if (response.Routes.Length == 0) {
						Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this.ViewModel, "Cannot find the route"));
					}

					//add new route
					foreach (var route in response.Routes) {
						MKPolyline polyline = route.Polyline;
						mapView.AddOverlay (polyline);

						ViewModel.TotalDistance = route.Distance;
						ViewModel.ExpectedTime = TimeSpan.FromSeconds(route.ExpectedTravelTime);

						foreach (var step in route.Steps) {
							ViewModel.Routes.Add (new FlexyPark.Core.Models.RouteItem () {
								Instruction = step.Instructions,
								Distance = step.Distance,
								Long = step.Polyline.Coordinate.Longitude,
								Lat = step.Polyline.Coordinate.Latitude
							});

							Console.WriteLine (step.Instructions);
							Console.WriteLine (step.Distance);
						}
						break;
					}

				} else {
					Console.WriteLine (error.LocalizedDescription);
				}
			});

			MKMapPoint userPoint = MKMapPoint.FromCoordinate (new CLLocationCoordinate2D (sourceLat, sourceLng));
			MKMapRect zoomRect = new MKMapRect (userPoint.X, userPoint.Y, 0.1, 0.1);

			MKMapPoint annotationPoint = MKMapPoint.FromCoordinate (new CLLocationCoordinate2D (destinationLat, destinationLng));
			MKMapRect pointRect = new MKMapRect (annotationPoint.X, annotationPoint.Y, 0.1, 0.1);

			zoomRect = MKMapRect.Union (zoomRect, pointRect);

            overviewRegion = MKCoordinateRegion.FromMapRect (zoomRect); 
            overviewRegion.Span.LatitudeDelta += 0.05;
            overviewRegion.Span.LongitudeDelta += 0.05;

			StepAnnotation annotationSoure = new StepAnnotation ();
			annotationSoure.SetCoordinate (new CLLocationCoordinate2D (sourceLat, sourceLng));
			mapView.AddAnnotation (annotationSoure);

			MKPointAnnotation annotationDest = new MKPointAnnotation ();
			annotationDest.SetCoordinate (new CLLocationCoordinate2D (destinationLat, destinationLng));
			mapView.AddAnnotation (annotationDest);

			destinationAnnotation = annotationDest;
	
            mapView.SetRegion (overviewRegion, true);

            isShowRoute = true;
		}*/

        private async void FindRouteAndDrawRoute (double destinationLat, double destinationLng, DirectionsMode directionsMode = DirectionsMode.Driving)
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined || CLLocationManager.Status == CLAuthorizationStatus.Denied)
                return;
            
            //get current location
            Geolocator locator = new Geolocator(){ DesiredAccuracy = 100};
            var location = await locator.GetPositionAsync(50000);

            Console.WriteLine("Position Status: {0}", location.Timestamp);
            Console.WriteLine("Position Latitude: {0}", location.Latitude);
            Console.WriteLine("Position Longitude: {0}", location.Longitude);

            var sourceLat = location.Latitude;
            var sourceLng = location.Longitude;

            MKPlacemark source = new MKPlacemark (new CLLocationCoordinate2D (sourceLat, sourceLng), new NSDictionary ());
            MKMapItem sourceItem = new MKMapItem (source);

            desCoordinate = new CLLocationCoordinate2D (destinationLat, destinationLng);
            MKPlacemark destination = new MKPlacemark (new CLLocationCoordinate2D (destinationLat, destinationLng), new NSDictionary ());
            MKMapItem destinationItem = new MKMapItem (destination);

            MKDirectionsRequest request = new MKDirectionsRequest ();
            request.Source = sourceItem;
            request.Destination = destinationItem;
            request.TransportType = directionsMode == DirectionsMode.Driving ? MKDirectionsTransportType.Automobile : MKDirectionsTransportType.Walking;

            MKDirections direction = new MKDirections (request);

            direction.CalculateDirections (delegate(MKDirectionsResponse response, NSError error) {
                if (error == null) {
                    //remove all routes that has been drawn on map
                    if (mapView.Overlays != null && mapView.Overlays.Length != 0) {
                        foreach (var overlay in mapView.Overlays) {
                            mapView.RemoveOverlay (overlay);
                        }
                    }

                    //check if have route
                    if (response.Routes.Length == 0) {
                        Mvx.Resolve<IMvxMessenger> ().Publish (new ToastMessage (this.ViewModel, "Cannot find the route"));
                    }

                    //add new route
                    foreach (var route in response.Routes) {
                        MKPolyline polyline = route.Polyline;
                        mapView.AddOverlay (polyline);

                        ViewModel.TotalDistance = route.Distance;
                        ViewModel.ExpectedTime = TimeSpan.FromSeconds(route.ExpectedTravelTime);

                        foreach (var step in route.Steps) {
                            ViewModel.Routes.Add (new FlexyPark.Core.Models.RouteItem () {
                                Instruction = step.Instructions,
                                Distance = step.Distance,
                                Long = step.Polyline.Coordinate.Longitude,
                                Lat = step.Polyline.Coordinate.Latitude
                            });

                            Console.WriteLine (step.Instructions);
                            Console.WriteLine (step.Distance);
                        }
                        break;
                    }

                } else {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this.ViewModel, error.LocalizedDescription));
                }
            });

            MKMapPoint userPoint = MKMapPoint.FromCoordinate (new CLLocationCoordinate2D (sourceLat, sourceLng));
            MKMapRect zoomRect = new MKMapRect (userPoint.X, userPoint.Y, 0.1, 0.1);

            MKMapPoint annotationPoint = MKMapPoint.FromCoordinate (new CLLocationCoordinate2D (destinationLat, destinationLng));
            MKMapRect pointRect = new MKMapRect (annotationPoint.X, annotationPoint.Y, 0.1, 0.1);

            zoomRect = MKMapRect.Union (zoomRect, pointRect);

            overviewRegion = MKCoordinateRegion.FromMapRect (zoomRect); 
            overviewRegion.Span.LatitudeDelta += 0.05;
            overviewRegion.Span.LongitudeDelta += 0.05;

            StepAnnotation annotationSoure = new StepAnnotation ();
            annotationSoure.SetCoordinate (new CLLocationCoordinate2D (sourceLat, sourceLng));
            mapView.AddAnnotation (annotationSoure);

            MKPointAnnotation annotationDest = new MKPointAnnotation ();
            annotationDest.SetCoordinate (new CLLocationCoordinate2D (destinationLat, destinationLng));
            mapView.AddAnnotation (annotationDest);

            destinationAnnotation = annotationDest;

            mapView.SetRegion (overviewRegion, true);

            isShowRoute = true;
        }

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public virtual void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
        {
            Console.WriteLine(status.ToString());
            if (status == CLAuthorizationStatus.NotDetermined)
            {
                if (locationManager != null)
                {
                    // Check for iOS 8. Without this guard the code will crash with "unknown selector" on iOS 7.
                    if (locationManager.RespondsToSelector(new ObjCRuntime.Selector("requestWhenInUseAuthorization")) ) 
                    {
                        locationManager.RequestWhenInUseAuthorization();
                    }
                    return;
                }
            }

            if(status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.Denied)
            {
                string title = (status == CLAuthorizationStatus.Denied) ? "Location services are off" : "Location Service is not enabled";
                string message = "To use Location Service you must turn on 'While using the app' in the Location Services Settings";

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "Cancel", "Settings");

                    alertView.Clicked += (object sender, UIButtonEventArgs e) =>
                        {
                            if (e.ButtonIndex == alertView.CancelButtonIndex) //cancel
                            {
                                //ViewModel.CloseViewModel();
                            }
                            else
                            {
                                //go to settings
                                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                            }
                        };

                    alertView.Dismissed += (sender, e) =>
                        {
                            alertView.Dispose();
                            alertView = null;
                        };

                    alertView.Show();
                }
                else
                {
                    // ios 7 only has two CLAuthorizationStatus : Denied and AuthorizedAlways
                    if (status == CLAuthorizationStatus.AuthorizedAlways)
                        return;

                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this.ViewModel, false));

                    UIAlertView alertView = new UIAlertView(title, message, null, "OK" );

                    alertView.Clicked += (sender, e) => 
                        {
                            //if (e.ButtonIndex == alertView.CancelButtonIndex) 
                                //ViewModel.CloseViewModel();
                        };

                    alertView.Dismissed += (sender, e) =>
                        {
                            alertView.Dispose();
                            alertView = null;
                        };

                    alertView.Show();
                }
            }
        }

		#region DateTime Picker

		WYPopoverController popoverPicker;
		UIViewController fakeVC;

		void InitPicker ()
		{
			if (fakeVC == null) {
				fakeVC = new UIViewController ();
				fakeVC.View = vRouteInfo;
			}
			if (popoverPicker == null) {
				popoverPicker = new WYPopoverController (fakeVC);
				popoverPicker.PopoverContentSize = vRouteInfo.Frame.Size;
				popoverPicker.Theme = WYPopoverTheme.ThemeForIOS6 ();
			}
		}

		#endregion

		#region Steps Collection

		CGSize stepCellSize = new CGSize ();

		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		public virtual CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return stepCellSize;
		}

		[Export ("scrollViewDidEndDecelerating:")]
		public virtual void DecelerationEnded (UIScrollView scrollView)
		{
			var pageWidth = scrollView.Frame.Size.Width;
			var page = (int)Math.Floor (scrollView.ContentOffset.X / scrollView.Frame.Width);
			Console.WriteLine (page);
			DrawStepForIndex (page);
		}

		#endregion

		#region DrawStep

		int currentStepIndex = -1;
		//		List<StepAnnotation> listStepAnnotation = new List<StepAnnotation> ();
		DirectionAnnotation currentDirectionAnnotation = null;
		CLLocationCoordinate2D desCoordinate = new CLLocationCoordinate2D ();

		void DrawStepForIndex (int index, bool needZoom = true)
		{
			if (currentDirectionAnnotation != null && index == currentStepIndex)
				return;

			currentStepIndex = index;

//			mapView.RemoveAnnotations (listStepAnnotation.ToArray ());
//			listStepAnnotation.Clear ();

            RouteItem item = ViewModel.Routes [index];
			CLLocationCoordinate2D coordinate = new CLLocationCoordinate2D (item.Lat, item.Long);

//			StepAnnotation annotation = new StepAnnotation ();
//
//			annotation.SetCoordinate (coordinate);
//			mapView.AddAnnotation (annotation);
//			listStepAnnotation.Add (annotation);

			if (needZoom) {
				MKCoordinateRegion region = MKCoordinateRegion.FromDistance (coordinate, 400, 400);
				mapView.SetRegion (region, true);
//				mapView.SetCenterCoordinate (coordinate, true);
			}

			if (currentDirectionAnnotation != null)
				mapView.RemoveAnnotation (currentDirectionAnnotation);
			currentDirectionAnnotation = null;

			DirectionAnnotation nextDirectionAnnotation = new DirectionAnnotation ();
			nextDirectionAnnotation.SetCoordinate (coordinate);	

			if (currentStepIndex < ViewModel.Routes.Count - 1) {				
				RouteItem nextItem = ViewModel.Routes [index + 1];

				CLLocationCoordinate2D nextCoordinate = new CLLocationCoordinate2D (nextItem.Lat, nextItem.Long);		

				nextDirectionAnnotation.CurAngle = CalcBearing (coordinate, nextCoordinate).ToRad ();

			} else {
				if (desCoordinate.IsValid ()) {
					nextDirectionAnnotation.SetCoordinate (desCoordinate);	

					nextDirectionAnnotation.CurAngle = CalcBearing (coordinate, desCoordinate).ToRad ();
				}
			}

			mapView.AddAnnotation (nextDirectionAnnotation);
			currentDirectionAnnotation = nextDirectionAnnotation;
		}

		private double DegreeToRadian (double degree)
		{
			return degree / 180.0f * Math.PI;
		}

		private double RadianToDegree (double radian)
		{
			return radian / Math.PI * 180.0f;
		}

		private double CalcBearing (CLLocationCoordinate2D start, CLLocationCoordinate2D end)
		{
			var deltaLon = end.Longitude - start.Longitude;
			var y = Math.Sin (deltaLon.ToRad ()) * Math.Cos (end.Latitude.ToRad ());
			var x = Math.Cos (start.Latitude.ToRad ()) * Math.Sin (end.Latitude.ToRad ()) - Math.Sin (start.Latitude.ToRad ()) * Math.Cos (end.Latitude.ToRad ()) * Math.Cos (deltaLon.ToRad ());

			var brng = Math.Atan2 (y, x).ToDeg ();
			return brng;
		}

		private CLLocationCoordinate2D FindCenterPoint (CLLocationCoordinate2D _lo1, CLLocationCoordinate2D _loc2)
		{
			CLLocationCoordinate2D center;

			double lon1 = _lo1.Longitude.ToRad ();
			double lon2 = _loc2.Longitude.ToRad ();

			double lat1 = _lo1.Latitude.ToRad ();
			double lat2 = _loc2.Latitude.ToRad ();

			double dLon = lon2 - lon1;

			double x = Math.Cos (lat2) * Math.Cos (dLon);
			double y = Math.Cos (lat2) * Math.Sin (dLon);

			double lat3 = Math.Atan2 (Math.Sin (lat1) + Math.Sin (lat2), Math.Sqrt ((Math.Cos (lat1) + x) * (Math.Cos (lat1) + x) + y * y));
			double lon3 = lon1 + Math.Atan2 (y, Math.Cos (lat1) + x);

			center.Latitude = lat3.ToDeg ();
			center.Longitude = lon3.ToDeg ();

			return center;
		}

		#endregion

		#region IParkingMapView implementation

		public void ShowRoutesPopup ()
		{
			InitPicker ();

			popoverPicker.PresentPopoverFromRect (btnInfo.Frame, View, WYPopoverArrowDirection.Up, true);
		}

		public void ChangeNavigationMode ()
		{
			if (!ViewModel.IsNavigating) {
				// overview
				if (currentDirectionAnnotation != null)
					mapView.RemoveAnnotation (currentDirectionAnnotation);
				currentDirectionAnnotation = null;
				mapView.SetRegion (overviewRegion, true);
			} else {
				//resume
				DrawStepForIndex(currentStepIndex, true);
			}
		}

		public void StartNavigation ()
		{
			DrawStepForIndex (0, true);
		}

		public void EndNavigation ()
		{
			mapView.RemoveOverlays (mapView.Overlays);
			mapView.RemoveAnnotations (mapView.Annotations);
			currentDirectionAnnotation = null;

			if (destinationAnnotation != null) {
				mapView.AddAnnotation (destinationAnnotation);
				destinationAnnotation = null;
			}

			mapView.SetRegion (overviewRegion, true);
		}

		public void ReCenter ()
		{
			
		}

        public void ChangeBarButton()
        {
            if (ViewModel.HasStaredNavigation)
                NavigationItem.RightBarButtonItem = btnBarOverviewResume;
            else
                NavigationItem.RightBarButtonItem = btnBarStart;
        }

		#endregion
	}

	public class DirectionAnnotation : MKAnnotation
	{
		CLLocationCoordinate2D coordinate;

		public double CurAngle{ get; set; }

		public DirectionAnnotation ()
		{
		}

		public override void SetCoordinate (CLLocationCoordinate2D value)
		{
			coordinate = value;
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
		}
	}

	public class StepAnnotation : MKAnnotation
	{
		CLLocationCoordinate2D coordinate;

		public StepAnnotation ()
		{
		}

		public override void SetCoordinate (CLLocationCoordinate2D value)
		{
			coordinate = value;
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
		}
	}

	public class DestinationAnnotation : MKAnnotation
	{
		CLLocationCoordinate2D coordinate;

		public DestinationAnnotation ()
		{
		}

		public override void SetCoordinate (CLLocationCoordinate2D value)
		{
			coordinate = value;
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
		}
	}
}

