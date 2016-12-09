using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core.Messengers
{
    public class ToastMessage : MvxMessage
    {
        public string Message { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexyPark.Core.Messengers.ToastMessage"/> class.
        /// This constructor generates a toast with message.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="message">Message.</param>
        public ToastMessage(object sender, string message) : base(sender)
        {
            this.Message = message;
        }
    }
}

