using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpBasicClientTest
{
    class Program
    {
        const int PORT_NO = 8000;
        const string SERVER_IP = "127.0.0.1";

        static void Main(string[] args)
        {

           

            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
            NetworkStream nwStream = client.GetStream();
            BinaryReader reader = new BinaryReader(nwStream);
            BinaryWriter writer = new BinaryWriter(nwStream);

            CommandRecievedEventArgs message = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, null, null);

            writer.Write(message.ToJSON());

            while (true)
            {
                string answer = reader.ReadString();
                Console.WriteLine(answer);
            }
        }

        public static string LogToJSON(List<MessageRecievedEventArgs> logHistory)
        {
            JObject jLog = new JObject();
            JArray logArr = new JArray();
            foreach (MessageRecievedEventArgs entry in logHistory)
            {
                JObject message = new JObject();
                message = JObject.Parse(entry.ToJSON());
                logArr.Add(message);
            }
            jLog["Log"] = logArr;
            return jLog.ToString();
        }

        public static string ConfigToJSON()
        {
            JObject JsonConfig = new JObject();
            JArray pathJSON = new JArray();
            List<string> hans = new List<string> { "1", "shfih", "fuck" };
            foreach (string path in hans)
            {
                pathJSON.Add(path);
            }
            JsonConfig["Handler"] = pathJSON;
            JsonConfig["OutputDir"] = "yes";
            JsonConfig["SourceName"] = "no";
            JsonConfig["LogName"] = "maybe";
            JsonConfig["ThumbnailSize"] = "125";

            return JsonConfig.ToString();
        }
    }

    public class CommandRecievedEventArgs : EventArgs
    {
        public int CommandID { get; set; }      // The Command ID
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; }  // The Request Directory

        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }

        public string ToJSON()
        {
            JObject eventobj = new JObject();
            eventobj["CommandID"] = CommandID;

            JArray args = new JArray(Args);
            eventobj["Args"] = args;

            eventobj["RequestDirPath"] = RequestDirPath;

            return eventobj.ToString();
        }

        public static CommandRecievedEventArgs FromJSON(string str)
        {
            JObject cmJson = JObject.Parse(str);
            // parse id
            int id = (int)cmJson["CommandID"];

            // parse args
            JArray JArgs = (JArray)cmJson["Args"];
            string[] args = new string[JArgs.Count];
            int i = 0;
            foreach (JToken tok in JArgs.Children())
            {
                args[i] = (string)tok;
                i++;
            }

            // parse path
            string path = (string)cmJson["RequestDirPath"];

            CommandRecievedEventArgs cmArgs = new CommandRecievedEventArgs(id, args, path);
            return cmArgs;
        }
    }

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

    public enum MessageTypeEnum : int
    {
        INFO,
        WARNING,
        FAIL
    }

    public enum CommandEnum : int
    {
        NewFileCommand,
        CloseCommand,
        HandlerRemoveCommand,
        ConfigCommand,
        LogCommand,
        CloseClientCommand
    }

}




