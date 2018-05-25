using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Image_Backuper_GUI.Message;
using Image_Backuper_GUI.Command;
using System.Runtime.Serialization.Json;
using Image_Backuper_GUI.Config;

namespace Image_Backuper_GUI.Client
{
    class GUIClient
    {
        private static GUIClient instance;

        private GUIClient()
        {
            countModels = 0;
            IPAddress IP = IPAddress.Parse(ConfigurationManager.AppSettings["IP"]);
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            IPEndPoint ep = new IPEndPoint(IP, port);
            client = new TcpClient();
            try { client.Connect(ep); }
            catch
            {
                return;
            }
            NetworkStream stream = client.GetStream();
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public static GUIClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GUIClient();
                }
                return instance;
            }
        }

        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;

        public bool connected { get { return client.Connected; } }

        public event EventHandler<CommandRecievedEventArgs> CommandReceived;

        private int countModels;

        public void Register()
        {
            if (++countModels == 2 && client.Connected)
            {
                Task t = new Task( () => 
                {
                    while (true)
                    {
                        string JCommand = reader.ReadString();
                        CommandRecievedEventArgs command = CommandRecievedEventArgs.FromJSON(JCommand);
                        if (command.CommandID == (int)CommandEnum.CloseCommand)
                        {
                            client.Close();
                            break;
                        }
                        CommandReceived?.Invoke(this, command);
                    }
                }
                );
                t.Start();
            }
        }

        public void SendCommand(CommandRecievedEventArgs command)
        {
            if (client.Connected)
            {
                writer.Write(command.ToJSON());
            }
        }

        ~GUIClient()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.CloseClientCommand, null, "");
            client.Close();
        }
    }
}
