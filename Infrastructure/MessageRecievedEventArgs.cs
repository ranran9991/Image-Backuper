using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }
        public MessageRecievedEventArgs(string message, MessageTypeEnum status)
        {
            Message = message;
            Status = status;
        }
        public MessageRecievedEventArgs() { }
        public static MessageRecievedEventArgs FromJSON(string str)
        {
            JObject msJson = JObject.Parse(str);
            int Status = (int)msJson["Status"];
            string Message = (string)msJson["Message"];
            MessageRecievedEventArgs message = new MessageRecievedEventArgs();
            message.Message = Message;
            message.Status = (MessageTypeEnum)Status;
            return message;
        }
        public string ToJSON()
        {
            JObject msgJSON = new JObject();
            msgJSON["Status"] = (int)Status;
            msgJSON["Message"] = Message;

            return msgJSON.ToString();
        }
        public static List<MessageRecievedEventArgs> LogFromJSON(string str)
        {
            JObject lgJson = JObject.Parse(str);
            JArray JLogs = (JArray)lgJson["Log"];
            List<MessageRecievedEventArgs> logs = new List<MessageRecievedEventArgs>();
            foreach (JObject obj in JLogs.Children())
            {
                logs.Add(FromJSON(obj.ToString()));
            }
            return logs;
        }
    }
}
