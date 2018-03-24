using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    public class LoggingModal : ILoggingModal
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        void ILoggingModal.Log(string message, MessageTypeEnum type)
        {
            //initializing a MessageRecievedEvent
            MessageRecievedEventArgs e = new MessageRecievedEventArgs
            {
                Status = type,
                Message = message
            };
            // calling handler on Event if its not null
            MessageRecieved?.Invoke(this, e);
        } 
    }
}
