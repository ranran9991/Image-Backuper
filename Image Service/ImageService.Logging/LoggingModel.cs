using ImageService.Infrastructure.Enums;
using ImageService.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Model
{
    public class LoggingModel : ILoggingModel
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        // instance of notifiyng event
        public event NotifyClients NotifyLogChanged;
        public List<MessageRecievedEventArgs> logHistory;

        public LoggingModel()
        {
            logHistory = new List<MessageRecievedEventArgs>();
        }
        
        public void Log(string message, MessageTypeEnum type)
        {
            //initializing a MessageRecievedEvent
            MessageRecievedEventArgs e = new MessageRecievedEventArgs
            {
                Status = type,
                Message = message
            };
            this.logHistory.Add(e);

            string jsonMessage = e.ToJSON();
            string[] args = new string[] { jsonMessage };
            
            CommandRecievedEventArgs cmndRecieved = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, args, null);
            // calling handler on Event if its not null
            MessageRecieved?.Invoke(this, e);
            // tell clients that the log has changed
            NotifyLogChanged?.Invoke(cmndRecieved);
        }
        public string LogToJSON()
        {
            JObject jLog = new JObject();
            JArray logArr = new JArray();
            foreach(MessageRecievedEventArgs entry in logHistory)
            {
                JObject message = new JObject();
                message = JObject.Parse(entry.ToJSON());
                logArr.Add(message);
            }
            jLog["Log"] = logArr;
            return jLog.ToString();
        }
    }
}
