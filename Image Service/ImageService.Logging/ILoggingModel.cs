using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Model
{
    public delegate void NotifyClients(CommandRecievedEventArgs cmndRecieved);

    public interface ILoggingModel
    {
        event NotifyClients NotifyLogChanged;
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
       
        void Log(string message, MessageTypeEnum type);
        string LogToJSON();
    }
}