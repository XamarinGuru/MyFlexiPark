using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace FlexyPark.Core.Messengers
{
    public class AlertMessage : MvxMessage
    {
        public string Title { get; private set;}
        public string Message { get; private set;}
        public string CancelTitle { get; private set;}
        public Action CancelAction { get; private set;}
        public string[] OtherTitles { get; private set;}
        public Action[] OtherActions { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexyPark.Core.Messengers.AlertMessage"/> class.
        /// This constructor generates a alert with a Cancel button.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancelTitle">Cancel title.</param>
        /// <param name="cancelAction">Cancel action.</param>
        public AlertMessage(object sender, string title, string message, string cancelTitle, Action cancelAction) : base (sender)
        {
            this.Title = title;
            this.Message = message;
            this.CancelTitle = cancelTitle;
            this.CancelAction = cancelAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexyPark.Core.Messengers.AlertMessage"/> class.
        /// This constructor generates a alert with two buttons or more.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancelTitle">Cancel title.</param>
        /// <param name="cancelAction">Cancel Action.</param>
        /// <param name="otherTitles">Other titles.</param>
        /// <param name="otherActions">Other actions.</param>
        public AlertMessage (object sender, string title, string message, string cancelTitle, Action cancelAction, string[] otherTitles, params Action[] otherActions) : base (sender)
        {
            this.Title = title;
            this.Message = message;
            this.CancelTitle = cancelTitle;
            this.CancelAction = cancelAction;
            this.OtherTitles = otherTitles;
            this.OtherActions = otherActions;
        }

    }

}

