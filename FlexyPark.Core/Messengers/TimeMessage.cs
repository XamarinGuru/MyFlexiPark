using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core.Messengers
{
    public class TimeMessage : MvxMessage
    {
        public int TimeLeft { get; private set;}

        public TimeMessage(object sender, int timeLeft) : base(sender)
        {
            this.TimeLeft = timeLeft;
        }
    }
}

