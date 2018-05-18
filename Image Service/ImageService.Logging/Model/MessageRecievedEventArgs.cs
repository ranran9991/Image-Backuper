using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Model
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        public string ToJSON()
        {
            JObject Jobj = new JObject();
            Jobj["Status"] = (int)Status;
            Jobj["Message"] = Message;

            return Jobj.ToString();
        }
    }

    
}