using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core.Messengers
{
    public class ProgressMessage : MvxMessage
    {
        public bool IsShow{ get; private set;}
		public bool IsAllowInteraction { get; private set; }
        public string Message { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexyPark.Core.Messengers.ProgressMessage"/> class.
        /// This constructor generates a progress dialog with message.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="isShow">If set to <c>true</c> is to show, <c>false</c> is to dismiss.</param>
        /// <param name="message">Message.</param>
        /// Message to be shown. Default is empty.
		public ProgressMessage(object sender, bool isShow, string message = "", bool isAllowInteraction = false) : base(sender)
        {
            this.IsShow = isShow;
            this.Message = message;
			this.IsAllowInteraction = isAllowInteraction;
        }
    }
}

