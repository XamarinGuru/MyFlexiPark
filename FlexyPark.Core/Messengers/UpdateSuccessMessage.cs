using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core
{
    public class UpdateSuccessMessage :MvxMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlexyPark.Core.UpdateSuccessMessage"/> class.
        /// Using for update vehicles list after add / edit.
        /// </summary>
        /// <param name="sender">Sender.</param>
        public UpdateSuccessMessage(object sender)
            : base(sender)
        {
        }
    }
}

