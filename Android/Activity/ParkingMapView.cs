using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Resources.layout;
using FlexyPark.UI.Touch.Extensions;
using Flurl.Http;
using Newtonsoft.Json;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class ParkingMapView : BaseView, GoogleMap.IOnMyLocationChangeListener, IParkingMapView
    {
        #region UI Controls

        public MvxSubscriptionToken mMapKitMessage;
        private GoogleMap map;
        private ViewPager mViewPager;
        private LinearLayout llStart;

        ObservableCollection<LatLng> ListNextLocation = new ObservableCollection<LatLng>();

        #endregion

        #region Variables

        private bool IsDrawedStreet = false;
        public ObservableCollection<Step> ListSteps = new ObservableCollection<Step>();
        private Bitmap bmArrow;
        private BitmapDescriptor bmDescriptor;
        private Bitmap bmRotate;
        private MarkerOptions mkOptions;
        GroundOverlayOptions groundOverlayOptions;
        GroundOverlay myOverlay;
        private Matrix matrix;
        private Marker mMarker;
        private int CurrentInstruction = -1;
        private LatLng StartLocation;
        private LatLng toLatLng;
        private LatLng Destination = new LatLng(50.91039363, 4.34389114);
        private LatLng CurrentLocation;
        private bool IsZoommingCamera = false;
        private bool IsFirstInstruction = true;
        private bool IsAuto = false;
        private bool IsAutoChangeCamera = false;
        bool IsUserTouching = false;
        bool IsSetPagerSelected = false;
        private System.Timers.Timer mTimer;



        #endregion

        #region Constructors

        public new ParkingMapViewModel ViewModel
        {
            get { return base.ViewModel as ParkingMapViewModel; }
            set
            {
                base.ViewModel = value;
            }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParkingMapView);
            Init();
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (map == null)
            {
                var fragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
                map = fragment.Map;

            }

            if (map != null && IsDrawedStreet == false)
            {
                map.MyLocationEnabled = true;
                map.UiSettings.ZoomControlsEnabled = false;
                map.UiSettings.CompassEnabled = true;
                map.UiSettings.MapToolbarEnabled = false;
                map.SetOnMyLocationChangeListener(this);

                //toLatLng = new LatLng(50.673859, 4.615169);
                toLatLng = new LatLng(Destination.Latitude, Destination.Longitude
                    );

                if (CurrentLocation != null)
                {
                    DrawStreet(CurrentLocation, toLatLng);
                    MarkerOptions CurrentLocationMarkerOptions = new MarkerOptions();
                    CurrentLocationMarkerOptions.SetPosition(CurrentLocation);
                    map.AddMarker(CurrentLocationMarkerOptions);
                    if (IsDrawedStreet)
                    {
                        llStart.Visibility = ViewStates.Visible;
                    }

                }

                MarkerOptions toMarkerOptions = new MarkerOptions();
                toMarkerOptions.SetPosition(Destination);
                map.AddMarker(toMarkerOptions);
                map.CameraChange += (sender, args) =>
                {

                    if (!IsZoommingCamera && !IsAutoChangeCamera && CurrentLocation != null &&
                        TouchableWrapper.IsTouching)
                    {
                        ViewModel.IsShowReCenter = true;
                    }
                };
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopTimer();
        }

        #endregion

        #region Implements

        public void ShowRoutesPopup()
        {
        }

        // Change Mode Overview and Resume
        public async void ChangeNavigationMode()
        {
            if (ViewModel.IsNavigating)
            {
                // Resume
                if (CurrentLocation != null)
                {
                    ZoomCamera(CurrentLocation, null);
                }

                DrawArrow(ListSteps[mViewPager.CurrentItem].StartLocation, ListSteps[mViewPager.CurrentItem].EndLocation);
                // Delay for Animation, if not delay, animation not finish then animation of current location cut.

                ReCenter();
            }
            else
            {
                //Overview
                ViewModel.IsShowReCenter = false;
                if (StartLocation != null)
                {
                    ZoomCamera(StartLocation, Destination);
                }

                if (mMarker != null)
                {
                    mMarker.Remove();
                }
            }
        }

        public void StartNavigation()
        {

            if (CurrentLocation != null && ListSteps != null && ListSteps.Count != 0)
            {
                ZoomCamera(ListSteps[mViewPager.CurrentItem == null ? 0 : mViewPager.CurrentItem].StartLocation,
                    null);
                ViewModel.IsShowReCenter = false;

            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(ViewModel, "Warning", "Can not find routes!", "OK", null));
            }

        }

        public void EndNavigation()
        {
        }

        public async void ReCenter()
        {

            if (CurrentLocation != null && !IsZoommingCamera)
            {
                IsAutoChangeCamera = true;
                IsZoommingCamera = true;

                IsAuto = true;
                if (mViewPager != null && mViewPager.CurrentItem != CurrentInstruction)
                {
                    mViewPager.SetCurrentItem(CurrentInstruction, false);
                }
                map.AnimateCamera(CameraUpdateFactory.NewLatLng(CurrentLocation));
                await Task.Delay(2000);
                IsAutoChangeCamera = false;
                IsZoommingCamera = false;
                IsAuto = false;
                ViewModel.IsShowReCenter = false;
            }
        }

        public void ChangeBarButton()
        {
            if (ViewModel.HasStaredNavigation)
            {
                // NavigationItem.RightBarButtonItem = btnBarOverviewResume;
                FindViewById(Resource.Id.tvStart).Visibility = ViewStates.Gone;
                FindViewById(Resource.Id.tvOverview).Visibility = ViewStates.Visible;
            }
            else
            {
                // NavigationItem.RightBarButtonItem = btnBarStart;
                FindViewById(Resource.Id.tvOverview).Visibility = ViewStates.Gone;
                FindViewById(Resource.Id.tvStart).Visibility = ViewStates.Visible;
            }
        }

        public async void OnMyLocationChange(Location p0)
        {
            if (StartLocation == null)
            {
                StartLocation = new LatLng(p0.Latitude, p0.Longitude);
            }
            // Update current location.
            CurrentLocation = new LatLng(p0.Latitude, p0.Longitude);

            // Check if not draw street
            if (!IsDrawedStreet)
            {
                DrawStreet(CurrentLocation, Destination);
                MarkerOptions CurrentLocationMarkerOptions = new MarkerOptions();
                CurrentLocationMarkerOptions.SetPosition(CurrentLocation);
                map.AddMarker(CurrentLocationMarkerOptions);
                if (IsDrawedStreet)
                {
                    llStart.Visibility = ViewStates.Visible;
                }
            }

            if (IsFirstInstruction && ViewModel.HasStaredNavigation && ViewModel.IsNavigating)
            {
                // Distance betweeen First Location and Your Location more than 10 m -> Instruction change to next (Second Instruction)

                float[] results = new float[5];
                float[] results_StartAndFirst = new float[5];
                float[] results_CurLocationAndFirst = new float[5];

                Location.DistanceBetween(CurrentLocation.Latitude, CurrentLocation.Longitude, ListSteps[0].StartLocation.Latitude, ListSteps[0].StartLocation.Longitude, results);
                Location.DistanceBetween(ListSteps[0].StartLocation.Latitude, ListSteps[0].StartLocation.Longitude, StartLocation.Latitude, StartLocation.Longitude, results_StartAndFirst);
                Location.DistanceBetween(CurrentLocation.Latitude, CurrentLocation.Longitude, StartLocation.Latitude, StartLocation.Longitude, results_CurLocationAndFirst);
                if (results[0] > 10 && (results_CurLocationAndFirst[0] > results_StartAndFirst[0]))
                {
                    if (mViewPager != null)
                    {
                        try
                        {
                            IsAuto = true;
                            IsFirstInstruction = false;
                            mViewPager.SetCurrentItem(1, false);
                            CurrentInstruction = 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }


            }
            else
            {
                // Find Location nearest and change next instruction when Distance <5;
                if (ViewModel.IsNavigating && ListSteps != null && ListSteps.Count != 0)
                {

                    float[] results = new float[ListSteps.Count];
                    List<float> ListDistance = new List<float>();

                    for (int i = 0; i < ListSteps.Count; i++)
                    {
                        try
                        {
                            Location.DistanceBetween(p0.Latitude, p0.Longitude, ListSteps[i].StartLocation.Latitude,
                                ListSteps[i].StartLocation.Longitude, results);
                            ListDistance.Add(results[0]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                        }

                    }
                    if (ListDistance.Count != 0)
                    {
                        int index = 0;
                        float Min = ListDistance[0];
                        for (int i = 1; i < ListDistance.Count; i++)
                        {
                            if (Min > ListDistance[i])
                            {

                                Min = ListDistance[i];
                                index = i;
                            }
                        }
                        if (mViewPager != null && Min < 10 && CurrentInstruction != index + 1)
                        {
                            IsAuto = true;
                            mViewPager.SetCurrentItem(index + 1, true);
                            CurrentInstruction = index + 1;
                            IsAuto = false;
                        }
                    }
                }
            }

            // Move camera follow current location
            if (IsDrawedStreet && ViewModel.HasStaredNavigation && ViewModel.IsNavigating && !IsZoommingCamera && !ViewModel.IsShowReCenter)
            {
                IsAutoChangeCamera = true;
                IsZoommingCamera = true;
                map.AnimateCamera(CameraUpdateFactory.NewLatLng(CurrentLocation));
                await Task.Delay(1000);
                IsAutoChangeCamera = false;
                IsZoommingCamera = false;

            }

            //if (ListNextLocation.Count != 0 && ViewModel.IsNavigating && ViewModel.HasStaredNavigation)
            //{
            //    float[] results = new float[ListSteps.Count];
            //    List<float> ListDistance = new List<float>();

            //    for (int i = 0; i < ListNextLocation.Count; i++)
            //    {
            //        try
            //        {
            //            Location.DistanceBetween(CurrentLocation.Latitude, CurrentLocation.Longitude, ListNextLocation[i].Latitude,
            //               ListNextLocation[i].Longitude, results);
            //            ListDistance.Add(results[0]);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.ToString());

            //        }
            //    }

            //    if (ListDistance.Count != 0)
            //    {
            //        int index = 0;
            //        float Min = ListDistance[0];
            //        for (int i = 0; i < ListDistance.Count; i++)
            //        {
            //            if (Min > ListDistance[i])
            //            {

            //                Min = ListDistance[i];
            //                index = i;
            //            }

            //        }
            //         if (Min < 10 && CurrentInstruction != (index +1))
            //         {
            //             ListNextLocation.RemoveAt(index);
            //             if (mViewPager != null)
            //             {
            //                 mViewPager.SetCurrentItem(index+1, true);
            //                 CurrentInstruction = index+1;
            //                 DrawArrow(ListNextLocation[index + 1], ListNextLocation[index + 2]);
            //             }


            //         }
            //    }
            //}
        }

        #endregion

        #region Methods

        #region Init

        private void Init()
        {
            ViewModel.View = this;
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            llStart = FindViewById<LinearLayout>(Resource.Id.llStart);

            (FindViewById<TextView>(Resource.Id.tvRecenter)).Text =
                ViewModel.SharedTextSource.GetText("RecenterText");

            mViewPager.Visibility = ViewStates.Gone;

            // Check if Location Service on -> Start Visible
            if (CurrentLocation != null)
            {
                llStart.Visibility = ViewStates.Visible;
            }
            else
            {
                llStart.Visibility = ViewStates.Gone;
            }

            #region Google Maps

            bmArrow = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.blue_arrow);
            GoogleMapOptions mapOptions = new GoogleMapOptions()
                   .InvokeMapType(GoogleMap.MapTypeNormal)
                   .InvokeZoomControlsEnabled(true)
                   .InvokeCompassEnabled(true);

            var fragTx = SupportFragmentManager.BeginTransaction();
            var _mapFragment = SupportMapFragment.NewInstance(mapOptions);
            fragTx.Add(Resource.Id.map, _mapFragment, "map");
            fragTx.Commit();

            #endregion

            #region Subscrible Navigation

            mMapKitMessage = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<NavigateMapMessage>(
            message =>
            {

                if (message.Sender.GetType() != typeof(ParkingSummaryViewModel))
                    return;

                Destination = new LatLng(message.DestinationLat, message.DestinationLng);

                if (map != null && !IsDrawedStreet)
                {

                    // Set map move follow current location
                    ViewModel.IsShowReCenter = false;
                    // First instruction - Flag to estimate distance between current location and start location to change to next instruction.
                    IsFirstInstruction = true;

                    // Clear marker, line to draw new.
                    map.Clear();


                    //LatLng Destination = new LatLng(message.DestinationLat, message.DestinationLng);
                    // LatLng Destination = new LatLng(10.757915, 106.654325);

                    if (CurrentLocation != null)
                    {
                        DrawStreet(CurrentLocation, Destination);

                        if (ListSteps.Count != 0)
                        {
                            // If draw street success then show button Start.
                            llStart.Visibility = ViewStates.Visible;
                        }

                        // Set new Navigating if user re-draw street
                        ViewModel.IsNavigating = false;




                        // Marker Current Location
                        MarkerOptions CurrentLocationMarkerOptions = new MarkerOptions();
                        CurrentLocationMarkerOptions.SetPosition(CurrentLocation);
                        map.AddMarker(CurrentLocationMarkerOptions);

                        MarkerOptions toMarkerOptions = new MarkerOptions();
                        toMarkerOptions.SetPosition(toLatLng);
                        map.AddMarker(toMarkerOptions);
                    }

                }

            });

            #endregion

            #region Setup timer

            if (ViewModel.Status == ParkingStatus.Rented)
            {
                DecreaseTime();
            }
            else
            {
                FindViewById<TextViewWithFont>(Resource.Id.tvfTitle).Text = ViewModel.Title;
            }

            #endregion
        }

        #endregion

        #region Timer

        private void StopTimer()
        {
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Elapsed -= OnTimedEvent;
                mTimer.Dispose();
                mTimer = null;
            }
        }

        private void DecreaseTime()
        {
            if (mTimer == null)
            {
                mTimer = new System.Timers.Timer(1000);
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
            }
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            ViewModel.TotalParkingTime--;

            //Update visual representation here
            //Remember to do it on UI thread

            if (ViewModel.TotalParkingTime == 0)
            {
                mTimer.Stop();
            }
        }

        #endregion

        #region ZoomCamera
        public async void ZoomCamera(LatLng LatMyLocation, LatLng LatConsumer)
        {
            IsZoommingCamera = true;

            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            try
            {
                builder.Include(LatMyLocation);
            }
            catch (Exception e)
            {
            }
            if (LatConsumer != null)
            {
                try
                {
                    builder.Include(LatConsumer);
                }
                catch (Exception e)
                {
                }
            }
            LatLngBounds bounds = builder.Build();

            // 2 Postion
            if (LatMyLocation != null & LatConsumer != null)
            {
                CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, 100);
                map.MoveCamera(cu);
                map.AnimateCamera(cu);
            }
            else
            {
                map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(LatMyLocation, 19.0f));
            }

            await Task.Delay(2000);
            //await Task.Delay(2000);

            IsZoommingCamera = false;

        }

        #endregion

        #region DrawStreet
        public async Task DrawStreet(LatLng start, LatLng end)
        {

            var rectLine = new PolylineOptions();
            rectLine.InvokeWidth(10);
            rectLine.InvokeColor(Color.Rgb(116, 117, 225));
            string url = "http://maps.googleapis.com/maps/api/directions/json?"
                         + "origin=" + start.Latitude.ParseToCultureInfo(new CultureInfo("en-US")) + "," + start.Longitude.ParseToCultureInfo(new CultureInfo("en-US"))
                         + "&destination=" + end.Latitude.ParseToCultureInfo(new CultureInfo("en-US")) + "," + end.Longitude.ParseToCultureInfo(new CultureInfo("en-US"))
                         + "&sensor=false&units=metric&mode=driving";
            try
            {
                try
                {
                    List<List<LatLng>> routes = new List<List<LatLng>>();
                    var json = await url.GetJsonAsync<ResponseMapData>();
                    string polyline = json.Routes[0].OverviewPolyline.Points;

                    ViewModel.TotalDistance = double.Parse(json.Routes[0].Legs[0].TotalDistance.value, CultureInfo.InvariantCulture);
                    ViewModel.ExpectedTime = TimeSpan.FromSeconds(double.Parse(json.Routes[0].Legs[0].TotalDuration.value, CultureInfo.InvariantCulture));

                    List<LatLng> path = new List<LatLng>();
                    List<LatLng> list = decodePoly(polyline);
                    for (int l = 0; l < list.Count; l++)
                    {
                        LatLng tmpLat = new LatLng(list[l].Latitude, list[l].Longitude);
                        path.Add(tmpLat);
                    }
                    routes.Add(path);
                    for (int i = 0; i < routes.Count; i++)
                    {
                        for (int j = 0; j < routes[i].Count; j++)
                        {
                            rectLine.Add(routes[i][j]);
                        }
                    }
                    ListSteps = GetListStep(json);

                    SetAdapterViewPager();

                    StartLocation = CurrentLocation;
                    IsDrawedStreet = true;
                    ViewModel.IsDrawedStreet = true;

                    llStart.Visibility = ViewStates.Visible;


                }
                catch (FlurlHttpException exception)
                {
                    llStart.Visibility = ViewStates.Gone;
                    Console.WriteLine(exception.Message);
                }
                catch (Exception e)
                {
                    llStart.Visibility = ViewStates.Gone;
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                llStart.Visibility = ViewStates.Gone;
            }

            map.AddPolyline(rectLine);
            ZoomCamera(start, end);
        }

        #endregion

        #region ResponseMapData
        public class ResponseMapData
        {
            [JsonProperty("routes")]
            public List<Route> Routes { get; set; }
            public class Route
            {
                [JsonProperty("legs")]
                public List<Legs> Legs { get; set; }

                [JsonProperty("overview_polyline")]
                public OverviewPolyline OverviewPolyline { get; set; }
            }
            public class OverviewPolyline
            {
                [JsonProperty("points")]
                public string Points { get; set; }
            }


            public class Legs
            {
                [JsonProperty("steps")]
                public List<Steps> Steps { get; set; }

                [JsonProperty("distance")]
                public Distance TotalDistance { get; set; }

                [JsonProperty("duration")]
                public Duration TotalDuration { get; set; }
            }
            public class Steps
            {
                [JsonProperty("distance")]
                public Distance Distance { get; set; }

                [JsonProperty("duration")]
                public Duration Duration { get; set; }

                [JsonProperty("html_instructions")]
                public string html_instructions { get; set; }

                [JsonProperty("polyline")]
                public Polyline Polyline { get; set; }

                [JsonProperty("start_location")]
                public Location StartLocation { get; set; }

                [JsonProperty("end_location")]
                public Location EndLocation { get; set; }
            }
            public class Distance
            {
                [JsonProperty("text")]
                public string text { get; set; }

                [JsonProperty("value")]
                public string value { get; set; }
            }
            public class Duration
            {
                [JsonProperty("text")]
                public string text { get; set; }
                [JsonProperty("value")]
                public string value { get; set; }
            }

            public class Polyline
            {
                [JsonProperty("points")]
                public string Points { get; set; }
            }

            public class Location
            {
                [JsonProperty("lat")]
                public double Lat { get; set; }
                [JsonProperty("lng")]
                public double Lng { get; set; }
            }
        }
        #endregion

        #region GetListStep

        public ObservableCollection<Step> GetListStep(ResponseMapData json)
        {
            ObservableCollection<Step> steps = new ObservableCollection<Step>();
            for (int i = 0; i < json.Routes[0].Legs[0].Steps.Count; i++)
            {
                Step step = new Step();
                step.Distance = json.Routes[0].Legs[0].Steps[i].Distance.text;
                step.Duration = json.Routes[0].Legs[0].Steps[i].Duration.text;
                string tmpInstruction = (Html.FromHtml(json.Routes[0].Legs[0].Steps[i].html_instructions)).ToString();
                step.Instructions = tmpInstruction.Replace("\n", " ");
                LatLng tmpStartLocation = new LatLng(json.Routes[0].Legs[0].Steps[i].StartLocation.Lat, json.Routes[0].Legs[0].Steps[i].StartLocation.Lng);
                step.StartLocation = tmpStartLocation;
                LatLng tmpEndLocation = new LatLng(json.Routes[0].Legs[0].Steps[i].EndLocation.Lat, json.Routes[0].Legs[0].Steps[i].EndLocation.Lng);
                step.EndLocation = tmpEndLocation;
                step.Polyline = json.Routes[0].Legs[0].Steps[i].Polyline.Points;
                steps.Add(step);

            }

            // tmp Destination
            // LatLng mDestination = new LatLng(Destination.Latitude, Destination.Longitude);
            Step stepFinish = new Step();
            stepFinish.Distance = "";
            stepFinish.Duration = "";
            stepFinish.Instructions = "Arrive at the destination";
            stepFinish.StartLocation = Destination;
            stepFinish.EndLocation = null;
            stepFinish.Polyline = null;
            steps.Add(stepFinish);

            return steps;
        }

        #endregion

        #region DeCodePoly
        private List<LatLng> decodePoly(String encoded)
        {
            List<LatLng> poly = new List<LatLng>();
            int index = 0, len = encoded.Count();
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                LatLng p = new LatLng((((double)lat / 1E5)),
                        (((double)lng / 1E5)));
                poly.Add(p);
            }

            return poly;
        }

        #endregion

        #region Adapter


        private class ViewPagerAdapter : FragmentPagerAdapter
        {
            private int MaxCount = 200;
            ObservableCollection<Step> ListSteps = new ObservableCollection<Step>();

            public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, ObservableCollection<Step> mListSteps)
                : base(fm)
            {
                this.ListSteps = mListSteps;
                MaxCount = mListSteps.Count;
            }

            public override int Count
            {
                get
                {
                    return MaxCount;
                }
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                MvxFragment fragment;

                fragment = new ItemViewPagerFragment(ListSteps[position].Distance, ListSteps[position].Instructions);
                //: TODO: ZOOM CAMERA;
                //ZoomCamera(ListSteps[position].StartLocation, ListSteps[position].EndLocation);

                return fragment;
            }


        }

        #endregion

        #region DrawArrow

        public void DrawArrow(LatLng StartLocation, LatLng EndLocation)
        {
            if (mMarker != null)
            {
                mMarker.Remove();
            }

            matrix = new Matrix();

            if (EndLocation != null && StartLocation != null)
            {
                var Deg = CalcBearing(StartLocation, EndLocation);
                matrix.PostRotate(float.Parse(Deg.ToString()));
                bmRotate = Bitmap.CreateBitmap(bmArrow, 0, 0, bmArrow.Width, bmArrow.Height, matrix, true);

                bmDescriptor = BitmapDescriptorFactory.FromBitmap(bmRotate);

                mkOptions = new MarkerOptions();
                mkOptions.SetIcon(bmDescriptor);
                mkOptions.SetPosition(StartLocation);
                mkOptions.Anchor(0.5f, 0.5f);
                mMarker = map.AddMarker(mkOptions);

                bmRotate.Recycle();
                bmRotate = null;

                ////groundOverlayOptions = new GroundOverlayOptions()
                //groundOverlayOptions = new GroundOverlayOptions()
                //    .Position(StartLocation, bmArrow.Width, bmArrow.Height)
                //    .InvokeImage(bmDescriptor)
                //    .InvokeZIndex(5);
                //myOverlay = map.AddGroundOverlay(groundOverlayOptions);
                //ListArrow.Add(myOverlay);
            }



        }
        #endregion

        #region CalcBearing

        private double CalcBearing(LatLng start, LatLng end)
        {
            var deltaLon = end.Longitude - start.Longitude;
            var y = Math.Sin(deltaLon.ToRad()) * Math.Cos(end.Latitude.ToRad());
            var x = Math.Cos(start.Latitude.ToRad()) * Math.Sin(end.Latitude.ToRad()) - Math.Sin(start.Latitude.ToRad()) * Math.Cos(end.Latitude.ToRad()) * Math.Cos(deltaLon.ToRad());
            var brng = Math.Atan2(y, x).ToDeg();
            return brng;
        }

        #endregion

        #region SetAdapterViewPager

        public void SetAdapterViewPager()
        {
            if (ListSteps.Count != 0)
            {
                mViewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager, ListSteps);
                mViewPager.Visibility = ViewStates.Visible;
                mViewPager.SetCurrentItem(0, true);

                if (!IsSetPagerSelected)
                {
                    mViewPager.PageSelected += (sender, args) =>
                    {
                        if (IsAuto)
                        {
                            ViewModel.IsShowReCenter = false;
                            IsAuto = false;

                        }
                        else
                        {
                            ViewModel.IsShowReCenter = true;
                            map.AnimateCamera(CameraUpdateFactory.NewLatLng(ListSteps[args.Position].StartLocation));
                        }

                        {

                            DrawArrow(ListSteps[args.Position].StartLocation, ListSteps[args.Position].EndLocation);
                        }

                    };

                    IsSetPagerSelected = true;
                }
                for (int i = 1; i < ListSteps.Count; i++)
                {
                    ListNextLocation.Add(ListSteps[i].StartLocation);
                }
            }
        }

        #endregion

        #region TouchableWrapper

        public class TouchableWrapper : FrameLayout
        {
            public static bool IsTouching = false;

            #region Constructors

            protected TouchableWrapper(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public TouchableWrapper(Context context)
                : base(context)
            {
            }

            public TouchableWrapper(Context context, IAttributeSet attrs)
                : base(context, attrs)
            {
            }

            public TouchableWrapper(Context context, IAttributeSet attrs, int defStyleAttr)
                : base(context, attrs, defStyleAttr)
            {
            }

            public TouchableWrapper(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
                : base(context, attrs, defStyleAttr, defStyleRes)
            {
            }

            #endregion

            public override bool OnInterceptTouchEvent(MotionEvent ev)
            {

                switch (ev.Action)
                {
                    case MotionEventActions.Down:
                        IsTouching = true;
                        break;
                    case MotionEventActions.Up:
                        IsTouching = false;
                        break;
                }
                return base.OnInterceptTouchEvent(ev);
            }
        }

        #endregion

        #endregion
    }
}