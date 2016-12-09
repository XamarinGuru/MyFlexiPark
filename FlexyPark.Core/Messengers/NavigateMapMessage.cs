using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core.Messengers
{
    public class NavigateMapMessage : MvxMessage
    {
        public double SourceLat { get; private set;}
        public double SourceLng { get; private set;}
        public double DestinationLat { get; private set;}
        public double DestinationLng { get; private set;}
        public DirectionsMode DirectionsMode { get; private set;}

        public NavigateMapMessage(object sender, double sourceLat, double sourceLng, double destinationLat, double destinationLng, DirectionsMode directionsMode = DirectionsMode.Driving ) : base(sender)
        {
            this.SourceLat = sourceLat;
            this.SourceLng = sourceLng;
            this.DestinationLat = destinationLat;
            this.DestinationLng = destinationLng;
            this.DirectionsMode = directionsMode;
        }

        public NavigateMapMessage(object sender, double destinationLat, double destinationLng, DirectionsMode directionsMode = DirectionsMode.Driving ) : base(sender)
        {
            this.DestinationLat = destinationLat;
            this.DestinationLng = destinationLng;
            this.DirectionsMode = directionsMode;
        }
    }

    public class TextSourceMessage : MvxMessage
    {
        public TextSourceMessage(object sender) : base(sender)
        {
        }
    }
}

