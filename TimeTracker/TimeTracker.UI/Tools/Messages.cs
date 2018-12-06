using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.UI.Tools
{
    public class Messages
    {
        private static Models.UserMessage _message = new Models.UserMessage();

        public static Models.UserMessage CreateMessage(string headerText, string messageText, string icon, int hideAfter = 3000)
        {
            _message.Heading = headerText;
            _message.Message = messageText;
            _message.Icon = icon;
            _message.HideAfter = hideAfter;

            return _message;
        }

    }
}