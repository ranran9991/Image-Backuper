using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            TcpListener listener = new TcpListener(ep);
            listener.Start();
            Console.WriteLine("Waiting for client connections...");
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Console.WriteLine("Waiting for a command");
                string command = reader.ReadString();
                Console.WriteLine("Command accepted");

                string c = ConfigToJSON();
                writer.Write(c);
                command = reader.ReadString();
                MessageRecievedEventArgs log = new MessageRecievedEventArgs();
                log.Message = "Test message 1";
                log.Status = MessageTypeEnum.INFO;
                MessageRecievedEventArgs log2 = new MessageRecievedEventArgs();
                log2.Message = "Test message 2";
                log2.Status = MessageTypeEnum.WARNING;
                MessageRecievedEventArgs log3 = new MessageRecievedEventArgs();
                log3.Message = "Test message 3";
                log3.Status = MessageTypeEnum.FAIL;
                List<MessageRecievedEventArgs> logs = new List<MessageRecievedEventArgs> { log, log2, log3 };
                string a = LogToJSON(logs);
                writer.Write(a);
                Console.WriteLine("Sent logs and config");
                Console.ReadKey();
                MessageRecievedEventArgs log4 = new MessageRecievedEventArgs();
                log4.Message = "Test message 4";
                log4.Status = MessageTypeEnum.WARNING;
                string[] comArgs = new string[] { log4.ToJSON() };
                CommandRecievedEventArgs com = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, comArgs, "");
                writer.Write(com.ToJSON());
                Console.WriteLine("Sent log");
                Console.ReadKey();
                com = new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, null, "shfih");
                writer.Write(com.ToJSON());
                Console.WriteLine("Sent handler removed");
                Console.ReadKey();
                while (true)
                {
                    writer.Write(reader.ReadString());
                    Console.WriteLine("Command received");
                }
                com.CommandID = (int)CommandEnum.CloseCommand;
                com.Args = null;
                writer.Write(com.ToJSON());
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
