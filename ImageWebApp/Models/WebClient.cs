using ImageWebApp.Models;
using Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ImageWebApp.Client
{
    public class WebClient
    {
        private static WebClient instance;

        private WebClient()
        {
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            int port = 8000;
            IPEndPoint ep = new IPEndPoint(IP, port);
            client = new TcpClient();
            try { client.Connect(ep); }
            catch
            {
                return;
            }
            NetworkStream stream = client.GetStream();
            reader = new BinaryReader(stream, Encoding.UTF8, true);
            writer = new BinaryWriter(stream, Encoding.UTF8, true);
            config = new Config("", "", "", 0);
            logs = new Log();
            GetCommands();
            SendCommand(new CommandRecievedEventArgs((int)CommandEnum.LogHistoryCommand, null, null));
            SendCommand(new CommandRecievedEventArgs((int)CommandEnum.ConfigCommand, null, ""));
        }

        public static WebClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebClient();
                }
                return instance;
            }
        }

        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;

        public Config config { get; set; }
        public Log logs;
        public List<string> handlers;

        public bool connected { get { return client.Connected; } }

        public void GetCommands()
        {
            Task t = new Task(() =>
            {
                while (true)
                {
                    string JCommand = reader.ReadString();
                    CommandRecievedEventArgs command = CommandRecievedEventArgs.FromJSON(JCommand);
                    if (command.CommandID == (int)CommandEnum.CloseClientCommand)
                    {
                        client.Close();
                        break;
                    }
                    
                    if (command.CommandID == (int)CommandEnum.LogChangedCommand)
                    {
                        logs.log.Insert(0, MessageRecievedEventArgs.FromJSON(command.Args[0]));
                        continue;
                    }

                    if (command.CommandID == (int)CommandEnum.LogHistoryCommand)
                    {
                        foreach (MessageRecievedEventArgs log in MessageRecievedEventArgs.LogFromJSON(command.Args[0]))
                        {
                            logs.log.Insert(0, log);
                        }
                        continue;
                    }

                    if (command.CommandID == (int)CommandEnum.HandlerRemoveCommand)
                    {
                        handlers.Remove(command.Args[0]);
                        continue;
                    }

                    if (command.CommandID == (int)CommandEnum.ConfigCommand)
                    {
                        config = (Config.FromJSON(command.Args[0]));
                        JObject cnJson = JObject.Parse(command.Args[0]);
                        JArray JHandler = (JArray)cnJson["Handler"];
                        foreach (JToken obj in JHandler.Children())
                        {
                            handlers.Add(obj.ToString());
                        }
                        continue;
                    }
                }
            });
            t.Start();
        }

        public void SendCommand(CommandRecievedEventArgs command)
        {
            if (client.Connected)
            {
                writer.Write(command.ToJSON());
            }
        }

        public void DeleteHandler(string dir)
        {
            string[] args = { dir };
            SendCommand(new CommandRecievedEventArgs((int)CommandEnum.HandlerRemoveCommand, args, null));
        }

        ~WebClient()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.CloseClientCommand, null, "");
            client.Close();
        }
    }
}