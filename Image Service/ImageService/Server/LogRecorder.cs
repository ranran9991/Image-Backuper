using ImageService.Logging.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Image_Service.ImageService.Server
{
    public class LogRecorder
    {
        public List<MessageRecievedEventArgs> Log;
        public List<TcpClient> clientList;
        public LogRecorder()
        {
            Log = new List<MessageRecievedEventArgs>();
            clientList = new List<TcpClient>();
        }
        // Notifies all clients of new entries in log
        public void notify(MessageRecievedEventArgs e)
        {
            JObject message = new JObject();
            message["Message"] = e.Message;
            message["Type"] = (int)e.Status;
            foreach (TcpClient c in clientList)
            {
                NetworkStream stream = c.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                
                writer.Write(message.ToString());

                reader.Close();
                writer.Close();

            }
        }
        /// <summary>
        /// Parses the list of log entries to JSON for easy sending through socket
        /// </summary>
        /// <returns>JSON representation of Log</returns>
        public string ToJSON()
        {
            JArray entries = new JArray();
            foreach (var entry in Log)
            {
                // Create JObject for each entry
                JObject jentry = new JObject
                {
                    ["Message"] = entry.Message,
                    ["Status"] = (int)entry.Status
                };
                // Add to Array
                entries.Add(jentry);
            }
            return entries.ToString();
        }
    }
}
